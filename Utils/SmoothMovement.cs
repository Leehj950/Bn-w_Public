using UnityEngine;
using System.Collections;

public class SmoothMovement : MonoBehaviour
{
    private Vector3 startPosition;  // ���� ��ġ
    private Vector3 targetPosition; // ��ǥ ��ġ

    private Quaternion startRotation;  // ���� ����
    private Quaternion targeRotation; // ��ǥ ����

    private float moveDuration = 0.2f; // �ð� ���� �̵�
    private Coroutine moveCoroutine; // ���� ���� ���� �ڷ�ƾ

    void Start()
    {
        startPosition = transform.position;
        targetPosition = transform.position;

        startRotation = transform.rotation;
        targeRotation = transform.rotation;

        moveDuration = BattleManager.Instance.locationNotificationDelay;
    }

    // �������� ���ο� ��ġ�� ���� �� ȣ��
    public void UpdateTargetTransform(Vector3 newPosition, Quaternion newRotation)
    {
        // ���� �ڷ�ƾ�� ���� ���̸� ����
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        // �� �ڷ�ƾ ����
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

            // ����: �������� �̵�
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            transform.rotation = Quaternion.Slerp(startRotation, targeRotation, t);

            // �� ������ ���
            yield return null;
        }

        // ������ ��ġ�� ��Ȯ�� ����
        transform.position = targetPosition;
        startPosition = targetPosition;

        transform.rotation = targeRotation;
        startRotation = targeRotation;

        // �ڷ�ƾ ����
        moveCoroutine = null;

    }
}
