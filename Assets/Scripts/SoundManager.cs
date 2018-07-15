using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    public AudioSource efxSource;
    public AudioSource musicSource;
    public static SoundManager sm = null;

    public AudioClip gameMain;
    public AudioClip gameOver;
    public AudioClip gameWin;

    public float lowPitchRange = .95f;
    public float highPitchRange = 1.05f;

    private void Awake() {
        if (sm == null) sm = this;
        else if (sm != this) Destroy(gameObject);

        DontDestroyOnLoad(sm);
    }

    public void PlayGameMain() {
        musicSource.clip = gameMain;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlayGameOver() {
        musicSource.clip = gameOver;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlayGameWin() {
        musicSource.clip = gameWin;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlaySingle(AudioClip clip) {
        efxSource.clip = clip;
        efxSource.Play();
    }

    public void RandomizeSfx(params AudioClip[] clips) {
        int randomIndex = Random.Range(0, clips.Length);
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);

        efxSource.pitch = randomPitch;
        efxSource.clip = clips[randomIndex];
        efxSource.Play();
    }
}
