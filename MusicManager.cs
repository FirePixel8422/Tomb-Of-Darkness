using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;
    private void Awake()
    {
        Instance = this;
    }


    public AudioSource musicPlayer;
    public AudioSource musicPlayerAlt;

    public bool altMusicPlayerActive;
    public AudioClip[] clips;



    public void ChangeMusicTrack(int id, float fadeSpeed)
    {
        StartCoroutine(FadeChangeMusicTrack(clips[id], fadeSpeed));
    }
    public void ChangeMusicTrack(AudioClip clip, float fadeSpeed)
    {
        StartCoroutine(FadeChangeMusicTrack(clip, fadeSpeed));
    }

    private IEnumerator FadeChangeMusicTrack(AudioClip audioClip, float fadeSpeed)
    {
        AudioSource currentSource = altMusicPlayerActive ? musicPlayerAlt : musicPlayer;
        AudioSource altSource = altMusicPlayerActive ? musicPlayer : musicPlayerAlt;

        altSource.clip = audioClip;
        altSource.Play();

        while (currentSource.volume > 0)
        {
            currentSource.volume -= fadeSpeed * Time.deltaTime;
            altSource.volume += fadeSpeed * Time.deltaTime;
            yield return null;
        }
        currentSource.Stop();

        altMusicPlayerActive = !altMusicPlayerActive;
    }
}
