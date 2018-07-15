using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JohnnyJacket : Survivor {

    private readonly string[] message = { "What's up friendo? I'm Johnny Jacket.",
                                          "I wonder if Mimi can teach me a thing or two about fending off those monsters outside? "};

    public readonly string[] firstRequest = { "It's hanging for sure. I think. Maybe. ",
                                              "Do you think you can show me how to survive longer on the outside? " };

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
        Canvas talkCanvas = GameManager.gm.GetTalkCanvas();
        talkCanvas.gameObject.SetActive(true);

        AnimatedText at = talkCanvas.GetComponentInChildren<AnimatedText>();
        at.SetComponents(this, this.message);
        at.StartScrolling();

        QuestManager.qm.johnnyActive = true;

        GameObject mimiObject = GameObject.Find("Mimi(Clone)");
        Mimi mimi = mimiObject.GetComponent<Mimi>();
        mimi.DeepTalkBubbleToggle();
    }

    public string GetQuestDesc() {
        return questDesc;
    }

    public string GetCharName() {
        return charName;
    }

}
