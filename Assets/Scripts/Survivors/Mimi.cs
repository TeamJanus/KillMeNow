using System.Collections.Specialized;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mimi : Survivor {

    private readonly string[] message = { "It's pretty fun out there. I get to use my bat and figure out how these things tick!",
                                          "There is this one weird place I can't get into. " +
                                          "It looks like there's a big cool shiny gate in there but I always get horrible headaches when I approach it.",
                                          "Oh! The scary Maddie girl might have some meds that could help! Let's ask her ~"};

    private readonly string[] firstJohnny = { "Hello JJ. What's it hanging? " };
    private readonly string[] secondJohnny = { "Oh yes that sounds fun. Come looting with me sometime soon. ",
                                               "We will bash some heads! " };

    public readonly string[] firstRequest = { "Mads! I wanna go see a shiny gate but it has weird colors that give me headaches.",
                                              "Got any cool potions that'll stop the headaches?" };
    public readonly string[] secondRequest = { "Oh HECK YES that is totally the bad feeling color gate! Do you know what it is?" };
    public readonly string[] thirdRequest = { "YES. Let's go see the shiny colors beyond the gate!",
                                              "Don't worry Mads, stay behind me and no mosties will touch us." };

    public new const string charName = "Mimi Necrosynth, Dread Queen";

    private string questDesc = "I wanna go see a big cool looking gate but I get lots of headaches when I approach it. Let's ask the medicine girl for help!\r\n"
                             + "\r\nPress Maddie's quest icon.";

    private const string questCompl = "Wow, Maddie's weird mushroom potion did the trick! We should check out the pretty portal.\r\n" 
                                    + "Try sending all three survivors to loot the gate.";

    private JohnnyJacket johnny;

    // Use this for initialization
    void Start () {
        highest = Stats.Combat;
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

        if (questActive && !calledUpon) {
            at.SetComponents(this, this.message);

            // TODO: Finding clones will become problematic. How are we gonna handle this?
            GameObject maddieObject = GameObject.Find("Maddie");
            Maddie maddie = maddieObject.GetComponent<Maddie>();

            maddie.calledUpon = true;
            maddie.SetMimi(this);
            maddie.DeepTalkBubbleActivate();
        } else if (johnny.questActive) { 
            List<AnimatedText.MessagePacket> survMsgs = new List<AnimatedText.MessagePacket> {
                new AnimatedText.MessagePacket(this, this.firstJohnny),
                new AnimatedText.MessagePacket(johnny, johnny.firstRequest),
                new AnimatedText.MessagePacket(this, this.secondJohnny)
            };

            at.SetComponents(survMsgs);

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
        questDesc = "Maddie said she wants to see the gate! She's ready with two mushroomy potions for our headaches. Let's go soon!\r\n"
                  + "\r\nSend Mimi and Maddie out looting together.";
    }
}
