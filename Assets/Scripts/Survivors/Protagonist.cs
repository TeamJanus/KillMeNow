using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Protagonist : Survivor {

	// Use this for initialization
	void Start () {
        // Should be about middle of the road. Values may be tweaked if this isn't the case at 50
        stats.charName = "Protagonist";
        stats.loot = 50;
        stats.combat = 50;
        stats.build = 50;
        stats.rally = 50;
    }
}
