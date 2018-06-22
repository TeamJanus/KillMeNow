using System.Collections;
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

    private bool newGame = false;
    private bool firstLoad = true;

    private void Awake() {
        blackPanel = GetComponent<Image>();
        texts = GetComponentsInChildren<Text>();
        trigger = GetComponent<EventTrigger>();
    }

    // Set up fade out at start of game to inform difficulty
    private void Start() {
        blackPanel.color = new Color(blackPanel.color.r, blackPanel.color.g, blackPanel.color.b, 1f);

        foreach (Text text in texts) {
            text.color = new Color(text.color.r, text.color.g, text.color.b, 1f);
        }

        gameObject.SetActive(true);
    }

    public void NewGame() {
        newGame = true;
    }

    // Adapted from Kiwasi Games's "Unity UI Fade Out" at https://youtu.be/MkoIZTFUego
    public void FadeIn() {
        gameObject.SetActive(true);
        GameManager.gm.AdvanceDay();
        StartCoroutine(DoFadeIn());
    }

    public void FadeOut() {
        if (!newGame) {
            if (firstLoad) {
                StartCoroutine(DoFadeOut());
            } else {
                GameManager.gm.UpdateGUI();
                StartCoroutine(DoFadeOut());
            }
        } else {
            GameManager.gm.ReloadLevel();
        }
    }

    IEnumerator DoFadeIn() {
        trigger.enabled = false;

        while (blackPanel.color.a < 1f) {
            if (Input.GetMouseButtonDown(0)) {
                blackPanel.color = new Color(blackPanel.color.r, blackPanel.color.g, blackPanel.color.b, 1f);

                foreach (Text text in texts) {
                    text.color = new Color(text.color.r, text.color.g, text.color.b, 1f);
                }
                yield return null;
            }

            Color alpha = blackPanel.color;
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
        trigger.enabled = false;

        int speed = FADE_SPEED;
        if (firstLoad) {
            firstLoad = false;
            speed = FIRST_FADE_SPEED;
        }

        while (blackPanel.color.a > 0f) {
            if (Input.GetMouseButtonDown(0)) {
                blackPanel.color = new Color(blackPanel.color.r, blackPanel.color.g, blackPanel.color.b, 0f);

                foreach (Text text in texts) {
                    text.color = new Color(text.color.r, text.color.g, text.color.b, 0f);
                }
                yield return null;
            }

            Color alpha = blackPanel.color;

            alpha.a -= Time.deltaTime / speed;
            blackPanel.color = alpha;

            foreach (Text text in texts) {
                Color textAlpha = text.color;
                textAlpha.a -= Time.deltaTime / speed;
                text.color = textAlpha;
            }

            yield return null;
        }
        gameObject.SetActive(false);
        trigger.enabled = true;

        yield return null;
    }

}
