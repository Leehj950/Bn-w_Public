using UnityEngine;
using System.Collections;

public class SmoothMovement : MonoBehaviour
{
    private Vector3 startPosition;  // 현재 위치
    private Vector3 targetPosition; // 목표 위치

    private Quaternion startRotation;  // 현재 방향
    private Quaternion targeRotation; // 목표 방향

    private float moveDuration = 0.2f; // 시간 동안 이동
    private Coroutine moveCoroutine; // 현재 실행 중인 코루틴

    void Start()
    {
        startPosition = transform.position;
        targetPosition = transform.position;

        startRotation = transform.rotation;
        targeRotation = transform.rotation;

        moveDuration = BattleManager.Instance.locationNotificationDelay;
    }

    // 서버에서 새로운 위치를 받을 때 호출
    public void UpdateTargetTransform(Vector3 newPosition, Quaternion newRotation)
    {
        // 기존 코루틴이 실행 중이면 중지
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        // 새 코루틴 시작
        moveCoroutine = StartCoroutine(SmoothMove(newPosition, newRotation));
    }

    private IEnumerator SmoothMove(Vector3 newPosition, Quaternion newRotation)
    {
        startPosition = transform.position;
        targetPosition = newPosition;

        startRotation = transform.rotation;
        targeRotation = newRotation;

        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / moveDuration;

            // 보간: 선형으로 이동
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            transform.rotation = Quaternion.Slerp(startRotation, targeRotation, t);

            // 한 프레임 대기
            yield return null;
        }

        // 마지막 위치를 정확히 설정
        transform.position = targetPosition;
        startPosition = targetPosition;

        transform.rotation = targeRotation;
        startRotation = targeRotation;

        // 코루틴 종료
        moveCoroutine = null;

    }
}
