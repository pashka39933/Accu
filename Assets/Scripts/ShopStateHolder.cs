using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShopStateHolder : MonoBehaviour {

    public GameObject[] characters;
    public Sprite[] charactersSprites;
    public bool[] charactersUnlocked;
    public GameObject[] shopItems;

    AudioSource audioSource;

	// Use this for initialization
	void Start () {

        //PlayerPrefs.DeleteAll();

        charactersUnlocked = StringToBoolArray(PlayerPrefs.GetString("UnlockedCharacters", "1_0_0"), '_');
        RefreshUnlockedCharacters();
        SetCurrentCharacter(PlayerPrefs.GetInt("Character", 0));

        audioSource = this.GetComponent<AudioSource>();
	}


    bool[] StringToBoolArray(string str, char sep)
    {
        string[] split = str.Split(sep);
        bool[] ret = new bool[split.Length];
        for (int i = 0; i < ret.Length; i++)
        {
            ret[i] = split[i] == "1" ? true : false;
        }
        return ret;
    }

    string BoolArrayToString(bool[] arr, char sep)
    {
        string ret = "";
        for (int i = 0; i < arr.Length; i++)
        {
            ret = ret + (arr[i] ? "1" : "0");
            if (i != arr.Length - 1)
                ret = ret + sep;
        }
        return ret;
    }

    public void SetCurrentCharacter(int characterID)
    {
        PlayerPrefs.SetInt("Character", characterID);
        GameObject.Find("Player").GetComponent<SpriteRenderer>().sprite = charactersSprites[characterID];

        for (int i = 0; i < characters.Length; i++)
        {
            if (i == characterID)
                shopItems[i].transform.FindChild("Check").gameObject.SetActive(true);
            else
                shopItems[i].transform.FindChild("Check").gameObject.SetActive(false);
        }
    }

    public void RefreshUnlockedCharacters()
    {

        for (int i = 0; i < charactersUnlocked.Length; i++)
        {
            if (charactersUnlocked[i])
            {
                shopItems[i].GetComponent<Image>().sprite = charactersSprites[i];
                Transform price = shopItems[i].transform.FindChild("Price");
                if (price)
                    price.gameObject.SetActive(false);
            }
        }
    }

    public void BuyOrSelectCharacter(int characterID)
    {
        audioSource.Play();

        if (charactersUnlocked[characterID])
            SetCurrentCharacter(characterID);
        else
        {
            Transform priceObject = shopItems[characterID].transform.FindChild("Price");

            if (priceObject)
            {
                int price = int.Parse(priceObject.GetComponent<Text>().text);
                if (price <= PlayerPrefs.GetInt("Coins", 0))
                {
                    GameObject.FindObjectOfType<CoinsController>().AddCoins(-price, 3f);
                    charactersUnlocked[characterID] = true;
                    PlayerPrefs.SetString("UnlockedCharacters", BoolArrayToString(charactersUnlocked, '_'));
                    SetCurrentCharacter(characterID);
                    RefreshUnlockedCharacters();
                }
            }
            else
            {

                StartCoroutine(OpenURLAndUnlockCharacter("https://twitter.com/jakomapoland", 1f, characterID));

            }
        }
    }

    IEnumerator OpenURLAndUnlockCharacter(string url, float sec, int charID)
    {
        Application.OpenURL("https://twitter.com/jakomapoland");

        yield return new WaitForSeconds(sec);

        charactersUnlocked[charID] = true;
        PlayerPrefs.SetString("UnlockedCharacters", BoolArrayToString(charactersUnlocked, '_'));
        SetCurrentCharacter(charID);
        RefreshUnlockedCharacters();
    }
}
