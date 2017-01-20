using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class PlaySphere : MonoBehaviour
    {

        public MovieTexture Sphere;
        private GestureSourceManager _gestureManager;
        private GESTURE[] _instruments;
        private Dictionary<GESTURE, float> _confidences;
        public Renderer Renderer;


        // Use this for initialization
        void Start ()
        {
            _instruments = new[] { GESTURE.GEIGE, GESTURE.TROMMEL, GESTURE.TROMPETE, GESTURE.FLOETE, GESTURE.GITARRE, GESTURE.HARFE };
            _confidences = new Dictionary<GESTURE, float>();
            Sphere.loop = true;
            Renderer = GetComponent<Renderer>();
            _gestureManager = FindObjectOfType<GestureSourceManager>();
            Renderer.material.mainTexture = Sphere;
            Sphere.Play();
        }
	
        // Update is called once per frame
        void FixedUpdate () {
            //foreach (var instrument in _instruments)
                //_confidences[instrument] = _gestureManager.getConfidence(instrument);
            KeyValuePair<GESTURE, float> bestconfidence = _confidences.First();
            foreach (KeyValuePair<GESTURE, float> confi in _confidences)
            {
                if (confi.Value > bestconfidence.Value) bestconfidence = confi;
            }
            var confidence = _confidences[bestconfidence.Key];
            print(confidence + " " + bestconfidence.Key);
            print(confidence);
            if (!Sphere.isPlaying) return;
            if (!(confidence > 0.3)) return;
            Sphere.Stop();
            switch (bestconfidence.Key)
            {
                case GESTURE.GEIGE:
                    SceneManager.LoadScene("PlayGeige");
                    break;
                case GESTURE.TROMMEL:
                    SceneManager.LoadScene("PlayTrommel");
                    break;
                case GESTURE.TROMPETE:
                    
                    break;
                case GESTURE.NICHTS:
                    SceneManager.LoadScene("PlaySphere");
                    break;
                case GESTURE.HARFE:
                    SceneManager.LoadScene("PlayHarfe");
                    break;
                case GESTURE.GITARRE:
                    SceneManager.LoadScene("PlayGitarre");
                    break;
                case GESTURE.FLOETE:
                    break;
                default:
                    SceneManager.LoadScene("PlaySphere");
                    return;
            }
        }
    }
}
