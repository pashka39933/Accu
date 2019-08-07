using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BackgroundColorController : MonoBehaviour {

    public Material backgroundSkybox;

    public List<Color> bgTints = new List<Color>();
    public int currentColorIndex;

    public Color destColor;

    Color tmpColor;

    public 
	// Use this for initialization
	void Start () {

        destColor = backgroundSkybox.GetColor("_Tint");
        currentColorIndex = bgTints.IndexOf(destColor);

	}
	
	// Update is called once per frame
	void Update () {

        tmpColor = backgroundSkybox.GetColor("_Tint");
        if (tmpColor != destColor)
        {
            tmpColor = Color.Lerp(tmpColor, destColor, Time.deltaTime);
            backgroundSkybox.SetColor("_Tint", tmpColor);
        }
        
	}
}
