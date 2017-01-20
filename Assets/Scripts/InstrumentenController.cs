using UnityEngine;

namespace Assets.Scripts
{
    public class InstrumentenController : MonoBehaviour {
        public MovieTexture movie;
        public bool active;
        public GESTURE instrument;
        public GameObject GestureSourceManager;
        private GestureSourceManager _GestureManager;

        private float confidence = 0.0f;

        void Awake()
        {
            Application.targetFrameRate = 30;
        }
        void Start () {
            movie.loop = true;
            _GestureManager = GestureSourceManager.GetComponent<GestureSourceManager>();
        }
	
        void Update () {
            if(! active)
            {
                return;
            }
            //confidence = _GestureManager.getConfidence(instrument);
            if (confidence > 0.3)
                movie.Play();
            else
                movie.Pause();
        }

    }
}
