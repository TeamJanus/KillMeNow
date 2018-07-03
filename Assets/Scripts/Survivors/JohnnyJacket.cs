using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JohnnyJacket : Survivor {

    private const string message = "What's up friendo? I'm Johnny Jacket";

    public new const string charName = "Johnny Jacket";

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public new void TalkCanvasToggle() {
        talkCanvas.gameObject.SetActive(true);

        // TODO: the children get components seems to be deterministic but I'm using magic numbers here. Can I get this not so... guessy?
        Image[] comps = talkCanvas.GetComponentsInChildren<Image>();
        comps[1].GetComponentInChildren<Image>().sprite = portrait;
        comps[3].GetComponentInChildren<Text>().text = charName;
        comps[4].GetComponentInChildren<Text>().text = message;
    }

    public void TriggerQuest() {
        this.DeepTalkBubbleToggle();
    }
}
