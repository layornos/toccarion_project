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
    public class GestureSourceManagerDetector : MonoBehaviour {

        public int _maxPlayers = 3;
        private KinectSensor _Sensor;
        private BodySourceManager _BodyManager;

        public UnityEngine.UI.Text playerNumText;
        public UnityEngine.UI.Text remainingTimeText;
        public SpriteRenderer backgroundRenderer;
        public GameObject timeLineCube;

        private const float detectionTime = 10.9f;
        private float _remainingTime = 0f;
        private int _detectedPlayers = 0;
        private float originalScaleX;

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

            originalScaleX = timeLineCube.transform.localScale.x;
        }
        
        // Update is called once per frame
        void Update ()
        {
            getPlayerNumber();
            playerNumText.text = _detectedPlayers + " Player";
            if(_detectedPlayers != 1)
            {
                playerNumText.text += "s";
            }

            _remainingTime -= Time.deltaTime;

            backgroundRenderer.sprite = Resources.Load<Sprite>(_detectedPlayers + "Player");

            if (_remainingTime <= 0)
            {
                if(_detectedPlayers > 0)
                {
                    SceneManager.LoadScene(_detectedPlayers + "Player");     
                }
                else
                {
                    _remainingTime = detectionTime;
                }
            }

            remainingTimeText.text = ((int)_remainingTime) + "s";
            float scaleX = originalScaleX * (_remainingTime / detectionTime);
            float scaleY = timeLineCube.transform.localScale.y;
            float scaleZ = timeLineCube.transform.localScale.z;
            timeLineCube.transform.localScale = new Vector3(scaleX, scaleY, scaleZ);
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
