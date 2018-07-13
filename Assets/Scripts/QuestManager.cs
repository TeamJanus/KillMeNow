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

    private void Awake() {
        if (qm == null) qm = this;
        else if (qm != this) Destroy(gameObject);
    }

    public void ActivateQuest(Survivor survivor) {

        GameObject tempObject = (GameObject)Instantiate(buttonPrefab);
        Button tempButton = tempObject.GetComponent<Button>();
        tempButton.transform.SetParent(questPanel.transform, false);
        tempButton.GetComponentInChildren<Text>().text = survivor.charName;
        tempButton.gameObject.name = survivor.charName + " Button";

        tempButton.onClick.AddListener(() => ShowQuestInfo(survivor));

        questers.Add(survivor);

        if (questers.Count > 0) {
            questsButton.interactable = true;
        }
    }

    public void CompleteQuest(Survivor survivor) {

        GameObject button = GameObject.Find(survivor.charName + " Button");
        Destroy(button);

        questers.Remove(survivor);
        if (questers.Count == 0) {
            questsButton.interactable = false;
        }
    }

    private void ShowQuestInfo(Survivor survivor) {
        Image[] images = questInfoPanel.GetComponentsInChildren<Image>();
        Text[] words = questInfoPanel.GetComponentsInChildren<Text>();

        switch (survivor.charName) {
            case JohnnyJacket.charName:
                JohnnyJacket johnny = survivor as JohnnyJacket;
                words[1].text = johnny.GetQuestDesc();
                break;
        }

        images[1].sprite = survivor.portrait;
        words[0].text = survivor.charName;

        SlidePanel slideScript = questInfoPanel.GetComponent<SlidePanel>();
        slideScript.CallSlide();
    }


}
