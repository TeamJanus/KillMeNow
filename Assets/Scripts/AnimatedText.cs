using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Adapted from Tony Morelli's video, "Unity 5.3 2D Animated Text Display," found at https://youtu.be/oWpCBGtjBm8
public class AnimatedText : MonoBehaviour {

    public Text text;
    public float speed = 0.1f;

    private string textString;
    private int characterIndex = 0;

	// Use this for initialization
	void Start () {
        StartCoroutine(DisplayTimer());
	}

    IEnumerator DisplayTimer() {
        while (characterIndex <= textString.Length) {
            yield return new WaitForSeconds(speed);
            text.text = textString.Substring(0, characterIndex);
            characterIndex++;
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonUp(0)) {
            if (characterIndex < textString.Length) {
                characterIndex = textString.Length;
            } else {

            }
        }
	}

    public void SetTextString(string msg) {
        textString = msg;
    }
}
