using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Survivor : MonoBehaviour {

    protected Stats stats;

    // Use this for initialization
    void Start () {
        stats.charName = "Survivor";
        stats.loot = 1;
        stats.combat = 1;
        stats.build = 1;
        stats.rally = 1;

        Debug.Log(stats);
    }
}

// Pure data construct for holding the stats. Should range from 1 to 100.
public struct Stats {
    public string charName;
    public int loot;
    public int combat;
    public int build;
    public int rally;
};

