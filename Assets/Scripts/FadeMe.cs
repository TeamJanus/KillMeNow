using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FadeMe : MonoBehaviour {

    private MaskableGraphic element;
    private EventTrigger trigger;

    private void Awake() {
        element = GetComponent<MaskableGraphic>();

        trigger = GetComponent<EventTrigger>();
        if (trigger == null) {
            trigger = GetComponentInParent<EventTrigger>();
        }
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

    IEnumerator DoFadeIn() {
        trigger.enabled = false;

        Color alpha = element.color;
        while (alpha.a < 1f) {
            alpha.a += Time.deltaTime / 2;
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
            alpha.a -= Time.deltaTime / 2;
            element.color = alpha;
            yield return null;
        }
        if (alpha.a <= 0f) {
            gameObject.SetActive(false);
            trigger.enabled = true;
            GameManager.gm.advanceDay();
            Debug.Log("Faded out");
        }
        yield return null;
    }

}
