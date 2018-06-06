using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    public static SoundManager sm = null;

    private void Awake() {
        if (sm == null) sm = this;
        else if (sm != this) Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }
}
