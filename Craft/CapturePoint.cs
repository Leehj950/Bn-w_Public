using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.CanvasScaler;

enum Condition
{
    Default,
    Captured,
    OpponentCaptured
}

public class CapturePoint : MonoBehaviour
{
    public bool isTop; // 위/아래 구분
    public const float requiredCaptureTime = 5f;
    public float triggerRadius = 5f;
    public Transform centerPoint;
    public Image fillImage;
    public Image backGround;
    public Mesh opponentCapturedMesh;
    public Mesh capturedMesh;
    public Mesh defaultMesh;


    private float captureTimer = 0f;
    private bool isCapturing = false;
    private bool isCaptured = false;
    private bool isOpponentCaptured = false;
    private WaitForSeconds checkUnitsInRangeDelay = new WaitForSeconds(0.2f);
    private WaitForSeconds UnitStopDelay = new WaitForSeconds(1.5f);

    public CapturePointUI capturePointUI;
    private MeshFilter meshFilter;

    private Dictionary<int, Character> unitsInZone = new Dictionary<int, Character>(); // 유닛 ID 관리

    private void Start()
    {
        capturePointUI = GetComponent<CapturePointUI>();
        meshFilter = GetComponent<MeshFilter>();
        var components = capturePointUI.uiInstance.GetComponentsInChildren<Image>(true);
        fillImage = components.LastOrDefault();

        StartCoroutine(CheckUnitsInRangeCoroutine());
    }

    private void Update()
    {
        if (isCapturing)
        {
            captureTimer += Time.deltaTime;
            capturePointUI.UpdateTimerUI(captureTimer, requiredCaptureTime);

            if (captureTimer >= requiredCaptureTime)
            {
                isCapturing = false;
            }

            //foreach (var unit in unitsInZone.Values)
            //{
            //    if(unit.targetEnemyList.Count == 0)
            //    {
            //        Vector3 targetPosition = GetRandomPositionAroundCenter(centerPoint.position, 2f); // 2f는 오프셋 반경
            //        unit.SetTarget(targetPosition);
            //    }
            //}          
        }

        //if (!isCaptured || isOpponentCaptured)
        //{
        //    foreach (var unit in unitsInZone.Values)
        //    {
        //        if (unit.gameObject.activeSelf) // TODO 유닛이 죽으면 unitsInZone 에서 빼주는 로직 필요
        //        {
        //            //Vector3 targetPosition = GetRandomPositionAroundCenter(centerPoint.position, 1f); // 2f는 오프셋 반경
        //            unit.transform.position = Vector3.MoveTowards(unit.transform.position, centerPoint.position, unit.characterData.Speed * Time.deltaTime);
        //        }
        //    }
        //}
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Unit")) // 유닛인지 확인
        {
            Character unit = other.GetComponent<Character>();
            int unitId = unit.unitId;
            if (!unitsInZone.ContainsKey(unitId) && !unit.IsEnemy())
            {
                unitsInZone.Add(unitId, unit);
                if (CaptureManager.Instance != null)
                {
                    // 서버에 유닛 진입 알림
                    CaptureManager.Instance.SendCaptureRequest(unitId);
                    if (!isCaptured || isOpponentCaptured)
                    {
                        Vector3 targetPosition = GetRandomPositionAroundCenter(centerPoint.position, 2f);
                        unit.SetTarget(targetPosition);
                        unit.characterStateMachine.ChangeState(unit.characterStateMachine.idleState);
                        StartCoroutine(UnitStopCoroutine(unit));
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Unit")) // 유닛인지 확인
        {
            Character unit = other.GetComponent<Character>();
            int unitId = unit.unitId;
            if (CaptureManager.Instance != null && !unit.IsEnemy())
            {
                // 유닛이 점령 중인지 확인
                if (unitsInZone.ContainsKey(unitId) && !isCapturing)
                {
                    unitsInZone.Remove(unitId);
                    CaptureManager.Instance.SendCaptureExitRequest(unitId);
                    
                }
            }
        }
    }

    private void StopUnitInCapturePoint(Character unit)
    {
        unit.ChangeStateByAnimationType(CatDogEnums.AnimationType.Idle);
    }

    //private void RemoveUnitFromCapturePoint(Character unit)
    //{
    //    throw new NotImplementedException();
    //}

    public void StartCapture(bool isOpponent)
    {
        isCapturing = true;
        isCaptured = false;
        Debug.Log($"Capture started at {(isTop ? "Top" : "Bottom")} point.");
        // 타이머 시작 로직
        if (capturePointUI.uiInstance != null)
        {
            capturePointUI.uiInstance.SetActive(true);
            fillImage.color = isOpponent ? Color.red : Color.blue;
        }

        // 유닛 이동 제한
        //foreach (var unit in unitsInZone.Values)
        //{
        //    Vector3 targetPosition = GetRandomPositionAroundCenter(centerPoint.position, 2f); // 2f는 오프셋 반경
        //    unit.SetTarget(targetPosition);
        //}
    }

    public void PauseCapture()
    {
        isCapturing = false;
        Debug.Log($"Capture paused at {(isTop ? "Top" : "Bottom")} point.");
        // 타이머 일시정지
        if (capturePointUI.uiInstance != null)
        {
            capturePointUI.uiInstance.SetActive(true);

            // 중단 상태에서 유지
        }
    }

    public void ResetTimer()
    {
        captureTimer = 0f;
        isCapturing = false;
        Debug.Log($"Capture timer reset at {(isTop ? "Top" : "Bottom")} point.");
        // 타이머 초기화 로직
        if (capturePointUI.uiInstance != null)
        {
            capturePointUI.uiInstance.SetActive(true);

        }
    }

    public void CaptureSuccess(bool isOpponent)
    {
        isCapturing = false;

        captureTimer = 0;

        Debug.Log($"Capture succeeded at {(isTop ? "Top" : "Bottom")} point.");
        // 점령 성공 처리
        foreach (var unit in unitsInZone.Values)
        {
            if (!unit.IsEnemy() && unit.gameObject.activeSelf) // 상대방 캐릭터나 내캐릭터가 죽으면 실행되지 말아야함 // TODO 유닛이 죽으면 unitsInZone 에서 빼주는 로직 필요
            {
                Debug.Log(unit.gameObject.name);

                unit.ChangeStateByAnimationType(CatDogEnums.AnimationType.Run);
                unit.MoveToNextTarget();
            }
           
        }

        unitsInZone.Clear();

        capturePointUI.ResetUI();

        if (capturePointUI.uiInstance != null)
        {
            capturePointUI.uiInstance.SetActive(false);
        }

        if(isOpponent)
        {
            ChangeMeshBasedOnCondition("OpponentCaptured");
            isOpponentCaptured = true;
            isCaptured = false;
        }
        else
        {
            ChangeMeshBasedOnCondition("Captured");
            isCaptured = true;
            isOpponentCaptured = false;
        }
    }

    public void ChangeMeshBasedOnCondition(string condition)
    {
        if (meshFilter != null)
        {
            switch (condition)
            {
                case "OpponentCaptured":
                    meshFilter.sharedMesh = opponentCapturedMesh;
                    break;
                case "Captured":
                    meshFilter.sharedMesh = capturedMesh;
                    break;
                default:
                    meshFilter.sharedMesh = defaultMesh;
                    break;
            }
        }
    }

    private IEnumerator CheckUnitsInRangeCoroutine()
    {
        while (true)
        {
            CheckUnitsInRange();
            yield return checkUnitsInRangeDelay;
        }
    }
    private void CheckUnitsInRange()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, triggerRadius);

        if (colliders.Length == 0)
        {
            capturePointUI.uiInstance.SetActive(false);
        }
    }

    Vector3 GetRandomPositionAroundCenter(Vector3 center, float radius)
    {
        Vector2 randomOffset = Random.insideUnitCircle * radius;
        return new Vector3(center.x + randomOffset.x, center.y, center.z + randomOffset.y);
    }

    private IEnumerator UnitStopCoroutine(Character unit)
    {
        yield return UnitStopDelay;
        unit.StopMove();
    }
}

