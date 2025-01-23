using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISoundPlayer : MonoBehaviour
{
    public AudioClip buttonClip;

    public void PlayButtonSound()
    {
        SoundManager.Instance.PlaySFX(buttonClip);
    }
}
