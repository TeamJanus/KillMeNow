using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour {

    public static QuestManager qm = null;
    public Image questPanel;
    public Button questsButton;

    public GameObject buttonPrefab;

    private Button johnnyButton;

    private List<Survivor> questers = new List<Survivor>();
    private bool johnnyActive = false;

    private void Awake() {
        if (qm == null) qm = this;
        else if (qm != this) Destroy(gameObject);
    }

    public void ActivateQuest(Survivor survivor) {
        switch(survivor.charName) {
            case JohnnyJacket.charName:
                GameObject johnnyObject = (GameObject)Instantiate(buttonPrefab);
                johnnyButton = johnnyObject.GetComponent<Button>();
                johnnyButton.transform.SetParent(questPanel.transform, false);
                johnnyButton.GetComponentInChildren<Text>().text = JohnnyJacket.charName;

                johnnyButton.onClick.AddListener(() => ShowQuestInfo(survivor));

                
                break;
        }

        questers.Add(survivor);
        if (questers.Count > 0) {
            questsButton.interactable = true;
        }
    }

    public void CompleteQuest(Survivor survivor) {
        switch(survivor.charName) {
            case JohnnyJacket.charName:
                Destroy(johnnyButton.gameObject);
                break;
        }

        questers.Remove(survivor);
        if (questers.Count == 0) {
            questsButton.interactable = false;
        }
    }

    private void ShowQuestInfo(Survivor survivor) {
        switch(survivor.charName) {
            case JohnnyJacket.charName:
                Debug.Log("Clicked Johnny's Quest");
                break;
        }
    }


}
