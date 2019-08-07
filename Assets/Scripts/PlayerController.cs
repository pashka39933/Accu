using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    Transform iceCollider;

    public GameObject spotPrefab;
    GameObject comboHolder;
    Image forceImage;
    Text scoreText;

    public CameraController camController;

    BackgroundColorController bgController;

    GameObject currentSpot;

    float forceStartTime;
    bool shotPerformed = false, gameoverChecking = false, wasMouseDown = false;
    Rigidbody rb;

    int comboCounter = 1, score = 0;

    CoinsController coinsController;

    EllipsoidParticleEmitter playerParticles;

    public AudioClip powerbarSound, releaseSound;

    public Sprite[] spotSprites;

	// Use this for initialization
	void Start () {

        iceCollider = GameObject.Find("IceCollider").transform;
        comboHolder = GameObject.Find("ComboHolder");
        forceImage = GameObject.Find("PowerBar").GetComponent<Image>();
        scoreText = GameObject.Find("Score").GetComponent<Text>();
        camController = GameObject.Find("Main Camera Container").GetComponent<CameraController>();
        bgController = GameObject.Find("Main Camera").GetComponent<BackgroundColorController>();
        coinsController = GameObject.FindObjectOfType<CoinsController>();
        playerParticles = this.GetComponent<EllipsoidParticleEmitter>();

        Time.timeScale = 3.67f;
        
        rb = this.GetComponent<Rigidbody>();

        spotPrefab.GetComponent<SpriteRenderer>().sprite = spotSprites[0];
        currentSpot = (GameObject)Instantiate(spotPrefab, new Vector2(this.transform.position.x, this.transform.position.y + 5.925f - ((float)Random.Range(0, 450)/100)), Quaternion.identity);

        iceCollider.transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));
        this.transform.rotation = Quaternion.identity;

        scoreText.text = score.ToString();

	}
	
	// Update is called once per frame
    void Update()
    {

        if (rb.velocity == Vector3.zero)
        {
            if (!shotPerformed && camController.cameraReady)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    forceStartTime = Time.time;
                    Vector3 tmp = camController.GetComponentInChildren<Camera>().ScreenToWorldPoint(Input.mousePosition);
                    tmp.y += 1f;
                    tmp.z = 0;
                    forceImage.transform.position = tmp;
                    wasMouseDown = true;

                    AudioSource powerBar = forceImage.GetComponent<AudioSource>();
                    powerBar.pitch = 1.1f;
                    powerBar.volume = 0.1f;
                    powerBar.clip = powerbarSound;
                    powerBar.Play();
                }
                else if(Input.GetMouseButton(0) && wasMouseDown)
                {
                    forceImage.fillAmount = (Time.time - forceStartTime) / 3f;
                }
                else if (Input.GetMouseButtonUp(0) && wasMouseDown)
                {
                    if (forceStartTime == 0)
                        return;
                    float forceAmount = Time.time - forceStartTime;
                    if (forceAmount > 3f)
                        forceAmount = 3f;

                    Vector2 direction = currentSpot.transform.position - this.transform.position;
                    direction = direction.normalized;

                    forceAmount = forceAmount * 1.44f;

                    rb.AddForce(direction.x * forceAmount, direction.y * forceAmount, 0, ForceMode.VelocityChange);

                    rb.angularVelocity = Vector3.forward * forceAmount * 1.75f;

                    if (playerParticles != null)
                        playerParticles.emit = true;

                    forceImage.fillAmount = 0;


                    AudioSource powerBar = forceImage.GetComponent<AudioSource>();
                    powerBar.pitch = 1.667f;
                    powerBar.volume = 0.1f;
                    powerBar.clip = releaseSound;
                    powerBar.Play();
                    

                    shotPerformed = true;

                    wasMouseDown = false;
                }
            }
            else
            {
                if (playerParticles != null)
                    playerParticles.emit = false;

                if (Vector2.Distance(this.transform.position, currentSpot.transform.position) < 1.15f)
                {

                    if (Vector2.Distance(this.transform.position, currentSpot.transform.position) < 0.2f)
                        comboCounter = comboCounter * 2;
                    else
                        comboCounter = 1;

                    shotPerformed = false;

                    StartCoroutine(ComboSound(0.3f, comboCounter));

                    StartCoroutine(MultipleParticles(comboCounter));

                    StartCoroutine(SleepAndMoveCam(1.5f, comboCounter));

                    score+=comboCounter;

                    PlayerPrefs.SetInt("LatestScore", score);

                    if (PlayerPrefs.GetInt("BestScore", 0) < score)
                        PlayerPrefs.SetInt("BestScore", score);

                    Animation anim = scoreText.GetComponent<Animation>();
                    anim.clip = anim.GetClip("ScoreAdd");
                    anim.Play();
                    scoreText.text = score.ToString();
                }
            }

            if(shotPerformed && camController.cameraReady && !gameoverChecking)
            {
                StartCoroutine(SleepAndCheckGameover(3.7f));
            }
        }

    }

    IEnumerator MultipleParticles(int combo)
    {
        currentSpot.GetComponent<EllipsoidParticleEmitter>().Emit(1);
        yield return null;
    }

    IEnumerator SleepAndMoveCam(float sec, int combo)
    {
        shotPerformed = false;

        currentSpot.GetComponent<EllipsoidParticleEmitter>().Emit(1);

        if (combo > 1)
        {
            GameObject comboText = comboHolder.transform.GetChild(0).gameObject;
            comboHolder.transform.position = currentSpot.transform.position;
            comboText.GetComponent<Text>().text = "+" + combo.ToString();
            float scale = (70f + ((float)combo / 2f) * 10f);
            if (scale > 120f)
                scale = 120f;
            comboText.GetComponent<RectTransform>().sizeDelta = Vector3.one * scale;
            comboText.GetComponent<Animation>().Play();

            int colorID = Random.Range(0, bgController.bgTints.Count);
            if (colorID == bgController.currentColorIndex)
                colorID = (colorID + 1) % bgController.bgTints.Count;
            bgController.destColor = bgController.bgTints[colorID];
            bgController.currentColorIndex = colorID;

            coinsController.AddCoins(1, 1);
        }

        yield return new WaitForSeconds(sec);

        camController.cameraDest = new Vector3(this.transform.position.x, this.transform.position.y, -10);
        camController.cameraReady = false;

        Animation anim = currentSpot.GetComponent<Animation>();
        anim.clip = anim.GetClip("FadeOut");
        anim.Play();

        if (combo > 1)
        {
            int rnd = Random.Range(1, spotSprites.Length);
            if (spotSprites[rnd] == spotPrefab.GetComponent<SpriteRenderer>().sprite)
                rnd = (rnd + 1) % spotSprites.Length;
            spotPrefab.GetComponent<SpriteRenderer>().sprite = spotSprites[rnd];
        }

        currentSpot = (GameObject)Instantiate(spotPrefab, new Vector2(this.transform.position.x + (float)Random.Range(-120, 120) / 100f, this.transform.position.y + 5.925f - ((float)Random.Range(0, 450) / 100)), Quaternion.identity);
    }

    IEnumerator SleepAndCheckGameover(float sec)
    {
        gameoverChecking = true;

        yield return new WaitForSeconds(sec);
        gameoverChecking = false;
        if (shotPerformed && camController.cameraReady)
        {

            for (int i = 0; i < this.transform.childCount; i++)
            {
                Destroy(this.transform.GetChild(0).gameObject);
            }

            Animation anim = currentSpot.GetComponent<Animation>();
            anim.clip = anim.GetClip("FadeOut");
            anim.Play();
            anim = this.GetComponent<Animation>();
            anim.clip = anim.GetClip("FadeOut");
            anim.Play();
            anim = scoreText.GetComponent<Animation>();
            anim.clip = anim.GetClip("FadeOutCanvasGroup");
            anim.Play();

            yield return new WaitForSeconds(2);
            Time.timeScale = 1;
            Application.LoadLevel("Scenes/GameOver");

        }
    }

    IEnumerator ComboSound(float thres, int comboCount)
    {

        AudioSource spotAudio = currentSpot.GetComponents<AudioSource>()[0];
        spotAudio.pitch = 1 + 0.1f * Mathf.Log(comboCount, 2);
        if (spotAudio.pitch > 1.5f)
            spotAudio.pitch = 1.5f;
        spotAudio.Play();

        if (comboCount > 1)
        {
            yield return new WaitForSeconds(thres);
            spotAudio = currentSpot.GetComponents<AudioSource>()[1];
            spotAudio.pitch = 1 + 0.4f * (Mathf.Log(comboCount, 2) + 1);
            if (spotAudio.pitch > 3f)
                spotAudio.pitch = 3f;
            spotAudio.Play(); 
        }

    }
}
