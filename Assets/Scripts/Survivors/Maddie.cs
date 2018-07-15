using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Maddie : Survivor {

    private readonly string[] message = { "Why are people so fragile? Am I supposed to be a medic and a psych around here?" };

    public new const string charName = "Maddie";

    // Use this for initialization
    void Start () {
        highest = Stats.Rally;
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
    }
}
