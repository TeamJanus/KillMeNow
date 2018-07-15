using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour {

    public static QuestManager qm = null;
    public Button questsButton;
    public Image questPanel;
    public Image questInfoPanel;

    public Image portraitImage;
    public Text nameText;
    public Text descText;

    public SlidePanel slideScript;

    public GameObject buttonPrefab;

    private List<Survivor> questers = new List<Survivor>();

    public bool johnnyActive = false;
    public bool maddieActive = false;
    public bool mimiActive = false;

    public bool johnnyComplete = false;
    public bool maddieComplete = false;
    public bool mimiComplete = false;

    private void Awake() {
        if (qm == null) qm = this;
        else if (qm != this) Destroy(gameObject);
    }

    public void ActivateQuest(Survivor survivor) {
        survivor.DeepTalkBubbleActivate();

        // TODO: I hate this. Redo the entire survivor inheritance tree dude. Asap
        switch (survivor.charName) {
            case (JohnnyJacket.charName):
                johnnyActive = true;
                break;
            case (Maddie.charName):
                maddieActive = true;
                break;
            case (Mimi.charName):
                mimiActive = true;
                break;
        }

        GameObject tempObject = (GameObject)Instantiate(buttonPrefab);
        Button tempButton = tempObject.GetComponent<Button>();
        tempButton.transform.SetParent(questPanel.transform, false);
        tempButton.GetComponentInChildren<Text>().text = survivor.charName;
        tempButton.gameObject.name = survivor.charName + " Button";

        tempButton.onClick.AddListener(() => ShowQuestInfo(survivor));

        questers.Add(survivor);

        questsButton.interactable = true;
    }

    public void CompleteQuest(Survivor survivor) {

        GameObject button = GameObject.Find(survivor.charName + " Button");
        Button bb = button.GetComponent<Button>();
        Text bbt = bb.GetComponentInChildren<Text>();
        bbt.text += " = Complete!";

        bb.onClick.AddListener(() => ShowCompleteQuestInfo(survivor));
    }

    private void ShowQuestInfo(Survivor survivor) {

        switch (survivor.charName) {
            case JohnnyJacket.charName:
                descText.text = (survivor as JohnnyJacket).GetQuestDesc();
                break;
            case Mimi.charName:
                descText.text = (survivor as Mimi).GetQuestDesc();
                break;
            case Maddie.charName:
                descText.text = (survivor as Maddie).GetQuestDesc();
                break;
        }

        portraitImage.sprite = survivor.portrait;
        nameText.text = survivor.charName;

        slideScript.CallSlide();
    }

    private void ShowCompleteQuestInfo(Survivor survivor) {

        switch (survivor.charName) {
            case JohnnyJacket.charName:
                descText.text = (survivor as JohnnyJacket).GetQuestCompl();
                break;
            case Mimi.charName:
                descText.text = (survivor as Mimi).GetQuestCompl();
                break;
            case Maddie.charName:
                descText.text = (survivor as Maddie).GetQuestCompl();
                break;
        }

        portraitImage.sprite = survivor.portrait;
        nameText.text = survivor.charName;

        slideScript.CallSlide();
    }

    public List<Survivor> GetQuesters() {
        return questers;
    }
}
