using UnityEngine;
using System.Collections;

public class TutorialController : MonoBehaviour {

    Animation anim;
    bool tapped = false;

	// Use this for initialization
	void Start () {
        anim = this.GetComponent<Animation>();
	}
	
	// Update is called once per frame
	void Update () {

	    if(Input.GetMouseButtonDown(0) && !anim.isPlaying && !tapped)
        {
            anim.clip = anim.GetClip("FadeOutUI");
            anim.Play();
            tapped = true;
        }

        if(tapped && !anim.isPlaying)
        {
            Application.LoadLevel("Scenes/Gameplay");
        }
	}
}
