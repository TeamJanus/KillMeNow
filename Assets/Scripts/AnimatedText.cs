using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Adapted from Tony Morelli's video, "Unity 5.3 2D Animated Text Display," found at https://youtu.be/oWpCBGtjBm8
public class AnimatedText : MonoBehaviour {

    public Canvas parentCanvas;
    public float speed = 0.1f;
    public Text messageText;

    public Text nameText;
    public Image portrait;

    private string[] textStrings;
    private int stringIndex = 0;
    private int characterIndex = 0;

    private Survivor caller;
    private List<AnimatedText.MessagePacket> survMsgs;
    private int survMsgsIndex = 0;

    IEnumerator DisplayTimer() {
        while (characterIndex <= textStrings[stringIndex].Length) {
            yield return new WaitForSeconds(speed);
            messageText.text = textStrings[stringIndex].Substring(0, characterIndex);
            characterIndex++;
        }
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetMouseButtonUp(0)) {
            if (characterIndex < textStrings[stringIndex].Length) {
                StopCoroutine(DisplayTimer());
                characterIndex = textStrings[stringIndex].Length;
            } else if (stringIndex < textStrings.Length - 1) {
                stringIndex++;
                characterIndex = 0;
                StartCoroutine(DisplayTimer());
            } else {
                if (survMsgs != null && (survMsgsIndex < survMsgs.Count)) {
                    survMsgsIndex++;

                    Survivor next = survMsgs[survMsgsIndex].GetSurvivor();
                    nameText.text = next.charName;
                    portrait.sprite = next.portrait;
                    textStrings = survMsgs[survMsgsIndex].GetMsg();

                    stringIndex = 0;
                    characterIndex = 0;
                    StartCoroutine(DisplayTimer());
                } else {
                    stringIndex = 0;
                    characterIndex = 0;
                    messageText.text = "";
                    survMsgs = null;
                    survMsgsIndex = 0;
                    // TODO: figure this out for when two quests are active at the same 
                    caller.DeepTalkBubbleDeactivate();
                    parentCanvas.gameObject.SetActive(false);
                }
            }
        }
    }

    public void SetComponents(Survivor survivor, string[] msg) {
        nameText.text = survivor.charName;
        portrait.sprite = survivor.portrait;
        textStrings = msg;
        caller = survivor;
    }

    public void SetComponents(List<AnimatedText.MessagePacket> survMsgs) {
        caller = survMsgs[0].GetSurvivor();
        nameText.text = caller.charName;
        portrait.sprite = caller.portrait;
        textStrings = survMsgs[0].GetMsg();

        this.survMsgs = survMsgs;
    }

    public void StartScrolling() {
        StartCoroutine(DisplayTimer());
    }

    public class MessagePacket {
        Survivor surv;
        string[] msg;

        public MessagePacket (Survivor surv, string[] msg) {
            this.surv = surv;
            this.msg = msg;
        }

        public Survivor GetSurvivor() {
            return this.surv;
        }

        public string[] GetMsg() {
            return this.msg;
        }
    }
}
