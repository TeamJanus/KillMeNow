using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public static GameManager gm = null;


    public Text dayCount;
    public Text barrierCount;
    public Text foodCount;
    public Text zombieCount;
    public Image nightlyNews;

    void Awake() {
        if (gm == null) gm = this;
        else if (gm != this) Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    void Start() {
 
    }

}
