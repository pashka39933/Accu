using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour {

    Vector3 mainCameraDest;
    bool gameLaunched = false, shopLaunched = false, startButtonFired = false, shopButtonFired = false, closeShopButtonFired = false, menuLaunched = false;

    public EllipsoidParticleEmitter particles;

    public Animation playerAnim, shopAnim, menuAnim;

    public Button startGameButton, shopButton;

    AudioSource audioSource;

    public Image Sound;
    public Sprite soundOn, soundOff;

    void Start()
    {

        mainCameraDest = this.transform.position;

        audioSource = this.GetComponent<AudioSource>();

        if (PlayerPrefs.GetInt("Sounds", 1) == 0)
        {
            AudioListener.volume = 0;
            Sound.sprite = soundOff;
        }
        else
        {
            AudioListener.volume = 1;
            Sound.sprite = soundOn;
        }
    }

	// Update is called once per frame
	void Update () {

        if (startButtonFired && !gameLaunched && !menuAnim.isPlaying && !playerAnim.isPlaying)
        {
        
            menuAnim.clip = menuAnim.GetClip("FadeOutUI");
            menuAnim.Play();
            mainCameraDest.y += 2.51f;
            particles.emit = false;

            shopButton.interactable = false;

            gameLaunched = true;
        
        }

        if(shopButtonFired && !shopLaunched && !menuAnim.isPlaying && !playerAnim.isPlaying)
        {

            menuAnim.clip = menuAnim.GetClip("FadeOutUI");
            menuAnim.Play();
            playerAnim.clip = playerAnim.GetClip("FadeOutPlayerMenu");
            playerAnim.Play();

            startGameButton.interactable = false;

            shopLaunched = true;

        }

        if(closeShopButtonFired && !menuLaunched && !shopAnim.isPlaying)
        {

            shopAnim.clip = menuAnim.GetClip("FadeOutUI");
            shopAnim.Play();
            menuLaunched = true;

        }



        // ### ### ###



        if (gameLaunched)
        {
            if (!menuAnim.isPlaying)
            {
                this.transform.position = Vector3.Lerp(this.transform.position, mainCameraDest, Time.deltaTime * 6);

                if (Vector3.Distance(this.transform.position, mainCameraDest) < 0.001f && particles == null)
                {
                    if(PlayerPrefs.GetInt("FirstRun", 1) == 1)
                    {
                        PlayerPrefs.SetInt("FirstRun", 0);
                        Application.LoadLevel("Scenes/Tutorial");
                    }
                    else
                        Application.LoadLevel("Scenes/Gameplay");
                }
            }
        }

        if (shopLaunched && !menuAnim.isPlaying && !playerAnim.isPlaying)
        {
            menuAnim.gameObject.SetActive(false);
            shopAnim.gameObject.SetActive(true);
            shopAnim.clip = shopAnim.GetClip("FadeInUI");
            shopAnim.Play();
            shopLaunched = false;
            shopButtonFired = false;
        }

        if(menuLaunched && !shopAnim.isPlaying)
        {
            shopAnim.gameObject.SetActive(false);
            menuAnim.gameObject.SetActive(true);
            menuAnim.clip = menuAnim.GetClip("FadeInUI");
            menuAnim.Play();
            playerAnim.clip = playerAnim.GetClip("FadeInPlayerMenu");
            playerAnim.Play();

            startGameButton.interactable = true;

            menuLaunched = false;
            closeShopButtonFired = false;
        }
	}

    public void StartGame()
    {
        startButtonFired = true;
    }

    public void LoadShop()
    {
        audioSource.Play();
        shopButtonFired = true;
    }

    public void LoadMenu()
    {
        audioSource.Play();
        closeShopButtonFired = true;
    }

    public void RateButton()
    {
        audioSource.Play();
        Application.OpenURL("http://www.jakoma.eu");
    }

    public void SoundButton()
    {
        int currentState = PlayerPrefs.GetInt("Sounds", 1);
        if(currentState == 1)
        {
            PlayerPrefs.SetInt("Sounds", 0);
            AudioListener.volume = 0;
            Sound.sprite = soundOff;
        }
        else
        {
            PlayerPrefs.SetInt("Sounds", 1);
            AudioListener.volume = 1;
            Sound.sprite = soundOn;
            audioSource.Play();
        }
    }
}
