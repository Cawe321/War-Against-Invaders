using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioFiles", menuName = "WarAgainstInvaders/Settings/AudioFilesSettings", order = 3)]
public class AudioFiles : ScriptableObject
{
    [Header("SFX Audio managed by Manager")]
    public AudioClip _buttonHoverSFX;
    public AudioClip _buttonClickSFX;
    public AudioClip _uiOpenSound;
    public AudioClip _uiCloseSound;
    public AudioClip _gameplayNotificationOpenSound;
    public AudioClip _gameplayNotificationCloseSound;

    [Header("BGM Audio")]
    public AudioClip _defenderMainMenuBGM;
    public AudioClip _invaderMainMenuBGM;
    public AudioClip _gameSceneBGM;
    public AudioClip _matchmakingBGM;

    [Header("SFX Audio managed by external players")]
    public AudioClip _flightEngineSound;
    public AudioClip _gunsFireSound;
    public AudioClip _gunsHitSound;
    public AudioClip _lasersFireSound;
    public AudioClip _lasersHitSound;
    public AudioClip _megaLasersFireSound;
    public AudioClip _megaLasersHitSound;
    public AudioClip _missilesFireSound;
    public AudioClip _explosionSound;
}
