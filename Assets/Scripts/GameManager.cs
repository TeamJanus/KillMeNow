using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class GameManager : MonoBehaviour {

    private const int ZOMBIE_DAMAGE = 5;
    private const int HUNGER_SCORE = 5;
    private const int SURVIVOR_SLOTS = 3;

    public static GameManager gm = null;

    private GameObject survivorSlots;
    public Transform[] slots = new Transform[SURVIVOR_SLOTS];
    private Survivor[] survivors = new Survivor[SURVIVOR_SLOTS];
    public List<Survivor> survivorsToBeFound = new List<Survivor>();

    public Button nextDayButton;

    public Text dayText;
    public Text barrierText;
    public Text foodText;
    public Text zombieText;

    public FadePanelAndText nightlyNews;
    public Text dayReport;
    public Text actionReport;
    public Text numbersReport;

    private int dayCount = 1;
    private int barrierCount = 100;
    private int foodCount = 100;
    private int zombieCount = 1;
    private int survivorCount = 2;

    private int daysLeft = 10;
    private string[] neutralDescriptors = new string[] { "The banging at the door grows louder tonight.",
                                                         "The survivors grab a few hours of uneasy sleep.",
                                                         "Night watch catches glimpses of strange shapes in the fading light" };

    private void Awake() {
        if (gm == null) gm = this;
        else if (gm != this) Destroy(gameObject);

        // We don't need to persist the gameManager if we only ever have one scene or don't need to move data between scenes we do create in in. Experiment
        //DontDestroyOnLoad(gameObject);

        survivorSlots = GameObject.Find("Survivor Slots");
        survivors = survivorSlots.GetComponentsInChildren<Survivor>();
        nextDayButton.interactable = false;
    }

    private void Start() {
        UpdateGUI();
        nextDayButton.interactable = false;
    }

    private void SetSurvivor(Survivor survivor, int slotNum) {
        survivors[slotNum - 1] = survivor;
    }

    public void CheckActions() {
        int howManyActions = 0;

        foreach (Survivor survivor in survivors) {
            if (survivor.action != Survivor.Action.None) howManyActions += 1; 
        }

        if (howManyActions == survivorCount) nextDayButton.interactable = true;
        else nextDayButton.interactable = false;
    }

    private void ResetActions() {
        foreach (Survivor survivor in survivors) survivor.ResetAction();
        nextDayButton.interactable = false;
    }

    public void UpdateGUI() {
        dayText.text = dayCount.ToString();
        barrierText.text = barrierCount.ToString();
        foodText.text = foodCount.ToString();
        zombieText.text = zombieCount.ToString();
    }

    public void AdvanceDay() {
        dayCount += 1;
        barrierCount -= ZOMBIE_DAMAGE * zombieCount;
        foodCount -= HUNGER_SCORE * survivorCount;
        zombieCount += 1;

        if (barrierCount <= 0 || foodCount <= 0) LoseGame();
        else if (dayCount >= 10) WinGame();
        else ContinueGame();
    }

    private void ContinueGame() {
        daysLeft -= 1;
        dayReport.text = string.Format("{0} days left until help arrives.", daysLeft);

        actionReport.text = EvaluateAction();

        numbersReport.text = EvaluateNumbers();

        ResetActions();
    }

    private void WinGame() {
        dayReport.text = "The sounds of gunfire finally reach your door and the pounding stops." + 
                         "\r\nYou join up with the survivor army and live to fight another day.";
        actionReport.text = "Click to play again.";
        numbersReport.enabled = false;

        nightlyNews.NewGame();
    }

    private void LoseGame() {
        dayReport.text = "The door bursts open and stumbling shapes outside pour in." +
                         "\r\nThe survivors are too weak to protest as they are torn apart.";
        actionReport.text = "Click to play again.";
        numbersReport.enabled = false;

        nightlyNews.NewGame();
    }

    public void ReloadLevel() {
        SceneManager.LoadScene(0);
    }

    private string EvaluateAction() {
        string output = "";

        foreach (Survivor survivor in survivors) {
            switch (survivor.action) {
                case Survivor.Action.Build:
                    barrierCount += survivor.build / 10;
                    output += survivor.charName + " restored the barrier by " + survivor.build / 10 + " points.\r\n";
                    break;
                case Survivor.Action.Loot:
                    output += EvaluateLooting(survivor);
                    break;
                case Survivor.Action.Support:
                    output += EvaluateSupport(survivor);
                    break;
                case Survivor.Action.LastChance:
                    output += EvaluateLastChance(survivor);
                    break;
                default:
                    Debug.LogError("Survivor " + survivor.charName + " in slot " + System.Array.IndexOf(survivors, survivor) + "doesn't have an action selected.");
                    break;
            }
        }

        output += neutralDescriptors[Random.Range(0, neutralDescriptors.Length)];
        return output;
    }

    private string EvaluateLooting(Survivor survivor) {
        string output = "";
        if (Random.Range(1, 100) < survivor.loot) {
            // Good Stuff
            int foodFound = Random.Range(3, 8);
            foodCount += foodFound;
            output += survivor.charName + " braved the outside world and found " + foodFound + " units of food.\r\n";

            // TODO: Add more survivors to the list
            // TODO: Add ability to select slot to add survivor to
            if (survivorsToBeFound.Count > 0 && Random.Range(1, 100) < survivor.loot / 2) { 
                System.Random rnd = new System.Random();
                int r = survivorsToBeFound.Count;
                Survivor addition = survivorsToBeFound[rnd.Next(r)];
                survivorsToBeFound.Remove(addition);

                output += survivor.charName + " stumbles upon someone wielding a spiked bat with ease.\r\n" +
                          "In between giant swings she introduces herself as Mimi Necrosynth, Dread Queen.\r\n" +
                          "Mimi accompanies " + survivor.charName + " back to the library.\r\n";

                Instantiate(addition, slots[1]);
                survivors = survivorSlots.GetComponentsInChildren<Survivor>();
                survivorCount = survivors.Length;
            }
        } else {
            // Bad Stuff
            switch (Random.Range(1,3)) {
                case 1:
                    survivor.AddStatus(Survivor.Status.Frightened);
                    output += survivor.charName + " saw something beyond comprehension. \r\nThey return to the library jumping at every bump and screech.\r\n";
                    break;
                case 2:
                    output += survivor.charName + " gets clipped by something sharp. \r\nThey return to the library bleeding and weak.\r\n";
                    survivor.AddStatus(Survivor.Status.Hurt);
                    break;
            }
        }
        return output;
    }

    private string EvaluateSupport(Survivor survivor) {
        string output = "";

        //System.Random rnd = new System.Random();
        // TODO: Figure out how to make this loop randomly and using awful bools for breaking loops
        List<Survivor> evalSurv = survivors.ToList();
        evalSurv.Remove(survivor);
        bool checkSurvivor = false;
        for (int i = 0; i < evalSurv.Count; i++) {
            if (!checkSurvivor) {
                if (evalSurv[i].GetStatuses().Count > 0) {
                    checkSurvivor = true;
                    if (survivor.rally < Random.Range(1, 100)) {
                        bool checkStatus = false;
                        foreach (Survivor.Status status in evalSurv[i].GetStatuses()) {
                            if (!checkStatus) {
                                switch (status) {
                                    case Survivor.Status.Frightened:
                                        evalSurv[i].RemoveStatus(status);
                                        output += survivor.charName + " soothes " + evalSurv[i].charName + "'s frayed nerves. \r\n" +
                                                    evalSurv[i].charName + " is ready to go out again.\r\n";
                                        checkStatus = true;
                                        break;
                                    case Survivor.Status.Hurt:
                                        evalSurv[i].RemoveStatus(status);
                                        output += survivor.charName + " cleans and sets " + evalSurv[i].charName + "'s wounds. \r\n" +
                                                    evalSurv[i].charName + " can do heavy work again.\r\n";
                                        checkStatus = true;
                                        break;
                                }
                            }
                        }
                    } else {
                        output += survivor.charName + " can't help " + evalSurv[i].charName + "'s wounds. \r\n" +
                                    evalSurv[i].charName + " is no better than they were before.\r\n";
                    }
                } else {
                    checkSurvivor = true;
                    barrierCount += survivor.build / 10;
                    output += survivor.charName + " doesn't see anyone in need of help.\r\n" +
                                survivor.charName + " restored the barrier by " + survivor.build / 10 + " points.\r\n";
                }
            }
        } 
        return output;
    }

    private string EvaluateLastChance(Survivor survivor) {
        string output = "";

        if (Random.Range(1, 100) > 50) {
            if (survivor.GetStatuses().Contains(Survivor.Status.Frightened)) {
                survivor.GetStatuses().Remove(Survivor.Status.Frightened);
            } else {
                survivor.GetStatuses().Remove(Survivor.Status.Hurt);
            }
            output += survivor.charName + " reaches into the depths of their spirit and pulls through their wounds.\r\n";
        } else {
            output += survivor.charName + " is too scared and hurt to get up. They remain incapacitated.\r\n";
        }

        return output;
    }

    private string EvaluateNumbers() {
        string output = string.Format("Barrier took {0} damage. {1} units of food left. {2} horrors at the door.",
                                            ZOMBIE_DAMAGE * zombieCount, foodCount, zombieCount);

        return output;
    }
}
