using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseClickSound : MonoBehaviour
{
    public AudioClip clickSound; // 클릭 소리 클립

    void Update()
    {
        // 마우스 왼쪽 버튼 클릭 감지
        if (Input.GetMouseButtonDown(0)) // 0: 왼쪽 마우스 버튼
        {
            PlayClickSound();
        }
    }

    void PlayClickSound()
    {
        if (clickSound != null)
        {
            SoundManager.Instance.PlaySFX(clickSound);
        }
    }
}
