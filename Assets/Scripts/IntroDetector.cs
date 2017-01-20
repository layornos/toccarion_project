using System.Collections;
using System.Collections.Generic;
using System.Linq;
//using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class IntroDetector : MonoBehaviour
    {
        private Animator _animator;
        public RuntimeAnimatorController IntroAnimator;
        public RuntimeAnimatorController IdleAnimator;

        private GestureSourceManager _gestureManager;
        
        // Use this for initialization
        void Start ()
        {
            _animator = GetComponent<Animator>();
            _animator.runtimeAnimatorController = IntroAnimator;
            _gestureManager = FindObjectOfType<GestureSourceManager>();

            StartCoroutine(WaitIntroPlayed());
        }
	
        // Update is called once per frame
        void FixedUpdate ()
        {
            
        }

        IEnumerator WaitIntroPlayed()
        {
            yield return new WaitForSeconds(1);
            _animator.runtimeAnimatorController = IdleAnimator;
        }
    }
}
