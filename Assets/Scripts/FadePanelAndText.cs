﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FadePanelAndText : MonoBehaviour {

    private const int FIRST_FADE_SPEED = 3;
    private const int FADE_SPEED = 1;

    private Image blackPanel;
    private Text[] texts;
    private EventTrigger trigger;

    public bool newGame = false;

    private void Awake() {
        blackPanel = GetComponent<Image>();
        texts = GetComponentsInChildren<Text>();
        trigger = GetComponent<EventTrigger>();
    }

    // Fade out at start of game to inform difficulty
    private void Start() {
        FirstFadeOut();
    }

    // Adapted from Kiwasi Games's "Unity UI Fade Out" at https://youtu.be/MkoIZTFUego
    public void FadeIn() {
        gameObject.SetActive(true);
        GameManager.gm.AdvanceDay();
        StartCoroutine(DoFadeIn());
    }

    public void FadeOut() {
        if (!newGame) {
            GameManager.gm.UpdateGUI();
            StartCoroutine(DoFadeOut());
            trigger.enabled = false;
        } else {
            GameManager.gm.ReloadLevel();
        }
    }

    private void FirstFadeOut() {
        blackPanel.color = new Color(blackPanel.color.r, blackPanel.color.g, blackPanel.color.b, 1f);

        // This is the topmost text on the panel
        texts[0].color = new Color(texts[0].color.r, texts[0].color.g, texts[0].color.b, 1f);

        gameObject.SetActive(true);
        StartCoroutine(DoFirstFadeOut());
    }

    IEnumerator DoFadeIn() {
        trigger.enabled = false;

        Color alpha = blackPanel.color;
        while (alpha.a < 1f) {
            alpha.a += Time.deltaTime / FADE_SPEED;
            blackPanel.color = alpha;

            foreach (Text text in texts) {
                Color textAlpha = text.color;
                textAlpha.a += Time.deltaTime / FADE_SPEED;
                text.color = textAlpha;
            }

            yield return null;
        }
        trigger.enabled = true;

        yield return null;
    }

    IEnumerator DoFadeOut() { 
        Color alpha = blackPanel.color;
        while (alpha.a > 0f) {
            alpha.a -= Time.deltaTime / FADE_SPEED;
            blackPanel.color = alpha;

            foreach (Text text in texts) {
                Color textAlpha = text.color;
                textAlpha.a -= Time.deltaTime / FADE_SPEED;
                text.color = textAlpha;
            }

            yield return null;
        }
        gameObject.SetActive(false);

        yield return null;
    }

    IEnumerator DoFirstFadeOut() {
        trigger.enabled = false;

        Color alpha = blackPanel.color;
        while (alpha.a > 0f) {
            alpha.a -= Time.deltaTime / FIRST_FADE_SPEED;
            blackPanel.color = alpha;

            Color textAlpha = texts[0].color;
            textAlpha.a -= Time.deltaTime / FIRST_FADE_SPEED;
            texts[0].color = textAlpha;

            yield return null;
        }
        gameObject.SetActive(false);
        trigger.enabled = true;

        yield return null;
    }

}
