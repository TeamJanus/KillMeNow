using UnityEngine;
using System.Collections;

/** Taken from Michigan State University's Coursera-based 
    Game Development And Development Specialization at https://www.coursera.org/specializations/game-development
*/
public static class PlayerPrefManager {

    // Examples for later
    public static int GetLives() {
        if (PlayerPrefs.HasKey("Lives")) {
            return PlayerPrefs.GetInt("Lives");
        } else {
            return 0;
        }
    }

    public static void SetLives(int lives) {
        PlayerPrefs.SetInt("Lives", lives);
    }

    public static int GetScore() {
        if (PlayerPrefs.HasKey("Score")) {
            return PlayerPrefs.GetInt("Score");
        } else {
            return 0;
        }
    }

    public static void SetScore(int score) {
        PlayerPrefs.SetInt("Score", score);
    }

    // story the current player state info into PlayerPrefs
    public static void SavePlayerState(int score, int lives) { 
        SetScore(score);
        SetLives(lives);
    }

    // reset stored player state and variables back to defaults
    public static void ResetPlayerState(int startLives) {
        Debug.Log("Player State reset.");
        SetScore(0);
        SetLives(startLives);
    }

    // output the defined Player Prefs to the console
    public static void ShowPlayerPrefs() {
        // store the PlayerPref keys to output to the console
        string[] values = { "Score", "Lives" };

        // loop over the values and output to the console
        foreach (string value in values) {
            if (PlayerPrefs.HasKey(value)) {
                Debug.Log(value + " = " + PlayerPrefs.GetInt(value));
            } else {
                Debug.Log(value + " is not set.");
            }
        }
    }
}
