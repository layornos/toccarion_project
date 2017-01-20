using System;
using System.Collections.Generic;
using Windows.Kinect;
using UnityEngine;

namespace Assets.Scripts
{
    internal class KinectController
    {
        private static KinectController _instance;
        private const int MaxBodies = 2;
        private KinectSensor _kinectSensor;
        private List<GestureDetector> _gestureDetectorList;

        private KinectController()
        {
            InitKinectSensor();
            GetFrameReader();
            InitGestureDetecionObjects();
            CreateGestureDetectorForEachBody();
        }

        private void InitGestureDetecionObjects()
        {
            _gestureDetectorList = new List<GestureDetector>();
        }

        private void GetFrameReader()
        {
            _kinectSensor.OpenMultiSourceFrameReader(FrameSourceTypes.Body);
        }

        private void InitKinectSensor()
        {
            _kinectSensor = KinectSensor.GetDefault();
            _kinectSensor.Open();
        }

        private void CreateGestureDetectorForEachBody()
        {
            var maxBodies = Math.Min(MaxBodies, _kinectSensor.BodyFrameSource.BodyCount);
            for (var i = 0; i < maxBodies; ++i)
            {
                InitializeGestureDetector(i);
            }
        }

        private void InitializeGestureDetector(int gesture)
        {
            try
            {
                TryInitializeGestureDetector(gesture);
            }
            catch (Exception)
            {
                Debug.LogError("[ERROR] No Kinect connected!");
            }
        }

        private void TryInitializeGestureDetector(int gesture)
        {
            var detector = new GestureDetector(_kinectSensor, gestureCode: (GESTURE)gesture);
            _gestureDetectorList.Add(detector);
        }

        public static KinectController GetInstance()
        {
            if (_instance == null)
            {
                _instance = new KinectController();
            }
            return _instance;
        }

        public float GetConfidence(GESTURE gesture)
        {
            var confidence = 0.0f;
            foreach(var detector in _gestureDetectorList)
            {
                if (detector.GetGesture() == gesture)
                {
                    confidence = detector.GetConfidence();
                }
            }
            return confidence;
        }
    }
}