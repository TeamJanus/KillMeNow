using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour {

    public GameObject gm;
    public GameObject sm;

    void Awake() {
        if (GameManager.gm == null) {
            Instantiate(gm);
        }

        if (SoundManager.sm == null) {
            Instantiate(sm);
        }
    }

}
