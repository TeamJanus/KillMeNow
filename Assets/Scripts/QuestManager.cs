using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour {

    public static QuestManager qm = null;

    private bool johnnyQuest = false;

    private void Awake() {
        if (qm == null) qm = this;
        else if (qm != this) Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void ActivateJohnnyQuest() {
        johnnyQuest = true;
    }

    public void CompleteJohnnyQuest() {
        johnnyQuest = false;
    }

}
