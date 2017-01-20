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
    public class PlayerDetector : MonoBehaviour {

        public int _maxPlayers = 3;
        private KinectSensor _Sensor;
        private BodySourceManager _BodyManager;
        
        private const float detectionTime = 10.9f;
        private float _remainingTime = 0f;
        private int _detectedPlayers = 0;

        // Use this for initialization
        void Start()
        {
            _remainingTime = detectionTime;
            _Sensor = KinectSensor.GetDefault();
            if (_Sensor != null)
            {
                if (!_Sensor.IsOpen)
                {
                    _Sensor.Open();
                }
            }
            
        }
        
        // Update is called once per frame
        void Update ()
        {
            getPlayerNumber();

            if(_detectedPlayers > 0)
            {
                SceneManager.LoadScene("MenuSpielwahl");
            }
            else
            {
                _remainingTime = detectionTime;
            }
            
        }

        private void getPlayerNumber()
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

            _detectedPlayers = 0;

            foreach (var body in data)
            {
                if (body == null)
                {
                    continue;
                }

                if (body.IsTracked)
                {
                    _detectedPlayers++;
                }
            }

            _detectedPlayers = System.Math.Min(_detectedPlayers, _maxPlayers);
        }
    }
}
