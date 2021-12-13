using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 3D Buttons
/// </summary>
[RequireComponent(typeof(Collider))]
public class Button3D : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] Vector3 scaleUp = new Vector3(1.25f, 1.25f, 1.25f);

    [Header("Events")]
    [SerializeField] UnityEvent OnHoverEnter;
    [SerializeField] UnityEvent OnHoverExit;
    [SerializeField] UnityEvent OnMouseClick;

    Vector3 originalScale;

    InterfaceAnimManager animManager;

    private void Awake()
    {
        originalScale = transform.localScale;
        animManager = GetComponent<InterfaceAnimManager>();
    }

    private void OnMouseEnter()
    {
        OnHoverEnter.Invoke();
        transform.localScale = new Vector3(transform.localScale.x * scaleUp.x, transform.localScale.y * scaleUp.y, transform.localScale.z * scaleUp.z);
        AudioManager.instance.PlaySFX(AudioManager.instance.audioFiles._buttonHoverSFX);
    }

    private void OnMouseExit()
    {
        OnHoverExit.Invoke();
        transform.localScale = originalScale;
    }

    private void OnMouseDown()
    {
        OnMouseClick.Invoke();
        AudioManager.instance.PlaySFX(AudioManager.instance.audioFiles._buttonClickSFX);
    }
}
