using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class PlayInstrument : MonoBehaviour
    {
        public MovieTexture ToInstrument;
        public MovieTexture ToSphere;
        public MovieTexture InstrumentMovie;
        public GESTURE Instrument;
        private GestureSourceManager _gestureManager;
        public Renderer Renderer;
//        public AudioSource InstrumentMusic;

        private float _confidence;
        private bool _minimumPlaytimeIsOver;
        private bool _outroPlaying;

        void Awake()
        {
            Application.targetFrameRate = 30;
        }

        void Start()
        {
            InstrumentMovie.loop = true;
            _minimumPlaytimeIsOver = false;
            _outroPlaying = false;
            Renderer = GetComponent<Renderer>();
            Renderer.material.mainTexture = ToInstrument;
            ToInstrument.Play();
            StartCoroutine(WaitIntroPlayed());
        }

        void FixedUpdate()
        {
            _gestureManager = FindObjectOfType<GestureSourceManager>();
            //_confidence = _gestureManager.getConfidence(Instrument);
            print(_confidence);
            if (_confidence < 0.1 && _minimumPlaytimeIsOver && !_outroPlaying)
            {
                Renderer.material.mainTexture = ToSphere;
                ToSphere.Play();
                StartCoroutine(WaitOutroPlayed());
            }
        }
        IEnumerator WaitIntroPlayed()
        {
            print(Time.time);
            yield return new WaitForSeconds(5);
            Renderer.material.mainTexture = InstrumentMovie;
            InstrumentMovie.Play();
            //InstrumentMusic.Play();
            StartCoroutine(WaitForMinimumPlaytime());
        }

        IEnumerator WaitForMinimumPlaytime()
        {
            print(Time.time);
            yield return new WaitForSeconds(5);
            _minimumPlaytimeIsOver = true;
        }

        IEnumerator WaitOutroPlayed()
        {
            yield return new WaitForSeconds(1);
            ToSphere.Stop();
            //InstrumentMusic.Stop();
            SceneManager.LoadScene("PlaySphere");
        }
    }
}
