using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DestroyAudioWhenSceneChanged : MonoBehaviour
{
    AudioSource audioSource;

    string currentScene;

    float initialVolume;

    bool coroutineTriggered = false;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        audioSource = GetComponent<AudioSource>();
        initialVolume = audioSource.volume;
        currentScene = SceneManager.GetActiveScene().name;
        coroutineTriggered = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentScene != SceneManager.GetActiveScene().name && !coroutineTriggered)
        {
            coroutineTriggered = true;
            Debug.Log("A");
            StartCoroutine(SlowlyLowerVolume());
        }
    }

    IEnumerator SlowlyLowerVolume()
    {
        while (audioSource.volume > 0.001f)
        {
            audioSource.volume -= 1f * Time.deltaTime * initialVolume;
            yield return new WaitForEndOfFrame();
        }
        Destroy(this.gameObject);
    }
}
