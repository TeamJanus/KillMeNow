using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JohnnyJacket : Survivor {

    private readonly string[] message = { "What's up friendo? I'm Johnny Jacket.",
                                          "I wonder if Mimi can teach me a thing or two about fending off those monsters outside? "};

    private const string questDesc = "It's crawling with crazies outside. I should talk to Mimi and see if she'll teach me how she managed to survive outside.\r\n"
                                       + "\r\nPress Mimi's quest icon.";

    public new const string charName = "Johnny Jacket";

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

    public string GetQuestDesc() {
        return questDesc;
    }

}
