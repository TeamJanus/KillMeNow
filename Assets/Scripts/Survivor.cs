using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Survivor : MonoBehaviour, 
    IPointerEnterHandler,
    IPointerExitHandler{

    private const string defendButtonName = "DefendButton";
    private const string lootButtonName = "LootButton";
    private const string supportButtonName = "SupportButton";

    private SpriteRenderer sprite;
    private Transform slot;

    public Canvas canvas;
    [SerializeField]
    private Button defendButton, lootButton, supportButton;
    private Text nameText;

    public enum Action { Build, Loot, Support, None };
    public enum Status { Frightened, Hurt };

    public string charName = "Survivor";
    public int loot = 1;
    public int combat = 1;
    public int build = 1;
    public int rally = 1;

    public Action action = Action.None;
    private List<Status> statuses = new List<Status>(); 

    private void Awake () {
        sprite = GetComponent<SpriteRenderer>();
        slot = gameObject.transform.parent;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        canvas.gameObject.SetActive(true);

        if (nameText == null) nameText = GetComponentInChildren<Text>();

        nameText.text = charName;
    }

    public void OnPointerExit(PointerEventData eventData) {
        canvas.gameObject.SetActive(false);
    }

    public void AddStatus(Status status) {
        statuses.Add(status);
        if (status == Status.Frightened) lootButton.interactable = false;
        if (status == Status.Hurt) defendButton.interactable = false;
    }

    public void RemoveStatus(Status status) {
        statuses.Remove(status);
        if (status == Status.Frightened) lootButton.interactable = true;
        if (status == Status.Hurt) defendButton.interactable = true;
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
            default:
                this.action = Action.None;
                break;
        }

        GameManager.gm.CheckActions();
    }

    public void ResetAction() {
        action = Action.None;
    }


}

