using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    private const int  ZOMBIE_DAMAGE = 5;
    private const int HUNGER_SCORE = 1;
    public const int SURVIVOR_SLOTS = 3;

    public static GameManager gm = null;

    private GameObject survivorSlots;
    public Transform[] slots = new Transform[SURVIVOR_SLOTS];
    private Survivor[] survivors = new Survivor[SURVIVOR_SLOTS];
    public List<Survivor> survivorsToBeFound = new List<Survivor>();

    public Button nextDayButton;
    public Button storiesButton;

    public Text dayText;
    public Text barrierText;
    public Text foodText;
    public Text zombieText;

    public FadePanelAndText nightlyNews;
    public Text dayReport;
    public Text actionReport;
    public Text numbersReport;

    private List<Survivor.Status> allStatuses = new List<Survivor.Status>();

    private int dayCount = 1;
    private int barrierCount = 100;
    private int foodCount = 8;
    private int zombieCount = 1;
    public int survivorCount = 2;

    private int howManyActions = 0;

    private int daysLeft = 10;
    private string[] neutralDescriptors = new string[] { "The banging at the door grows louder tonight.",
                                                         "The survivors grab a few hours of uneasy sleep.",
                                                         "Night watch catches glimpses of strange shapes in the fading light" };

    private bool gameOver = false;

    private void Awake() {
        if (gm == null) gm = this;
        else if (gm != this) Destroy(gameObject);

        // We don't need to persist the gameManager if we only ever have one scene or don't need to move data between scenes we do create in in. Experiment
        //DontDestroyOnLoad(gameObject);

        survivorSlots = GameObject.Find("Survivor Slots");
        survivors = survivorSlots.GetComponentsInChildren<Survivor>();
        nextDayButton.interactable = false;

        // I'm doing this manually since I'm not sure yet how otherwise
        // TODO: Determine viability of not doing this by hand
        allStatuses.Add(Survivor.Status.Frightened);
        allStatuses.Add(Survivor.Status.Hurt);
    }

    private void Start() {
        UpdateGUI();
        nextDayButton.interactable = false;
    }

    public Survivor[] GetSurvivors() {
        return survivors;
    }

    private void SetSurvivor(Survivor survivor, int slotNum) {
        survivors[slotNum - 1] = survivor;
    }

    public void CheckActions() {
        howManyActions += 1;

        if (howManyActions >= survivorCount) nextDayButton.interactable = true;
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

        int frightCount = 0;
        // TODO: figure out if this awful active not active pattern can be removed
        // This resets the highlighting on all the buttons in each survivor
        foreach (Survivor survivor in survivors) {
            survivor.canvas.gameObject.SetActive(true);
            survivor.ResetHighlight();
            survivor.canvas.gameObject.SetActive(false);

            if (survivor.GetStatuses().Count == 1 && survivor.GetStatuses().Contains(Survivor.Status.Frightened)) frightCount += 1;
        }

        if (survivors.Length > 1 && frightCount == survivorCount) storiesButton.gameObject.SetActive(true);

        howManyActions = 0;
    }

    public void AdvanceDay() {
        dayCount += 1;
        barrierCount -= ZOMBIE_DAMAGE * zombieCount;
        foodCount -= HUNGER_SCORE * survivorCount;
        zombieCount += 1;

        if (barrierCount <= 0) LoseGame();
        else if (dayCount >= 10) WinGame();
        else ContinueGame();

    }

    private void ContinueGame() {
        daysLeft -= 1;
        dayReport.text = string.Format("{0} days left until help arrives.", daysLeft);

        actionReport.text = EvaluateAction();

        if (!gameOver) {
            numbersReport.text = EvaluateNumbers();

            ResetActions();
        }
    }

    private void WinGame() {
        dayReport.text = "The sounds of gunfire finally reach your door and the pounding stops.\r\n" + 
                         "You join up with the survivor army and live to fight another day.";
        actionReport.text = "Click to play again.";
        numbersReport.enabled = false;

        nightlyNews.NewGame();
    }

    private void LoseGame() {
        gameOver = true;

        if (barrierCount <= 0) {
            dayReport.text = "The door bursts open and the stumbling shapes outside pour in.\r\n" +
                             "The survivors are too weak to protest as they are torn apart.";
        } else if (foodCount < 0) {
            dayReport.text = "The food reserves have run too low. The survivors are too weak to go on.\r\n" +
                             "The monsters outside find slim pickings when they burst through the barricade.";
        }
        actionReport.enabled = false;
        numbersReport.text = "Click to play again.";

        nightlyNews.NewGame();
    }

    public void ReloadLevel() {
        SceneManager.LoadScene(0);
    }

    // Keep in mind that all random comparisons should be RandomValue <= SurvivorStat
    private string EvaluateAction() {
        string output = "";

        foreach (Survivor survivor in survivors) {
            switch (survivor.action) {
                case Survivor.Action.Build:
                    output += EvaluateDefending(survivor);
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
                case Survivor.Action.None:
                    output += survivor.charName + " curls up and bides " + survivor.pronounObject.ToLower() + " time, dreaming of better times.\r\n";
                    break;
                default:
                    Debug.LogError("Survivor " + survivor.charName + " in slot " + System.Array.IndexOf(survivors, survivor) + " doesn't have an action selected.");
                    break;
            }
        }

        if (foodCount < 0) LoseGame();

        output += "\r\n" + neutralDescriptors[Random.Range(0, neutralDescriptors.Length)];
        return output;
    }

    private string EvaluateDefending(Survivor survivor) {
        string output = "";

        if (Random.Range(1, 100) <= survivor.combat) {
            if (zombieCount > 0) {
                zombieCount -= 1;
                output += survivor.charName + " manages to crush a monster's skull with a hammer through a hole in the barrier. ";
            }
        }

        int buildPoints = GetBuildPoints(survivor.build);

        if (buildPoints > 0) {
            barrierCount += buildPoints;
            output += survivor.charName + " restores the barrier by " + buildPoints + " points.\r\n";
        } else {
            output += "The barrier is as sturdy as it's going to get. " + survivor.charName + " looks outside nervously.\r\n";
        }

        return output;
    }

    private int GetBuildPoints(int buildStat) {
        int buildPoints = barrierCount + buildStat / 10;
        if (buildPoints > 100) {
            buildPoints = 100 - barrierCount;
            return buildPoints;
        }

        return buildStat / 10;
    }

    private string EvaluateLooting(Survivor survivor) {
        string output = "";
        if (Random.Range(1, 100) <= survivor.loot) {
            // Good Stuff
            int foodFound = Random.Range(1, 3);
            foodCount += foodFound;
            output += survivor.charName + " braves the outside world and finds " + foodFound + " ration's worth of food.\r\n";

            // TODO: Add more survivors to the list
            // TODO: Add ability to select slot to add survivor to
            if (survivorsToBeFound.Count > 0 && Random.Range(1, 100) <= survivor.loot / 2) {
                int index = Random.Range(0, survivorsToBeFound.Count);
                Survivor addition = survivorsToBeFound[index];
                survivorsToBeFound.Remove(addition);

                output += "\r\n" + survivor.charName + " stumbles upon someone wielding a spiked bat with ease. " +
                          "In between giant swings she introduces herself as Mimi Necrosynth, Dread Queen. " +
                          "Mimi accompanies " + survivor.charName + " back to the library.\r\n";

                Instantiate(addition, slots[1]);
                survivors = survivorSlots.GetComponentsInChildren<Survivor>();
                survivorCount = survivors.Length;
                Debug.Log(survivors.Length);
            }
        } else {
            // Bad Stuff
            List<Survivor.Status> allStatusesCopy = new List<Survivor.Status>(allStatuses);
            foreach (Survivor.Status status in survivor.GetStatuses()) {
                allStatusesCopy.Remove(status);
            }

            int index = Random.Range(0, allStatusesCopy.Count);
            Survivor.Status affliction = allStatusesCopy[index];

            switch (affliction) {
                case Survivor.Status.Frightened:
                    survivor.AddStatus(Survivor.Status.Frightened);
                    output += survivor.charName + " sees something beyond comprehension.\r\n" + survivor.pronounSubject + " returns to the library jumping at every bump and screech.\r\n";
                    break;
                case Survivor.Status.Hurt:
                    survivor.AddStatus(Survivor.Status.Hurt);
                    output += survivor.charName + " gets clipped by something sharp.\r\n" + survivor.pronounSubject + " returns to the library bleeding and weak.\r\n";
                    break;
            }
        }
        return output;
    }

    private string EvaluateSupport(Survivor helper) {
        string output = "";

        Survivor target = helper.GetSupportTarget();

        if (target.GetStatuses().Count > 0) {
            if (Random.Range(1, 100) <= helper.rally) {
                List<Survivor.Status> statuses = target.GetStatuses();

                int index = Random.Range(0, statuses.Count);
                Survivor.Status status = statuses[index];
                target.RemoveStatus(status);

                switch (status) {
                    case Survivor.Status.Frightened:
                        output += helper.charName + " soothes " + target.charName + "'s frayed nerves. \r\n" +
                                  target.charName + " is ready to go out again.\r\n";
                        break;
                    case Survivor.Status.Hurt:
                        output += helper.charName + " cleans and sets " + target.charName + "'s wounds. \r\n" +
                                  target.charName + " can do heavy work again.\r\n";
                        break;
                }
            } else {
                output += helper.charName + " can't help " + target.charName + "'s wounds. \r\n" +
                          target.charName + " is no better than " + target.pronounSubject.ToLower() + " was before.\r\n";
            }
        } else {
            output += helper.charName + " takes a moment to comfort " + target.charName + ".\r\n";

            if (Random.Range(1, 100) <= (helper.rally + (target.rally / 2))) {
                output += "Insert some kind of good buff for giving nice support.\r\n";
            } else {
                output += "The two don't make much of a connection.\r\n";

                int buildPoints = GetBuildPoints(helper.build);

                if (buildPoints > 0) {
                    barrierCount += buildPoints;
                    output += helper.charName + " restores the barrier by " + buildPoints + " points.\r\n";
                } else {
                    output += helper.charName + " looks outside nervously.\r\n";
                }
            }
        }

        return output;
    }

    private string EvaluateLastChance(Survivor survivor) {
        string output = "";

        if (Random.Range(1, 3) == 2) {
            survivor.RemoveStatus(Survivor.Status.Frightened);
            output += survivor.charName + " uses the pain to sharpen " + survivor.pronounObject.ToLower() + " resolve. " + survivor.pronounSubject + " gets up, ready.\r\n";
        } else {
            output += survivor.charName + " is too scared and hurt to get up. " + survivor.pronounSubject + " remains incapacitated.\r\n";
        }

        return output;
    }

    private string EvaluateNumbers() {
        string output = "";

        output += string.Format("Barrier took {0} net damage. ", ZOMBIE_DAMAGE * zombieCount);

        output += string.Format("{0} horror{1} at the door.\r\n", zombieCount, zombieCount == 1 ? "" : "s");

        if (foodCount == 0) {
            output += "There's no more food. The survivors must find more.";
        } else if (foodCount < survivorCount) {
            output += "The survivors must find more food to make it another day.";
        } else if (foodCount == survivorCount) {
            output += "The survivors have enough food for one more day.";
        } else {
            output += string.Format("The survivors have {0} full days of food left.", foodCount / survivorCount);
        }

        return output;
    }

    public void TellStoriesAndAdvanceDay() {
        dayCount += 1;
        barrierCount -= ZOMBIE_DAMAGE * zombieCount;
        foodCount -= HUNGER_SCORE * survivorCount;
        zombieCount += 1;

        if (barrierCount <= 0 || foodCount < 0) LoseGame();
        else if (dayCount >= 10) WinGame();
        else {
            dayReport.text = "";

            actionReport.text = "The terrified survivors gather around the torch and take turns telling stories of comfort and bravery. ";
            actionReport.text += survivors[0].charName;
            if (survivors.Length == 2) {
                actionReport.text += " and " + survivors[1].charName;
            } else if (survivors.Length == 3) {
                actionReport.text += ", " + survivors[1].charName + ", and " + survivors[2].charName;
            }
            actionReport.text += " take the lessons to heart and get ready to face the hell outside again.";
            actionReport.text += "\r\n" + neutralDescriptors[Random.Range(0, neutralDescriptors.Length)];

            foreach (Survivor survivor in survivors) {
                survivor.RemoveStatus(Survivor.Status.Frightened);
            }

            numbersReport.text = EvaluateNumbers();
        }

        storiesButton.gameObject.SetActive(false);

    }
}
