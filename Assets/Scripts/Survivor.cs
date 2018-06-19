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

    private SpriteRenderer sprite;
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
    public int loot = 1;
    public int combat = 1;
    public int build = 1;
    public int rally = 1;

    public Action action = Action.None;
    private List<Status> statuses = new List<Status>(); 

    private void Awake() {
        sprite = GetComponent<SpriteRenderer>();
        slot = gameObject.transform.parent;

        nameText.text = charName;
        baseColors = defendButton.colors;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        canvas.gameObject.SetActive(true);
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
        if (supportButton.IsInteractable()) supportButton.interactable = false;
        if (status == Status.Frightened) lootButton.interactable = false;
        if (status == Status.Hurt) defendButton.interactable = false;
        if (statuses.Count == TOTAL_STATUSES) lastChanceButton.interactable = true;
    }

    public void RemoveStatus(Status status) {
        statuses.Remove(status);
        if (status == Status.Frightened) lootButton.interactable = true;
        if (status == Status.Hurt) defendButton.interactable = true;
        if (statuses.Count == 0) supportButton.interactable = true;
        if (lastChanceButton.IsInteractable() && statuses.Count > TOTAL_STATUSES) lastChanceButton.interactable = false;
    }

    public void SetAction(Button choice) {
        DeactivateSupportMenu();

        switch (choice.name) {
            case DEFEND_BUTTON_NAME:
                this.action = Action.Build;
                break;
            case LOOT_BUTTON_NAME:
                this.action = Action.Loot;
                break;
            case SUPPORT_BUTTON_NAME:
                this.action = Action.Support;
                ActivateSupportMenu();
                break;
            case LAST_CHANCE_BUTTON_NAME:
                this.action = Action.LastChance;
                break;
            default:
                this.action = Action.None;
                break;
        }

        KeepHighlight(choice);

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
        colors.normalColor = colors.highlightedColor;
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


}

