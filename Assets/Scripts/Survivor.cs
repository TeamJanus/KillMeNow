using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Survivor : MonoBehaviour, 
    IPointerEnterHandler,
    IPointerExitHandler{

    protected SpriteRenderer sprite;

    private Canvas canvas;
    private Text nameText;

    public string charName = "Survivor";
    public int loot = 1;
    public int combat = 1;
    public int build = 1;
    public int rally = 1;

    void Awake () {
        canvas = GetComponentInChildren<Canvas>();
        nameText = GetComponentInChildren<Text>();

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
}

