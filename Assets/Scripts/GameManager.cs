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
                Debug.Log(survivorCount);
            }
        } else {
            // Bad Stuff
            switch (Random.Range(1,2)) {
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

        System.Random rnd = new System.Random();
        IEnumerable<int> result = from value in Enumerable.Range(0, survivors.Length) orderby rnd.Next() select value;
        foreach (int i in result) {
            if (!string.Equals(survivor.charName, survivors[i].charName) && survivors[i].GetStatuses().Count > 0) {
                foreach(Survivor.Status status in survivors[i].GetStatuses()) {
                    if (survivor.rally < Random.Range(1, 100)) {
                        switch (status) {
                            case Survivor.Status.Frightened:
                                survivors[i].RemoveStatus(status);
                                output += survivor.charName + " soothes " + survivors[i].charName + "'s frayed nerves. \r\n" +
                                          survivors[i].charName + " is ready to go out again.\r\n";
                                break;
                            case Survivor.Status.Hurt:
                                survivors[i].RemoveStatus(status);
                                output += survivor.charName + " cleans and sets " + survivors[i].charName + "'s wounds. \r\n" +
                                          survivors[i].charName + " can do heavy work again.\r\n";
                                break;
                        }
                        break;
                    } else {
                        switch (status) {
                            case Survivor.Status.Frightened:
                                output += survivor.charName + " can't get through to " + survivors[i].charName + ".\r\n" +
                                          survivors[i].charName + " raves about the things outside.\r\n";
                                break;
                            case Survivor.Status.Hurt:
                                output += survivor.charName + " fumbles healing " + survivors[i].charName + "'s wounds. \r\n" +
                                          survivors[i].charName + " is no better than they were before.\r\n";
                                break;
                        }
                        break;
                    } 
                }
            } else {
                barrierCount += survivor.build / 10;
                output += survivor.charName + " doesn't see anyone in need of help.\r\n" + 
                          survivor.charName + " restored the barrier by " + survivor.build / 10 + " points.\r\n";
                break;
            }
            break;
        }

        return output;
    }

    private string EvaluateNumbers() {
        string output = string.Format("Barrier took {0} damage. {1} units of food left. {2} horrors at the door.",
                                            ZOMBIE_DAMAGE * zombieCount, foodCount, zombieCount);

        return output;
    }
}
