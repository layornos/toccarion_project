using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class Soundtrack : MonoBehaviour {
        private string[] _instrumente;
        private Animator _animator;
        private Dictionary<int, AudioSource> _instrumentPlayHashes;

        public Soundtrack(string[] instrumente)
        {
            _instrumente = instrumente;
        }

        // Use this for initialization
        void Start () {
            //DontDestroyOnLoad(gameObject);
            
            var sounds = FindObjectsOfType<AudioSource>();
            foreach (var audioSource in sounds)
            {
                audioSource.volume = 0.05f;
                audioSource.Play();
               
            }
            _instrumente = gestureList.gestures;

            _instrumentPlayHashes = new Dictionary<int, AudioSource>();

            foreach (var audioSource in sounds)
            {
                var audioSourceInstrumentName = audioSource.tag.Replace("Ton", "");
                _instrumentPlayHashes[Animator.StringToHash("Base Layer." + audioSourceInstrumentName + "Play")] = audioSource;
            }

        }
	
        // Update is called once per frame
        void Update()
        {
            _animator = GetComponent<Animator>();
            AnimatorStateInfo asi = _animator.GetCurrentAnimatorStateInfo(0);

            foreach (var hashes in _instrumentPlayHashes)
                hashes.Value.volume = hashes.Key != asi.fullPathHash ? 0.05f : 1.0f;
        }
    }
}
