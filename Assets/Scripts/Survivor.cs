using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Survivor : MonoBehaviour, 
    IPointerEnterHandler,
    IPointerExitHandler{

    private const int TOTAL_STATUSES = 2;

    private const string defendButtonName = "DefendButton";
    private const string lootButtonName = "LootButton";
    private const string supportButtonName = "SupportButton";
    private const string lastChanceButtonName = "LastChanceButton";

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

    private ColorBlock baseColors;

    public Text nameText;

    public enum Action { Build, Loot, Support, LastChance, None };
    public enum Status { Frightened, Hurt };

    public string charName = "Survivor";
    public string pronounSubject;
    public string pronounObject;
    public int loot = 1;
    public int combat = 1;
    public int build = 1;
    public int rally = 1;

    public Action action = Action.None;
    private List<Status> statuses = new List<Status>(); 

    private void Awake () {
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
        switch(choice.name) {
            case defendButtonName:
                this.action = Action.Build;
                break;
            case lootButtonName:
                this.action = Action.Loot;
                break;
            case supportButtonName:
                this.action = Action.Support;
                break;
            case lastChanceButtonName:
                this.action = Action.LastChance;
                break;
            default:
                this.action = Action.None;
                break;
        }

        foreach(Button block in GetComponentsInChildren<Button>()) {
            block.colors = baseColors;
        }

        ColorBlock colors = choice.colors;
        colors.normalColor = colors.highlightedColor;
        choice.colors = colors;

        GameManager.gm.CheckActions();
    }

    public void ResetAction() {
        action = Action.None;
    }


}

