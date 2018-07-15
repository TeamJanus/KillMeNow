using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour {

    public Slider volumeSlider;

	// Use this for initialization
	void Start () {
        volumeSlider.value = PlayerPrefManager.GetVolume();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetVolume() {
        PlayerPrefManager.SetVolume(volumeSlider.value);

        SoundManager.sm.musicSource.volume = volumeSlider.value;
    }
}
