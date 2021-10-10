using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterSeconds : MonoBehaviour
{
    public void DestroyAfterWaiting(float seconds)
    {
        StartCoroutine(WaitToDestroy(seconds));
    }

    IEnumerator WaitToDestroy(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(this.gameObject);
    }
}
