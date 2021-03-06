﻿using System.Collections;
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
    public List<GameObject> survivorsToBeFound = new List<GameObject>();

    public Button nextDayButton;
    public Button storiesButton;

    public Text dayText;
    public Text barrierText;
    public Text foodText;
    public Text zombieText;

    public FadePanelAndText nightlyNews;
    private Text dayReport;
    private Text actionReport;
    private Text numbersReport;

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

    public Canvas talkCanvas;

    // Survivor suite
    Mimi mimi = null;

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

        Text[] texts = nightlyNews.GetComponentsInChildren<Text>();
        dayReport = texts[0];
        actionReport = texts[1];
        numbersReport = texts[2];
    }

    private void Start() {
        SoundManager.sm.PlayGameMain();
        UpdateGUI();
        nextDayButton.interactable = false;
    }

    private void Update() {
        if (Input.GetKeyUp(KeyCode.R)) {
            ReloadLevel();
        }
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
        dayText.text = "Day " + dayCount.ToString();
        barrierText.text = "Barrier Strength: " + barrierCount.ToString();
        foodText.text = "Food Count: " + foodCount.ToString();
        zombieText.text = "Monsters Outside: " + zombieCount.ToString();

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

    public Canvas GetTalkCanvas() {
        return talkCanvas;
    }

    public void AdvanceDay() {
        dayCount += 1;
        barrierCount -= ZOMBIE_DAMAGE * zombieCount;
        foodCount -= HUNGER_SCORE * survivorCount;
        zombieCount += 1;

        // Mimi Quest Win
        if (mimi != null && mimi.questComplete) {
            int count = 0;
            foreach (Survivor surv in survivors) {
                if (surv.action == Survivor.Action.Loot) {
                    count += 1;
                }
            }

            if (count == SURVIVOR_SLOTS) {
                WinMimiGame();
                return;
            }
        }

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

            if (mimi != null && !mimi.questActive && !mimi.questComplete) {
                int count = 0;
                foreach (Survivor surv in survivors) {
                    if (surv is Mimi) {
                        continue;
                    } else if (surv.questComplete) {
                        count += 1;
                    }
                }

                if (count == 2) {
                    QuestManager.qm.ActivateQuest(mimi);
                }
            }

        }
    }


    private void WinGame() {
        SoundManager.sm.PlayGameWin();
        dayReport.text = "The sounds of gunfire finally reach your door and the pounding stops.\r\n" + 
                         "You join up with the survivor army and live to fight another day.";
        actionReport.text = "Click to play again.";
        numbersReport.enabled = false;

        nightlyNews.NewGame();
    }

    private void WinMimiGame() {
        SoundManager.sm.PlayGameGateWin();
        dayReport.text = "The trio make their way to the rainbow gate. They down Maddie's potions and find little resistance to their advance.\r\n" +
                         "The silence in the warehouse is in stark contrast to the visual noise of the light. A few excited looks and the trio plunge through the gate.\r\n" +
                         "Others find the library  a few days later. Anyone staying for longer than a day swears they can feel a push toward The Gate and the Way, sitting open on a side table near the barricaded door.";
        actionReport.text = "You got them out of the apocalypse! Click to play again.";
        numbersReport.enabled = false;

        nightlyNews.NewGame();
    }

    private void LoseGame() {
        gameOver = true;
        SoundManager.sm.PlayGameOver();

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

        // Johnny Quest Completion
        if ((survivor is JohnnyJacket) && survivor.questActive) {
            Mimi mimi = null;
            JohnnyJacket johnny = null;
            if (survivor is Mimi) {
                mimi = survivor as Mimi;
                foreach (Survivor surv in survivors) {
                    if (surv is JohnnyJacket) {
                        johnny = surv as JohnnyJacket;
                        break;
                    }
                }
            } else if (survivor is JohnnyJacket) {
                johnny = survivor as JohnnyJacket;
                foreach (Survivor surv in survivors) {
                    if (surv is Mimi) {
                        mimi = surv as Mimi;
                        break;
                    }
                }
            }

            if ((mimi.action == Survivor.Action.Loot) && (johnny.action == Survivor.Action.Loot)) {
                output += "\r\nMimi takes lead as she and Johnny go looting for the night. They find themselves at an junkyard near the library. "
                        + "\"This is my spot!\" says Mimi, handing Johnny a spiked bat. It's not long before creatures come over the heaps of junk and "
                        + "the two are bashing away. Johnny is a quick study. \"Radical,\" he remarks as they return with plenty of rations.\r\n\r\n";

                foodCount += 6;

                johnny.loot += 20;
                johnny.combat += 25;

                mimi.combat += 5;

                johnny.questActive = false;
                QuestManager.qm.CompleteQuest(johnny);
                johnny.questComplete = true;

                return output;
            }
        }

        // Mimi Quest Completion
        if ((survivor is Mimi) && survivor.questActive) {
            Mimi mimi = null;
            Maddie maddie = null;
            if (survivor is Mimi) {
                mimi = survivor as Mimi;
                foreach (Survivor surv in survivors) {
                    if (surv is Maddie) {
                        maddie = surv as Maddie;
                        break;
                    }
                }
            } else if (survivor is Maddie) {
                maddie = survivor as Maddie;
                foreach (Survivor surv in survivors) {
                    if (surv is Mimi) {
                        mimi = surv as Mimi;
                        break;
                    }
                }
            }

            if ((mimi.action == Survivor.Action.Loot) && (maddie.action == Survivor.Action.Loot)) {
                output += "\r\nMimi and Maddie approach a warehouse that looks like it's hosting a rave. It's surrounded by tentacled monstrosities. "
                        + "\"Bottoms up,\" says Maddie, handing Mimi a beaker of black ichor. Mimi is already elbow deep in squid bits when Maddie feels "
                        + "the strength roll through her body. The two make it inside. \"No headache!,\" Mimi exclaims and pushes past empty boxes and " 
                        + "turned over metal storage shelves. The rainbow lights at the gate threaten to engulf the pair. \"Let's bring Johnny,\" the say in unison.\r\n\r\n";

                foodCount += 10;

                mimi.questActive = false;
                QuestManager.qm.CompleteQuest(mimi);
                mimi.questComplete = true;

                return output;
            }
        }

        // Normal looting 
        if (Random.Range(1, 100) <= survivor.loot) {
            // Good Stuff
            int foodFound = Random.Range(2, 5);
            foodCount += foodFound;
            output += survivor.charName + " braves the outside world and finds " + foodFound + " ration's worth of food.\r\n";

            // TODO: Add more survivors to the list
            // TODO: Add ability to select slot to add survivor to
            if (survivorsToBeFound.Count > 0 && Random.Range(1, 100) <= survivor.loot) {
                int index = Random.Range(0, survivorsToBeFound.Count);
                GameObject additionObj = survivorsToBeFound[index];
                survivorsToBeFound.Remove(additionObj);

                output += "\r\n" + survivor.charName + " stumbles upon someone wielding a spiked bat with ease. " +
                          "In between giant swings she introduces herself as Mimi Necrosynth, Dread Queen. " +
                          "Mimi accompanies " + survivor.charName + " back to the library.\r\n";

                Instantiate(additionObj, slots[1]);
                survivors = survivorSlots.GetComponentsInChildren<Survivor>();
                survivorCount = survivors.Length;

                GameObject mimiObj = GameObject.Find("Mimi(Clone)");
                mimi = mimiObj.GetComponent<Mimi>();

                // TODO: Can I do this loop better? Also it assumes the survivor is Mimi. Bad for the future.
                foreach (Survivor surv in survivors) {
                    if (surv is JohnnyJacket) {
                        QuestManager.qm.ActivateQuest(surv);
                        break;
                    }
                }
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

        if (helper is Maddie && target is JohnnyJacket) {
            if (target.action == Survivor.Action.None) {
                output += "\r\nJohnny settles into his cot for the night as Maddie pulls out her homemade medical pamphlets and voltmeter. "
                        + "\"It'll be fine,\" she says as she gives Johnny an experimental sedative. Maddie pokes, prods, measures and takes notes. " 
                        + "Johnny wakes up. \"What's was in that syringe? I had the strangest dreams, some great underwater city.\"\r\n"
                        + "\"Oh nothing too much, just some opiates mixed in with some goo fungus I found the bad rations we have. How do you feel?\"\r\n"
                        + "\"Maddie, if it wasn't for how great I feel I'd be seriously concerned. We gotta talk about your methods sometime.\"\r\n" 
                        + "Johnny swears he can feel what people are thinking. Sometimes.\r\n\r\n" ;
                target.rally += 10;
                helper.rally += 10;

                helper.questActive = false;
                QuestManager.qm.CompleteQuest(helper);
                helper.questComplete = true;

                return output;
            }
        }

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

                if (helper is Maddie) {
                    (helper as Maddie).IncreaseQuestCount();
                }
            } else {
                output += helper.charName + " can't help " + target.charName + "'s wounds. \r\n" +
                          target.charName + " is no better than " + target.pronounSubject.ToLower() + " was before.\r\n";
            }
        } else {
            output += helper.charName + " takes a moment to comfort " + target.charName + ".\r\n";

            if (Random.Range(1, 100) <= (helper.rally + (target.rally / 2))) {
                // TODO: revamp this. Maybe not make it permanent per playthrough.
                output += target.charName + " receives " + helper.charName + " warmly. ";

                switch (helper.GetHighestStat()) {
                    case Survivor.Stats.Build:
                        output += helper.charName + " teaches " + target.charName + " the ins and outs of jury rigging a strong barricade. "
                                + target.charName + " feels a little more confident defending the door!\r\n";
                        target.build += 5;
                        break;
                    case Survivor.Stats.Combat:
                        output += helper.charName + " takes " + target.charName + " through the basics of self defense. "
                                + "It's a crazy world out there but " + target.charName + " feels a little more prepared to get through it all unscathed.\r\n";
                        target.combat += 5;
                        break;
                    case Survivor.Stats.Loot:
                        output += helper.charName + " takes out a map " + helper.pronounObject.ToLower() + " has been working on. "
                                + "\"These are the good spots,\" " + helper.pronounObject.ToLower() + " says and begins to show " 
                                + target.charName + " the paths of least resistance through the wreckage. " + target.charName 
                                + " feels ready to loot more rations.\r\n";
                        target.loot += 5;
                        break;
                    case Survivor.Stats.Rally:
                        output += helper.charName + " goes over the basics of knitting wounds back together. " + target.charName 
                                + " is as impressed with " + helper.charName + "'s steady hands as " + helper.pronounObject.ToLower() 
                                + " bed side manner. " + target.charName + " feels more capable of keeping the others healthier.\r\n";
                        target.rally += 5;
                        break;
                    default:
                        Debug.LogError("Helper " + helper.charName + " doesn't have a highest stat. Is this intended?");
                        output += helper.charName + " is feeling too insecure to teach " + target.charName + " anything.\r\n";
                        break;
                }

                if (helper is Maddie) {
                    (helper as Maddie).IncreaseQuestCount();
                }
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

        // TODO: figure out a way to stop checking for this every time instead of just pushing it past 2 for quest count
        if ((helper is Maddie) && (helper as Maddie).GetQuestCount() == 2) {
            QuestManager.qm.ActivateQuest(helper as Maddie);
            (helper as Maddie).IncreaseQuestCount();
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

    public void LoadMainMenu() {
        SceneManager.LoadScene(0);
    }

    public void ReloadLevel() {
        SceneManager.LoadScene(1);
    }

    public void DoAQuit() {
        Application.Quit();
    }
}
