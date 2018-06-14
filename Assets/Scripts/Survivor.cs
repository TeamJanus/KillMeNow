using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Survivor : MonoBehaviour, 
    IPointerEnterHandler,
    IPointerExitHandler{

    protected SpriteRenderer sprite;

    public Canvas canvas;
    public Text nameText;

    public string charName = "Survivor";
    public int loot = 1;
    public int combat = 1;
    public int build = 1;
    public int rally = 1;

    void Awake () {
        nameText.text = this.charName;
        nameText.enabled = false;
    }

    void Start() {
        
    }

    void Update() {

    }

    public void OnPointerEnter(PointerEventData eventData) {
        nameText.enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData) {
        nameText.enabled = false;
    }
}

