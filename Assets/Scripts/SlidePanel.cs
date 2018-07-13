using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlidePanel : MonoBehaviour {

    public Image menuPanel;
    public Animation slideAnimation;

    private bool onScreen = false;

    public void Slide() {
        if (onScreen) {
            slideAnimation.Play("MenuPanelSlideOut");
            onScreen = false;
        } else {
            slideAnimation.Play("MenuPanelSlideIn");
            onScreen = true;
        }
    }
}
