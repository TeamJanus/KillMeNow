using UnityEngine;
using System.Collections;

public static class PlayerPrefManager {

    public static float GetVolume() {
        if (PlayerPrefs.HasKey("Volume")) {
            return PlayerPrefs.GetFloat("Volume");
        } else {
            return 0;
        }
    }

    public static void SetVolume(float volume) {
        PlayerPrefs.SetFloat("Volume", volume);
    }

}
