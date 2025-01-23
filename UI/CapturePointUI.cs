using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CapturePointUI : MonoBehaviour
{
    public Transform capturePoint; // 점령지 Transform
    public GameObject captureProgressSliderObject; // 타이머 슬라이더 프리팹
    public Canvas worldSpaceCanvas; // 월드 공간 캔버스
    private Slider captureProgressSlider;
    private Vector3 capturePointPosition;
    public GameObject uiInstance;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;

        uiInstance = Instantiate(captureProgressSliderObject, worldSpaceCanvas.transform);

        captureProgressSlider = uiInstance.GetComponentInChildren<Slider>();
        captureProgressSlider.value = 0;
        capturePointPosition = capturePoint.position;
        uiInstance.SetActive(false);
        UpdateUIPosition();
    }

    public void UpdateTimerUI(float currentTime, float requiredTime)
    {
        captureProgressSlider.value = currentTime / requiredTime;
    }

    public void ResetUI()
    {
        captureProgressSlider.value = 0f;
        uiInstance.SetActive(false);
    }

    public void UpdateUIPosition()
    {
        if (uiInstance != null)
        {
            uiInstance.transform.position = capturePointPosition + new Vector3(0, 2, 0); // 위로 2 유닛
        }
    }
}
