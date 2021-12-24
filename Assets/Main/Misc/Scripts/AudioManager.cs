using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : SingletonObject<AudioManager>
{
    public AudioFiles audioFiles;

    [SerializeField]
    AudioSource player_BGM;
    [SerializeField]
    AudioSource player_SFX;

    public float SwitchBGMDuration = 1f;

    public float BGMVolume = 0.5f;

    Coroutine lastCO;

    public void PlayBGM(AudioClip audioClip)
    {
        StartCoroutine(SwitchBGM(audioClip));
    }

    public void StopBGM()
    {
        if (player_BGM.clip != null)
        {
            if (lastCO != null)
                StopCoroutine(lastCO);
            lastCO = StartCoroutine(SwitchBGM(null));
        }
    }

    public void PlaySFX(AudioClip audioClip)
    {
        player_SFX.clip = audioClip;
        player_SFX.Play();
    }

    IEnumerator SwitchBGM(AudioClip newBGM)
    {
        if (newBGM != player_BGM.clip)
        {
            if (player_BGM.clip != null)
            {
                while (true)
                {
                    yield return new WaitForEndOfFrame();
                    if (player_BGM.volume > Mathf.Epsilon)
                        player_BGM.volume -= Time.deltaTime * SwitchBGMDuration * BGMVolume;
                    else
                    {
                        player_BGM.Stop();
                        break;
                    }
                }
            }
            player_BGM.clip = newBGM;
            player_BGM.volume = BGMVolume;
            if (newBGM != null)
                player_BGM.Play();
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(this);
    }
}
