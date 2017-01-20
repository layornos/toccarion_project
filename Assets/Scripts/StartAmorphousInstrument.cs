using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartAmorphousInstrument : MonoBehaviour {
    public string nextScene;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // Detect mouse click
    private void OnMouseDown()
    {
        SceneManager.LoadScene(nextScene);
    }
}
