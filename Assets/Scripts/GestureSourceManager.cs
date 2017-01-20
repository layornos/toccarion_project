using System.Collections.Generic;
using Windows.Kinect;
using Assets.Scripts;
using Microsoft.Kinect.VisualGestureBuilder;
using UnityEngine;
using Gesture = Assets.Scripts.GESTURE;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Assets
{
    public class GestureSourceManager : MonoBehaviour {

        public int _maxPlayers = 3;
        private KinectSensor _Sensor;
        private VisualGestureBuilderFrameReader[] _Reader;
        private VisualGestureBuilderFrameSource[] _Source;
        private BodySourceManager _BodyManager;

		private readonly string gestureDatabase = @"Database\toccarion_erwachsen.gbd";

        private List<Dictionary<string, float>> confidences;

        private List<ulong> trackedIds = new List<ulong>();

        public UnityEngine.UI.Text remainingTimeText;
        private const float playTime = 30.9f;
        private float _remainingTime = 0f;

        // Use this for initialization
        void Start()
        {
            _remainingTime = playTime;
            _Sensor = KinectSensor.GetDefault();
            _Source = new VisualGestureBuilderFrameSource[_maxPlayers];
            _Reader = new VisualGestureBuilderFrameReader[_maxPlayers];
            if (_Sensor != null)
            {
                for (int i = 0; i < _maxPlayers; i++)
                {
                    _Source[i] = VisualGestureBuilderFrameSource.Create(_Sensor, 0);
                    this._Reader[i] = this._Source[i].OpenReader();
                    if (this._Reader != null)
                    {
                        this._Reader[i].IsPaused = false;
                        // this.vgbFrameReader.FrameArrived += this.Reader_GestureFrameArrived;
                    }

                    // load the gestures from the gesture database
                    using (VisualGestureBuilderDatabase database = VisualGestureBuilderDatabase.Create(this.gestureDatabase))
                    {
                        // we could load all available gestures in the database with a call to vgbFrameSource.AddGestures(database.AvailableGestures), 
                        // but for this program, we only want to track one discrete gesture from the database, so we'll load it by name
                        foreach (Microsoft.Kinect.VisualGestureBuilder.Gesture gesture in database.AvailableGestures)
                        {
                            this._Source[i].AddGesture(gesture);
                        }
                    }
                }

                if (!_Sensor.IsOpen)
                {
                    _Sensor.Open();
                }

                confidences = new List<Dictionary<string, float>>();
                for (int i = 0; i < _maxPlayers; i++)
                {
                    Dictionary<string, float> confidence;
                    confidence = new Dictionary<string, float>();
                    confidence.Add(gestureList.gestures[(int)GESTURE.GEIGE], 0);
                    confidence.Add(gestureList.gestures[(int)GESTURE.TROMMEL], 0);
                    //confidence.Add(gestureList.gestures[(int)GESTURE.TROMPETE], 0);
                    //confidence.Add(gestureList.gestures[(int)GESTURE.FLOETE], 0);
                    confidence.Add(gestureList.gestures[(int)GESTURE.GITARRE], 0);
                    confidence.Add(gestureList.gestures[(int)GESTURE.HARFE], 0);
                    //confidence.Add(gestureList.gestures[(int)GESTURE.CELLO], 0);
                    //confidence.Add(gestureList.gestures[(int)GESTURE.KLAVIER], 0);
                    confidences.Add(confidence);
                }
            }

        }

        internal float getConfidence(int player, Gesture instrument)
        {
            return confidences[player][gestureList.gestures[(int) instrument]];
        }

        // Update is called once per frame
        void Update ()
        {
            _remainingTime -= Time.deltaTime;
            if (_remainingTime <= 0)
            {
                SceneManager.LoadScene("Start");
            }
            remainingTimeText.text = ((int)_remainingTime) + "s";

            readTrackingIds();
            for (int i = 0; i < _maxPlayers; i++)
            {
                if (i < trackedIds.Count)
                {
                    _Source[i].TrackingId = trackedIds[i];

                    if (_Reader[i] != null)
                    {
                        var frame = _Reader[i].CalculateAndAcquireLatestFrame();
                        if (frame != null)
                        {
                            //Debug.Log(i);
                            EvalFrame(i, frame);
                            
                            frame.Dispose();
                            frame = null;
                       }
                    }
                }
                else
                {
                    _Source[i].TrackingId = 0;
                }
            }
            foreach (Microsoft.Kinect.VisualGestureBuilder.Gesture gesture in this._Source[0].Gestures)
            {
                if (gesture.GestureType == GestureType.Discrete)
                {
                    for(int i = 0; i < _maxPlayers; i++)
                    {
                        //Debug.Log("Player: " + i + ": " + gesture.Name + ": " + confidences[i][gesture.Name]);
                       
                    }
                }
            }
        }

        private void readTrackingIds()
        {
            _BodyManager = FindObjectOfType<BodySourceManager>();
            if (_BodyManager == null)
            {
                return;
            }

            Windows.Kinect.Body[] data = _BodyManager.GetData();
            if (data == null)
            {
                return;
            }

            trackedIds.Clear();

            foreach (var body in data)
            {
                if (body == null)
                {
                    continue;
                }

                if (body.IsTracked)
                {
                    trackedIds.Add(body.TrackingId);
                }
            }
        }

        private void EvalFrame(int player, VisualGestureBuilderFrame frame)
        {
            if (frame != null)
            {
                // get the discrete gesture results which arrived with the latest frame
                Dictionary<Microsoft.Kinect.VisualGestureBuilder.Gesture, DiscreteGestureResult> discreteResults = frame.DiscreteGestureResults;

                if (discreteResults != null)
                {
                    // we only have one gesture in this source object, but you can get multiple gestures
                    foreach (Microsoft.Kinect.VisualGestureBuilder.Gesture gesture in this._Source[player].Gestures)
                    {
                        if (gesture.GestureType == GestureType.Discrete)
                        {
                            DiscreteGestureResult result = null;
                            discreteResults.TryGetValue(gesture, out result);

                            if (result != null)
                            {
                                if (result.Detected)
                                {
                                    confidences[player][gesture.Name] = result.Confidence;
                                }
                                else
                                {
                                    confidences[player][gesture.Name] = 0.0f;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void resetTimer()
        {
            _remainingTime = playTime;
        }
    }
}
