using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlidePanel : MonoBehaviour {

    public Image panel;
    public Animation slideAnimation;

    private bool onScreen = false;
    private bool working = false;

    private void Update() {
        if (onScreen && Input.GetMouseButtonUp(0)) {
            if (!RectTransformUtility.RectangleContainsScreenPoint(panel.GetComponent<RectTransform>(),
                                                                  Input.mousePosition, null)) {
                StartCoroutine(Slide());
            }
        }
    }

    public void CallSlide() {
        StartCoroutine(Slide());
    }

    IEnumerator Slide() {
        if (!working) {
            if (onScreen) {
                working = true;
                slideAnimation.Play("PanelSlideOut");
                onScreen = false;
                yield return new WaitForSeconds(slideAnimation.GetClip("PanelSlideOut").length);
                working = false;
            } else {
                working = true;
                slideAnimation.Play("PanelSlideIn");
                onScreen = true;
                yield return new WaitForSeconds(slideAnimation.GetClip("PanelSlideIn").length);
                working = false;
            }
        }
    }
}
