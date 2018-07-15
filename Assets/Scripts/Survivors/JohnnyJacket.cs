using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JohnnyJacket : Survivor {

    private readonly string[] message = { "What's up friendo? I'm Johnny Jacket.",
                                          "I wonder if Mimi can teach me a thing or two about fending off those monsters outside? "};

    public readonly string[] firstRequest = { "It's hanging for sure. I think. Maybe. ",
                                              "Do you think you can show me how to survive longer on the outside? " };

    private readonly string[] firstMaddie = { "Hey Maddie." };
    private readonly string[] secondMaddie = { "Sure I think. What do you need me to... do?" };
    private readonly string[] thirdMaddie = { "If it's really gonna be that easy, the next free night I get I'm all yours. Please don't poison me. " };

    private string questDesc = "It's crawling with crazies outside. I should talk to Mimi and see if she'll teach me how she managed to survive outside.\r\n"
                                       + "\r\nPress Mimi's quest icon.";

    private const string questCompl = "I had a great time at the junkyard with Mimi. I can swing and loot harder now!";

    public new const string charName = "Johnny Jacket";

    private Maddie maddie;

	// Use this for initialization
	void Start () {
        // Johnny thinks himself well rounded
        highest = Stats.Build;
        // TODO: This will be unsafe in future builds when we can't assume the same set of characters. Make it general
        GameObject maddieObject = GameObject.Find("Maddie");
        maddie = maddieObject.GetComponent<Maddie>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void TalkCanvasToggle() {
        Canvas talkCanvas = GameManager.gm.GetTalkCanvas();
        talkCanvas.gameObject.SetActive(true);

        AnimatedText at = talkCanvas.GetComponentInChildren<AnimatedText>();

        if (questActive && !calledUpon) {
            at.SetComponents(this, this.message);

            // TODO: Finding clones will become problematic. How are we gonna handle this?
            GameObject mimiObject = GameObject.Find("Mimi(Clone)");
            Mimi mimi = mimiObject.GetComponent<Mimi>();
            mimi.calledUpon = true;
            mimi.DeepTalkBubbleActivate();
        } else if (maddie.questActive) {
            List<AnimatedText.MessagePacket> survMsgs = new List<AnimatedText.MessagePacket> {
                new AnimatedText.MessagePacket(this, this.firstMaddie),
                new AnimatedText.MessagePacket(maddie, maddie.firstRequest),
                new AnimatedText.MessagePacket(this, this.secondMaddie),
                new AnimatedText.MessagePacket(maddie, maddie.secondRequest),
                new AnimatedText.MessagePacket(this, this.thirdMaddie),
                new AnimatedText.MessagePacket(maddie, maddie.thirdRequest)
            };

            at.SetComponents(survMsgs);

            maddie.UpdateQuestDesc();
        }


    }

    public string GetQuestDesc() {
        return questDesc;
    }

    public string GetQuestCompl() {
        return questCompl;
    }

    public void UpdateQuestDesc() {
        questDesc = "Mimi invited me out on a looting run. I'm looking forward to learning some real moves!\r\n"
                  + "\r\nSend Mimi and Johnny looting.";
    }

}
