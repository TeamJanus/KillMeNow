using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreditsPanel : MonoBehaviour {

    public Image panel;
    public Animation slideAnimation;

    private bool onScreen = false;
    private bool working = false;

    private void Update() {
        if (onScreen && Input.GetMouseButtonUp(0)) {
            StartCoroutine(Slide());
        }
    }

    public void CallSlide() {
        StartCoroutine(Slide());
    }

    IEnumerator Slide() {
        if (!working) {
            if (onScreen) {
                working = true;
                slideAnimation.Play("CreditsSlideOut");
                onScreen = false;
                yield return new WaitForSeconds(slideAnimation.GetClip("CreditsSlideOut").length);
                working = false;
            } else {
                working = true;
                slideAnimation.Play("CreditsSlideIn");
                onScreen = true;
                yield return new WaitForSeconds(slideAnimation.GetClip("CreditsSlideIn").length);
                working = false;
            }
        }
    }
}
