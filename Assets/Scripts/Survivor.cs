using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Survivor : MonoBehaviour, 
    IPointerEnterHandler,
    IPointerExitHandler{

    private const string defendButton = "DefendButton";
    private const string lootButton = "LootButton";
    private const string supportButton = "SupportButton";

    protected SpriteRenderer sprite;

    private Canvas canvas;
    private Text nameText;

    public enum Action { Build, Loot, Support, None };

    public string charName = "Survivor";
    public int loot = 1;
    public int combat = 1;
    public int build = 1;
    public int rally = 1;

    public Action action = Action.None;

    void Awake () {
        canvas = GetComponentInChildren<Canvas>();
        nameText = GetComponentInChildren<Text>();

        nameText.text = charName;

        canvas.enabled = false;
    }

    void Start() {
        
    }

    void Update() {
    }

    public void OnPointerEnter(PointerEventData eventData) {
        canvas.enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData) {
        canvas.enabled = false;
    }

    public void SetAction(Button choice) {
        switch(choice.name) {
            case defendButton:
                this.action = Action.Build;
                break;
            case lootButton:
                this.action = Action.Loot;
                break;
            case supportButton:
                this.action = Action.Support;
                break;
            default:
                this.action = Action.None;
                break;
        }
    }


}

