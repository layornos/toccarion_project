using System.Collections;
using System.Collections.Generic;
using System.Linq;
//using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class Intro : MonoBehaviour
    {
        public int playerNumber = 0;

        private Animator _animator;
        public RuntimeAnimatorController IntroAnimator;
        public RuntimeAnimatorController IdleAnimator;

        private GESTURE[] _instruments;
        private Dictionary<GESTURE, float> _confidences;
        private GestureSourceManager _gestureManager;
        private int _confidenceThreshold;
        private KeyValuePair<GESTURE, float> _bestconfidence;

        // Use this for initialization
        void Start ()
        {
            _confidenceThreshold = 0;
            _animator = GetComponent<Animator>();
            _animator.runtimeAnimatorController = IntroAnimator;
            _instruments = new[] { GESTURE.GEIGE, GESTURE.TROMMEL, GESTURE.GITARRE, GESTURE.HARFE };
            _confidences = new Dictionary<GESTURE, float>();
            foreach (var instrument in _instruments)
                _confidences.Add(instrument, 0.0f);
            _gestureManager = FindObjectOfType<GestureSourceManager>();

            StartCoroutine(WaitIntroPlayed());
        }
	
        // Update is called once per frame
        void FixedUpdate ()
        {
            //print("Confidence Threshold: " + _confidenceThreshold);
            if (_animator.runtimeAnimatorController != IdleAnimator)
                return;

            SetBestConfidenceKeyValue();
            if (_bestconfidence.Value > 0.5f) _confidenceThreshold = 0;
            else _confidenceThreshold++;

            if (CanInstrumentBeStopped())
                StopPlayingTheInstrument();
            else if (!_animator.GetBool("PlayInstrument") && _bestconfidence.Value > 0.4f)
                StartPlayingInstrument();
        }

        private bool CanInstrumentBeStopped()
        {
            return _confidenceThreshold > 120 && IsInstrumentPlaying();
        }

        private bool IsInstrumentPlaying()
        {
            return _animator.GetBool("PlayInstrument"); // && _animator.GetBool("Outro");
        }

        private void SetBestConfidenceKeyValue()
        {
            foreach (var instrument in _instruments)
                _confidences[instrument] = _gestureManager.getConfidence(playerNumber, instrument);

            _bestconfidence = _confidences.First();
            foreach (var confi in _confidences)
            {
                if (confi.Value > _bestconfidence.Value) _bestconfidence = confi;
            }
            Debug.Log(_bestconfidence);
        }

        private void StopPlayingTheInstrument()
        {
            _animator.SetInteger("Instrument", 0);
            _animator.SetBool("PlayInstrument", false);
        }

        private void StartPlayingInstrument()
        {
            _gestureManager.resetTimer();
            switch (_bestconfidence.Key)
            {
                case GESTURE.GEIGE:
                    PlayInstrument(1);
                    break;
                case GESTURE.TROMMEL:
                    PlayInstrument(5);
                    break;
                case GESTURE.HARFE:
                    PlayInstrument(4);
                    break;
                case GESTURE.GITARRE:
                    PlayInstrument(2);
                    break;
            }
            
        }

        private void PlayInstrument(int instrument)
        {
            //_animator.SetBool("Outro", false);
            _animator.SetInteger("Instrument", instrument);
            //StartCoroutine(WaitToPlayOutro());
            _animator.SetBool("PlayInstrument", true);
        }

        IEnumerator WaitIntroPlayed()
        {
            yield return new WaitForSeconds(1);
            _animator.runtimeAnimatorController = IdleAnimator;
        }

        IEnumerator WaitToPlayOutro()
        {
            yield return new WaitForSeconds(2);
            //_animator.SetBool("Outro", true);
            _animator.SetBool("PlayInstrument", false);
            _animator.SetInteger("Instrument", 0);
            StartCoroutine(WaitToSetIdleState());
        }

        IEnumerator WaitToSetIdleState()
        {
            yield return new WaitForSeconds(0.3f);
        }
    }
}
