using System;
using System.Collections.Generic;
using Windows.Kinect;
using Microsoft.Kinect.VisualGestureBuilder;

namespace Assets.Scripts
{
    internal class GestureDetector
    {
        private readonly string gestureDatabase = @"Database\toccarion.gbd";
        private GESTURE _gesture = GESTURE.NICHTS;
        private string _name = "nichts";
        private VisualGestureBuilderFrameSource _visualGestureBuilderFrameSource;
        private VisualGestureBuilderFrameReader _visualGestureBuilderFrameReader;
        private float _confidence = 0.0f;

        public GestureDetector(KinectSensor kinectSensor, GESTURE gestureCode)
        {
            CheckIfKinectIsConnected(kinectSensor);
            SetGestureFields(gestureCode);
            CerateTheVgbSource(kinectSensor);
            OpenReaderForVgbFrames();
            LoadGesturesFromDatabase();
        }

        private static void CheckIfKinectIsConnected(KinectSensor kinectSensor)
        {
            if (IsKinectConnected(kinectSensor))
                throw new ArgumentNullException("kinectSensor");
        }

        private static bool IsKinectConnected(KinectSensor kinectSensor)
        {
            return kinectSensor == null;
        }

        private void SetGestureFields(GESTURE gestureCode)
        {
            _gesture = gestureCode;
            _name = gestureList.gestures[(int)_gesture];
        }

        private void CerateTheVgbSource(KinectSensor kinectSensor)
        {
            _visualGestureBuilderFrameSource = VisualGestureBuilderFrameSource.Create(kinectSensor, 0);
            _visualGestureBuilderFrameSource.TrackingIdLost += Source_TrackingIdLost;
        }

        private void OpenReaderForVgbFrames()
        {
            _visualGestureBuilderFrameReader = _visualGestureBuilderFrameSource.OpenReader();
            if (IsThereAFrameReader())
                _visualGestureBuilderFrameReader.IsPaused = false;
        }

        private bool IsThereAFrameReader()
        {
            return _visualGestureBuilderFrameReader != null;
        }

        private void LoadGesturesFromDatabase()
        {
            using (VisualGestureBuilderDatabase database = VisualGestureBuilderDatabase.Create(gestureDatabase))
            {
                LoadGesturesFromDatabase(database);
            }
        }

        private void LoadGesturesFromDatabase(VisualGestureBuilderDatabase database)
        {
            foreach (Gesture gesture in database.AvailableGestures)
                _visualGestureBuilderFrameSource.AddGesture(gesture);
        }

        public float GetConfidence()
        {
            var frame = _visualGestureBuilderFrameReader.CalculateAndAcquireLatestFrame();
            EvalFrame(frame);
            return _confidence;
        }

        public ulong TrackingId
        {
            get
            {
                return _visualGestureBuilderFrameSource.TrackingId;
            }

            set
            {
                if (_visualGestureBuilderFrameSource.TrackingId != value)
                    _visualGestureBuilderFrameSource.TrackingId = value;
            }
        }

        public bool IsPaused
        {
            get
            {
                return _visualGestureBuilderFrameReader.IsPaused;
            }

            set
            {
                if (_visualGestureBuilderFrameReader.IsPaused != value)
                {
                    _visualGestureBuilderFrameReader.IsPaused = value;
                }
            }
        }

        private void EvalFrame(VisualGestureBuilderFrame frame)
        {
            if (frame == null) return;
            var discreteResults = GetDiscreteGestureResults(frame);
            EvaluateGesture(discreteResults);
        }

        private void EvaluateGesture(Dictionary<Gesture, DiscreteGestureResult> discreteResults)
        {
            foreach (var gesture in _visualGestureBuilderFrameSource.Gestures)
                TryUpdateConfidence(discreteResults, gesture);
        }

        private void TryUpdateConfidence(Dictionary<Gesture, DiscreteGestureResult> discreteResults, Gesture gesture)
        {
            if (!IsRightGesture(gesture)) return;
            var result = TryGetDiscreteResults(discreteResults, gesture);
            if (result == null) return;
            UpdateConfidence(result);
        }

        private void UpdateConfidence(DiscreteGestureResult result)
        {
            if (IsThereAValidConfidence(result))
                _confidence = result.Confidence;
        }

        private static DiscreteGestureResult TryGetDiscreteResults(Dictionary<Gesture, DiscreteGestureResult> discreteResults, Gesture gesture)
        {
            DiscreteGestureResult result;
            discreteResults.TryGetValue(gesture, out result);
            return result;
        }

        private static bool IsThereAValidConfidence(DiscreteGestureResult result)
        {
            return result.Detected && result.Confidence > 0;
        }

        private bool IsRightGesture(Gesture gesture)
        {
            return !(IsGestureTypeDiscrete(gesture) || !MatchingGestures(gesture));
        }

        private bool MatchingGestures(Gesture gesture)
        {
            return gesture.Name.Equals(_name);
        }

        private static bool IsGestureTypeDiscrete(Gesture gesture)
        {
            return gesture.GestureType != GestureType.Discrete;
        }

        private static Dictionary<Gesture, DiscreteGestureResult> GetDiscreteGestureResults(VisualGestureBuilderFrame frame)
        {
            var discreteResults = frame.DiscreteGestureResults;
            return discreteResults;
        }

        internal GESTURE GetGesture()
        {
            return _gesture;
        }

        private void Source_TrackingIdLost(object sender, TrackingIdLostEventArgs e)
        {
            _confidence = 0.0f;
        }
    }
}