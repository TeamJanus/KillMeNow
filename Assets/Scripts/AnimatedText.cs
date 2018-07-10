using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Adapted from Tony Morelli's video, "Unity 5.3 2D Animated Text Display," found at https://youtu.be/oWpCBGtjBm8
public class AnimatedText : MonoBehaviour {

    public Canvas parentCanvas;

    public Text text;
    public float speed = 0.1f;

    private string[] textStrings;
    private int stringIndex = 0;
    private int characterIndex = 0;

    private Survivor caller;

    private void Start() {
        StartCoroutine(DisplayTimer());
    }

    IEnumerator DisplayTimer() {
        while (characterIndex < textStrings[stringIndex].Length) {
            yield return new WaitForSeconds(speed);
            text.text = textStrings[stringIndex].Substring(0, characterIndex);
            characterIndex++;
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonUp(0)) {
            if (characterIndex < textStrings[stringIndex].Length) {
                characterIndex = textStrings[stringIndex].Length;
            } else if (stringIndex < textStrings.Length - 1) {
                stringIndex++;
                characterIndex = 0;
                StartCoroutine(DisplayTimer());
            } else {
                stringIndex = 0;
                characterIndex = 0;
                text.text = "";
                caller.DeepTalkBubbleToggle();
                parentCanvas.gameObject.SetActive(false);
            }
        }
	}

    public void SetTextStringAndSurvivor(string[] msg, Survivor survivor) {
        textStrings = msg;
        caller = survivor;
    }

    public void StartScrolling() {
        StartCoroutine(DisplayTimer());
    }
}
