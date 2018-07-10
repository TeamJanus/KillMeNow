using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Maddie : Survivor {

    private readonly string[] message = { "Why are people so fragile? Am I supposed to be a medic and a psych around here?" };

    public new const string charName = "Maddie";

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void TalkCanvasToggle() {
        talkCanvas.gameObject.SetActive(true);

        // TODO: the children get components seems to be deterministic but I'm using magic numbers here. Can I get this not so... guessy?
        Image[] comps = talkCanvas.GetComponentsInChildren<Image>();
        // TODO: The portrait is the second image child in calling find in children on an image. What's up with that?
        comps[1].GetComponentsInChildren<Image>()[1].sprite = portrait;
        comps[3].GetComponentInChildren<Text>().text = charName;
        AnimatedText at = comps[4].GetComponentInChildren<AnimatedText>();
        at.SetTextStringAndSurvivor(this.message, this);
        at.StartScrolling();
    }
}
