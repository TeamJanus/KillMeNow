using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    private const int ZOMBIE_DAMAGE = 5;
    private const int HUNGER_SCORE = 5;
    private const int SURVIVOR_SLOTS = 3;

    public static GameManager gm = null;

    public GameObject[] slots = new GameObject[SURVIVOR_SLOTS];

    public Text dayText;
    public Text barrierText;
    public Text foodText;
    public Text zombieText;

    public Text dayReport;
    public Text actionReport;
    public Text numbersReport;

    private int dayCount = 1;
    private int barrierCount = 100;
    private int foodCount = 100;
    private int zombieCount = 1;
    private int survivorCount = 1;

    private int daysLeft = 10;
    private string[] neutralDescriptors = new string[] { "The banging at the door grows louder tonight.",
                                                         "The survivors grab a few hours of uneasy sleep.",
                                                         "Night watch catches glimpses of strange shapes in the fading light" };

    void Awake() {
        if (gm == null) gm = this;
        else if (gm != this) Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    void Start() {
        UpdateGUI();
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

        daysLeft -= 1;
        dayReport.text = string.Format("{0} days left until help arrives.", daysLeft);

        actionReport.text = neutralDescriptors[Random.Range(0, neutralDescriptors.Length)];

        numbersReport.text = string.Format("Barrier took {0} damage. {1} units of food left. {2} horrors at the door.",
                                            ZOMBIE_DAMAGE * zombieCount, foodCount, zombieCount);
    }

}
