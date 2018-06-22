using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Survivor : MonoBehaviour, 
    IPointerEnterHandler,
    IPointerExitHandler{

    private const int TOTAL_STATUSES = 2;

    private const string BASE_NAME = "Survivor";

    private const string DEFEND_BUTTON_NAME = "DefendButton";
    private const string LOOT_BUTTON_NAME = "LootButton";
    private const string SUPPORT_BUTTON_NAME = "SupportButton";
    private const string LAST_CHANCE_BUTTON_NAME = "LastChanceButton";

    private Transform slot;

    public Canvas canvas;
    [SerializeField]
    private Button defendButton; 
    [SerializeField]
    private Button lootButton;
    [SerializeField]
    private Button supportButton;
    [SerializeField]
    private Button lastChanceButton;

    [SerializeField]
    private VerticalLayoutGroup supportMenu;
    [SerializeField]
    private Button[] supportButtons = new Button[GameManager.SURVIVOR_SLOTS];
    private Survivor supportTarget;

    private ColorBlock baseColors;

    public Text nameText;

    public enum Action { Build, Loot, Support, LastChance, None };
    public enum Status { Frightened, Hurt };

    public string charName = BASE_NAME;
    public string pronounSubject;
    public string pronounObject;

    public SpriteRenderer deepTalkSign;

    public int loot = 1;
    public int combat = 1;
    public int build = 1;
    public int rally = 1;

    public Action action = Action.None;
    private List<Status> statuses = new List<Status>();

    private bool firstLoad = true;

    private void Awake() {
        slot = gameObject.transform.parent;

        nameText.text = charName;
        baseColors = defendButton.colors;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        canvas.gameObject.SetActive(true);

        if (slot.name.Equals("1") && firstLoad) {
            firstLoad = false;

            Vector2 canvasPos = canvas.transform.localPosition;
            canvas.transform.localPosition = new Vector2(canvasPos.x * -1, canvasPos.y);

            Vector2 namePos = nameText.transform.localPosition;
            nameText.transform.localPosition = new Vector2(namePos.x * -1, namePos.y);

            Vector2 suppPos = supportMenu.transform.localPosition;
            supportMenu.transform.localPosition = new Vector2(suppPos.x * -1, suppPos.y);
        }

        if (GameManager.gm.survivorCount < 2) {
            supportButton.interactable = false;
        } else if (statuses.Count == 0) {
            supportButton.interactable = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        canvas.gameObject.SetActive(false);
    }

    // Use the add and remove status methods below instead of operating on this list directly for button interaction
    public List<Status> GetStatuses() {
        return this.statuses;
    }

    public void AddStatus(Status status) {
        statuses.Add(status);
        supportButton.interactable = false;
        if (status == Status.Frightened) lootButton.interactable = false;
        if (status == Status.Hurt) defendButton.interactable = false;
        if (statuses.Count == TOTAL_STATUSES) lastChanceButton.interactable = true;
    }

    public void RemoveStatus(Status status) {
        statuses.Remove(status);
        if (status == Status.Frightened) lootButton.interactable = true;
        if (status == Status.Hurt) defendButton.interactable = true;
        if (statuses.Count == 0) supportButton.interactable = true;
        if (statuses.Count > 0) lastChanceButton.interactable = false;
    }

    public void SetAction(Button choice) {
        DeactivateSupportMenu();

        switch (choice.name) {
            case DEFEND_BUTTON_NAME:
                this.action = Action.Build;
                KeepHighlight(choice);
                break;
            case LOOT_BUTTON_NAME:
                this.action = Action.Loot;
                KeepHighlight(choice);
                break;
            case SUPPORT_BUTTON_NAME:
                this.action = Action.Support;
                ActivateSupportMenu();

                bool found = false;
                int i = 0;
                while (!found) {
                    if (supportButtons[i].IsActive()) {
                        SetSupportTarget(supportButtons[i]);
                        found = true;
                    } else {
                        i += 1;
                    }
                }

                break;
            case LAST_CHANCE_BUTTON_NAME:
                this.action = Action.LastChance;
                KeepHighlight(choice);
                break;
            default:
                this.action = Action.None;
                ResetHighlight();
                break;
        }

        GameManager.gm.CheckActions();
    }

    private void ActivateSupportMenu() {
        supportMenu.gameObject.SetActive(true);

        Survivor[] survivors = GameManager.gm.GetSurvivors();
        for (int i = 0; i < supportButtons.Length; i++) {
            Button button = supportButtons[i];
            button.gameObject.SetActive(true);
            if (i < survivors.Length) {
                button.GetComponentInChildren<Text>().text = survivors[i].charName;
                if (button.GetComponentInChildren<Text>().text.Equals(this.charName)) button.gameObject.SetActive(false);
            } else {
                button.gameObject.SetActive(false);
            }
        }
    }

    private void DeactivateSupportMenu() {
        supportMenu.gameObject.SetActive(false);
    }

    public void SetSupportTarget(Button choice) {
        Survivor[] survivors = GameManager.gm.GetSurvivors();
        int i = Array.IndexOf(supportButtons, choice);
        supportTarget = survivors[i];

        KeepHighlight(choice);
    }

    public Survivor GetSupportTarget() {
        return supportTarget;
    }

    private void KeepHighlight(Button choice) {
        ResetHighlight();

        ColorBlock colors = choice.colors;
        colors.normalColor = Color.blue;
        choice.colors = colors;
    }

    public void ResetHighlight() {
        foreach (Button block in GetComponentsInChildren<Button>()) { 
            block.colors = baseColors;
        }
    }

    public void ResetAction() {
        DeactivateSupportMenu();
        action = Action.None;
    }

    public void DeepTalkBubbleToggle() {
        deepTalkSign.gameObject.SetActive(true);
    }
}

