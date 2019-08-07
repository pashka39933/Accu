using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CoinsController : MonoBehaviour {

    public Text coinAmout, addAmount;
    public Animation addAnim;
    public Transform positioner;

    public GameObject[] characters;

	// Use this for initialization
	void Start () {

        if (GameObject.FindObjectsOfType<CoinsController>().Length > 1)
            Destroy(this.gameObject);
        else
        {
            GameObject.DontDestroyOnLoad(this.gameObject);

            //if (!PlayerPrefs.HasKey("Coins"))
            //    PlayerPrefs.SetInt("Coins", 100);

            coinAmout.text = PlayerPrefs.GetInt("Coins", 0).ToString();

            float xDiff = ((9f/16f - (float)Screen.width / (float)Screen.height) * 1000);
            positioner.GetComponent<RectTransform>().anchoredPosition = new Vector2(positioner.GetComponent<RectTransform>().anchoredPosition.x + xDiff, positioner.GetComponent<RectTransform>().anchoredPosition.y);

        }

	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void AddCoins(int amount, float animSpeed)
    {
        if (amount > 0)
            addAmount.text = "+" + amount;
        else
            addAmount.text = amount.ToString();

        addAnim["CoinCollect"].speed = animSpeed;

        addAnim.Play();
        int currentAmount = PlayerPrefs.GetInt("Coins", 0);
        currentAmount += amount;
        PlayerPrefs.SetInt("Coins", currentAmount);
        coinAmout.text = currentAmount.ToString();
    }


}
