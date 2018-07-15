using System.Collections.Specialized;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mimi : Survivor {

    private readonly string[] message = { "It's pretty fun out there. I get to use my bat and figure out how these things tick!" };

    private readonly string[] firstJohnny = { "Hello JJ. What's it hanging? " };
    private readonly string[] secondJohnny = { "Oh yes that sounds fun. Come looting with me sometime soon. ",
                                               "We will bash some heads! " };

    public new const string charName = "Mimi Necrosnyth, Dread Queen";

    private string questDesc = "Our loot runners keep getting hurt out there. Do you think Johnny would be willing to help me study some new medical procedures?\r\n"
                             + "\r\nPress Johnny's quest icon.";

    private const string questCompl = "I've learned so much! Thanks Johnny, you're a real boy. ";

    // Use this for initialization
    void Start () {
        highest = Stats.Combat;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void TalkCanvasToggle() {
        Canvas talkCanvas = GameManager.gm.GetTalkCanvas();
        talkCanvas.gameObject.SetActive(true);

        AnimatedText at = talkCanvas.GetComponentInChildren<AnimatedText>();

        if (QuestManager.qm.mimiActive) {
            at.SetComponents(this, this.message);
            at.StartScrolling();
        } else if (QuestManager.qm.johnnyActive) {
            GameObject johnnyObject = GameObject.Find("JohnnyJacket");
            JohnnyJacket johnny = johnnyObject.GetComponent<JohnnyJacket>();

            List<AnimatedText.MessagePacket> survMsgs = new List<AnimatedText.MessagePacket> {
                new AnimatedText.MessagePacket(this, this.firstJohnny),
                new AnimatedText.MessagePacket(johnny, johnny.firstRequest),
                new AnimatedText.MessagePacket(this, this.secondJohnny)
            };

            at.SetComponents(survMsgs);
            at.StartScrolling();

            johnny.UpdateQuestDesc();
        }
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
