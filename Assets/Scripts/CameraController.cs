using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public bool cameraReady = true;
    public Vector3 cameraDest;

	// Use this for initialization
	void Start () {
        cameraDest = this.transform.position;

        CoinsController cc = GameObject.FindObjectOfType<CoinsController>();
        Instantiate(cc.characters[PlayerPrefs.GetInt("Character", 0)]);
	}
	
	// Update is called once per frame
	void Update () {
	    if(!cameraReady)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, cameraDest, Time.deltaTime * 8 * 2 / Time.timeScale);
            if (Vector3.Distance(this.transform.position, cameraDest) < 0.01f)
            {
                this.transform.position = cameraDest;
                cameraReady = true;
            }
        }
	}
}
