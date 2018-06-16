using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FadeMe : MonoBehaviour {

    private const int FIRST_FADE_SPEED = 3;
    private const int FADE_SPEED = 1;

    private MaskableGraphic element;
    private EventTrigger trigger;

    private void Awake() {
        element = GetComponent<MaskableGraphic>();

        trigger = GetComponent<EventTrigger>();
        if (trigger == null) {
            trigger = GetComponentInParent<EventTrigger>();
        }
    }

    // Fade out at start of game to inform difficulty
    private void Start() {
        FirstFadeOut();
    }

    // Adapted from Kiwasi Games's "Unity UI Fade Out" at https://youtu.be/MkoIZTFUego
    public void FadeIn() {
        gameObject.SetActive(true);
        StartCoroutine(DoFadeIn());
    }

    public void FadeOut() {
        StartCoroutine(DoFadeOut());
        trigger.enabled = false;
    }

    private void FirstFadeOut() {
        Debug.Log("Starting Fade Me");
        element.color = new Color(element.color.r, element.color.g, element.color.b, 1f);
        gameObject.SetActive(true);
        StartCoroutine(DoFirstFadeOut());
        trigger.enabled = false;
    }

    IEnumerator DoFadeIn() {
        trigger.enabled = false;

        Color alpha = element.color;
        while (alpha.a < 1f) {
            alpha.a += Time.deltaTime / FADE_SPEED;
            element.color = alpha;
            yield return null;
        }
        if (alpha.a >= 1f) {
            trigger.enabled = true;
        }
        yield return null;
    }

    IEnumerator DoFadeOut() {
        Color alpha = element.color;
        while (alpha.a > 0f) {
            alpha.a -= Time.deltaTime / FADE_SPEED;
            element.color = alpha;
            yield return null;
        }
        if (alpha.a <= 0f) {
            gameObject.SetActive(false);
            trigger.enabled = true;
            GameManager.gm.advanceDay();
        }
        yield return null;
    }

    IEnumerator DoFirstFadeOut() {
        Color alpha = element.color;
        while (alpha.a > 0f) {
            alpha.a -= Time.deltaTime / FIRST_FADE_SPEED;
            element.color = alpha;
            yield return null;
        }
        if (alpha.a <= 0f) {
            gameObject.SetActive(false);
            trigger.enabled = true;
        }
        yield return null;
    }

}
