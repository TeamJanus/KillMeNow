using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour {

    public static QuestManager qm = null;
    public Button questsButton;
    public Image questPanel;
    public Image questInfoPanel;

    public GameObject buttonPrefab;

    private List<Survivor> questers = new List<Survivor>();
    private List<Button> questButtons = new List<Button>();

    private void Awake() {
        if (qm == null) qm = this;
        else if (qm != this) Destroy(gameObject);
    }

    public void ActivateQuest(Survivor survivor) {

        GameObject tempObject = (GameObject)Instantiate(buttonPrefab);
        Button tempButton = tempObject.GetComponent<Button>();
        tempButton.transform.SetParent(questPanel.transform, false);
        tempButton.GetComponentInChildren<Text>().text = survivor.charName;

        tempButton.onClick.AddListener(() => ShowQuestInfo(survivor));

        questButtons.Add(tempButton);

        questers.Add(survivor);
        if (questers.Count > 0) {
            questsButton.interactable = true;
        }
    }

    public void CompleteQuest(Survivor survivor) {

        int index = questers.IndexOf(survivor);
        Destroy(questButtons[index]);
        questButtons.RemoveAt(index);

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
