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

    private int dayCount = 1;
    private int barrierCount = 100;
    private int foodCount = 100;
    private int zombieCount = 1;
    private int survivorCount = 1;

    void Awake() {
        if (gm == null) gm = this;
        else if (gm != this) Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    void Start() {
        updateGUI();
    }

    private void updateGUI() {
        dayText.text = dayCount.ToString();
        barrierText.text = barrierCount.ToString();
        foodText.text = foodCount.ToString();
        zombieText.text = zombieCount.ToString();
    }

    public void advanceDay() {
        advanceGUI();
    }

    private void advanceGUI() {
        dayCount += 1;
        barrierCount -= ZOMBIE_DAMAGE * zombieCount;
        foodCount -= HUNGER_SCORE * survivorCount;
        zombieCount += 1;

        updateGUI();
    }
}
