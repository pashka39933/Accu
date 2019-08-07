using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameOverController : MonoBehaviour {

    public Text LatestScore, BestScore;

    Animation anim;

    bool menuLaunched = false, buttonFired = false;

	// Use this for initialization
	void Start () {
        LatestScore.text = PlayerPrefs.GetInt("LatestScore", 0).ToString();
        BestScore.text = "BEST " + PlayerPrefs.GetInt("BestScore", 0).ToString();

        anim = this.GetComponent<Animation>();
	}
	
	// Update is called once per frame
	void Update () {

        if (buttonFired && !menuLaunched && !anim.isPlaying)
        {
            anim.clip = anim.GetClip("FadeOutUI");
            anim.Play();
            menuLaunched = true;
        }

        if(menuLaunched && !anim.isPlaying)
            Application.LoadLevel("Scenes/MainMenu");

	}

    public void LaunchMenu()
    {
        buttonFired = true;
    }
}
