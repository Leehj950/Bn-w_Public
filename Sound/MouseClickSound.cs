using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseClickSound : MonoBehaviour
{
    public AudioClip clickSound; // Ŭ�� �Ҹ� Ŭ��

    void Update()
    {
        // ���콺 ���� ��ư Ŭ�� ����
        if (Input.GetMouseButtonDown(0)) // 0: ���� ���콺 ��ư
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
