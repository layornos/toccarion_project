﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.Kinect;
using Microsoft.Kinect.VisualGestureBuilder;

class KinectController
{
    static private KinectController instance;
    const int MAX_BODIES = 3;

    /// <summary> Active Kinect sensor </summary>
    private KinectSensor kinectSensor = null;

    /// <summary> Array for the bodies (Kinect will track up to 6 people simultaneously) </summary>
    private Body[] bodies = null;

    /// <summary> Reader for body frames </summary>
    private BodyFrameReader bodyFrameReader = null;

    /// <summary> List of gesture detectors, there will be one detector created for each potential body (max of 6) </summary>
    private List<GestureDetector> gestureDetectorList = null;

    private KinectController()
    {
        // only one sensor is currently supported
        this.kinectSensor = KinectSensor.GetDefault();

        // open the sensor
        this.kinectSensor.Open();

        // open the reader for the body frames
        this.bodyFrameReader = this.kinectSensor.BodyFrameSource.OpenReader();

        // set the BodyFramedArrived event notifier
        this.bodyFrameReader.FrameArrived += this.Reader_BodyFrameArrived;

        // initialize the gesture detection objects for our gestures
        this.gestureDetectorList = new List<GestureDetector>();

        // create a gesture detector for each body (6 bodies => 6 detectors)
        int maxBodies = Math.Min(MAX_BODIES, this.kinectSensor.BodyFrameSource.BodyCount);
        for (int i = 0; i < maxBodies; ++i)
        {
            GestureDetector detector = new GestureDetector(this.kinectSensor, (GESTURE) i);
            this.gestureDetectorList.Add(detector);
        }
    }

    static public KinectController getInstance()
    {
        if (instance == null)
        {
            instance = new KinectController();
        }
        return instance;
    }

    /// <summary>
    /// Handles the body frame data arriving from the sensor and updates the associated gesture detector object for each body
    /// </summary>
    /// <param name="sender">object sending the event</param>
    /// <param name="e">event arguments</param>
    private void Reader_BodyFrameArrived(object sender, BodyFrameArrivedEventArgs e)
    {
        bool dataReceived = false;

        using (BodyFrame bodyFrame = e.FrameReference.AcquireFrame())
        {
            if (bodyFrame != null)
            {
                if (this.bodies == null)
                {
                    // creates an array of 6 bodies, which is the max number of bodies that Kinect can track simultaneously
                    this.bodies = new Body[Math.Min(MAX_BODIES, bodyFrame.BodyCount)];
                }

                // The first time GetAndRefreshBodyData is called, Kinect will allocate each Body in the array.
                // As long as those body objects are not disposed and not set to null in the array,
                // those body objects will be re-used.
                bodyFrame.GetAndRefreshBodyData(this.bodies);
                dataReceived = true;
            }
        }

        if (dataReceived)
        {
            // we may have lost/acquired bodies, so update the corresponding gesture detectors
            if (this.bodies != null)
            {
                // loop through all bodies to see if any of the gesture detectors need to be updated
                int maxBodies = Math.Min(MAX_BODIES, this.kinectSensor.BodyFrameSource.BodyCount);
                for (int i = 0; i < maxBodies; ++i)
                {
                    Body body = this.bodies[i];
                    ulong trackingId = body.TrackingId;

                    // if the current body TrackingId changed, update the corresponding gesture detector with the new value
                    if (trackingId != this.gestureDetectorList[i].TrackingId)
                    {
                        this.gestureDetectorList[i].TrackingId = trackingId;

                        // if the current body is tracked, unpause its detector to get VisualGestureBuilderFrameArrived events
                        // if the current body is not tracked, pause its detector so we don't waste resources trying to get invalid gesture results
                        this.gestureDetectorList[i].IsPaused = trackingId == 0;
                    }
                }
            }
        }
    }

    public float getConfidence(GESTURE gesture)
    {
        float confidence = 0.0f;
        foreach(GestureDetector detector in this.gestureDetectorList)
        {
            if (detector.getGesture())
            {
                confidence = detector.getConfidence();
            }
        }
        return confidence;
    }
}