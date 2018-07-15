using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Maddie : Survivor {

    private readonly string[] message = { "Why are people so fragile? Am I supposed to be a medic and a psych around here?",
                                          "Whatever. Not much I can do about it except get better at it. ",
                                          "Oh, I could ask Johnny for some help in my research." };

    public readonly string[] firstRequest = { "Hey Johnny, I've hit a bit of a plateau in my research. Think you can help me out a bit? " };
    public readonly string[] secondRequest = { "Nothing much! Just hang around and try to take it easy for a night. Try a sedative. Safe stuff, nothing big. " };
    public readonly string[] thirdRequest = { "Perfect! This is going to be great. " };

    public new const string charName = "Maddie";

    private int supportQuestCount = 0;

    private string questDesc = "Our loot runners keep getting hurt out there. Do you think Johnny would be willing to help me study some new medical procedures?\r\n"
                             + "\r\nPress Johnny's quest icon.";

    private const string questCompl = "I've learned so much! Thanks Johnny, you're a real boy. ";

    private JohnnyJacket johnny;

    // Use this for initialization
    void Start () {
        highest = Stats.Rally;
        // TODO: This will be unsafe in future builds when we can't assume the same set of characters. Make it general
        GameObject johnnyObject = GameObject.Find("JohnnyJacket");
        johnny = johnnyObject.GetComponent<JohnnyJacket>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void TalkCanvasToggle() {
        Canvas talkCanvas = GameManager.gm.GetTalkCanvas();
        talkCanvas.gameObject.SetActive(true);

        AnimatedText at = talkCanvas.GetComponentInChildren<AnimatedText>();
        at.SetComponents(this, this.message);

        johnny.calledUpon = true;
        johnny.DeepTalkBubbleActivate();
    }

    public int GetQuestCount() {
        return supportQuestCount;
    }

    public void IncreaseQuestCount() {
        supportQuestCount += 1;
    }

    public string GetQuestDesc() {
        return questDesc;
    }

    public string GetQuestCompl() {
        return questCompl;
    }

    public void UpdateQuestDesc() {
        questDesc = "Johnny's willing to help me study some new techniques. I can't wait to get started!\r\n"
                  + "\r\nHave Mimi support Johnny while Johnny sits still and does nothing.";
    }
}
