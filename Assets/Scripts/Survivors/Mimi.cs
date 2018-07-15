using System.Collections.Specialized;
using System.Collections.Generic;
using UnityEngine;

public class Mimi : Survivor {

    private readonly string[] message = { "It's pretty fun out there. I get to use my bat and figure out how these things tick!" };

    private readonly string[] firstJohnny = { "Hello JJ. What's it hanging? " };
    private readonly string[] secondJohnny = { "Oh yes that sounds fun. Go looting with me at some time soon. ",
                                               "We will bash some heads! " };

    public new const string charName = "Mimi Necrosnyth, Dread Queen";

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

        if (QuestManager.qm.mimiActive) {
            at.SetComponents(this, this.message);
            at.StartScrolling();
        } else if (QuestManager.qm.johnnyActive) {
            Debug.Log("In Mimi's Johnny Active block");
            GameObject johnnyObject = GameObject.Find("JohnnyJacket");
            JohnnyJacket johnny = johnnyObject.GetComponent<JohnnyJacket>();

            List<AnimatedText.MessagePacket> survMsgs = new List<AnimatedText.MessagePacket> {
                new AnimatedText.MessagePacket(this, this.firstJohnny),
                new AnimatedText.MessagePacket(johnny, johnny.firstRequest),
                new AnimatedText.MessagePacket(this, this.secondJohnny)
            };

            at.SetComponents(survMsgs);
            at.StartScrolling();
        }
    }
}
