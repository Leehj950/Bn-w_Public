using System;
using UnityEngine;
using Cinemachine;
using UnityEngine.EventSystems; // Cinemachine ��Ű�� ���

public class CameraController : MonoBehaviour
{
    private CinemachineVirtualCamera virtualCamera;
    private Camera mainCamera;

    [SerializeField] private float moveRate = 1f; // �̵� �Ÿ� ����
    [SerializeField] private float accelerationRate = 1f; // ���� ����
    [SerializeField] private float decelerationRate = 1f; // ���� ����

    private Vector3 tmpClickPos;
    private Vector3 tmpCameraPos;
    private Vector3 distanceMoved;
    private Vector3 lastPos;
    private Vector3 velocity;

    // ��� : �� ����
    [SerializeField] private float zoomSpeed = 1.0f; // �ѹ��� �� �Է��� �� �Ǵ� ����
    [SerializeField] private float minZoomSize = -20f; // �ּ� ī�޶� ������
    [SerializeField] private float maxZoomSize = 0.0f; //  �ִ� ī�޶� ������

    // ���� : �� ����
    private float targetZoomSize; // ��ǥ ī�޶� ũ��

    private void Start()
    {
        mainCamera = GetComponentInChildren<Camera>();
        virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>(); // virtualCamera ����
    }

    private void Update()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            // ����� �÷������� �����ϴ� ����

            // UI ���������� ����
            if (EventSystem.current.IsPointerOverGameObject())
            {
                // ���� ����
                return;
            }

            TouchMovement();
            TouchZoom(); // TODO �ܾȵ�
        }
        else if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.LinuxPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {
            // PC �÷������� �����ϴ� ����

            // UI ���������� ����
            if (EventSystem.current.IsPointerOverGameObject())
            {
                // ���� ����
                return;
            }
            MouseMovement();
            MouseZoom();
        }
        else
        {
            RunningAccelertion();
        }
    }

    #region PC

    private void MouseMovement()
    {
        if (Input.GetMouseButtonDown(0))
        {
            tmpClickPos = Input.mousePosition;
            tmpCameraPos = virtualCamera.transform.position;

            ResetAcceleration();
        }
        else if (Input.GetMouseButton(0))
        {
            // ī�޶� �̵� (������ Ŭ�� ��ġ - ���� ���콺 ��ġ)
            Vector3 movePos = mainCamera.ScreenToViewportPoint(tmpClickPos - Input.mousePosition);
            movePos = new Vector3(movePos.y * 1.7f, 0, -movePos.x); // y�� ��� z�� ���
            virtualCamera.transform.position = tmpCameraPos + movePos * moveRate;
            // ���ӵ� ���
            CheckAcceleration();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            // ���ӵ� ����
            SetAcceleration(distanceMoved);
        }
    }

    private void MouseZoom()
    {
        var scrollInput = Input.GetAxis("Mouse ScrollWheel");
        var hasScrollInput = Mathf.Abs(scrollInput) > Mathf.Epsilon;
        if (!hasScrollInput)
        {
            return;
        }

        // virtualCamera�� ���� ���콺 ��ũ�� �Է¿� ���� �����Ͽ� Ȯ��/���
        var newSize = virtualCamera.m_Lens.OrthographicSize - scrollInput * zoomSpeed;

        // ī�޶� ũ�� ���� �ּҰ��� �ִ밪 ���̷� ����
        targetZoomSize = Mathf.Clamp(newSize, minZoomSize, maxZoomSize);

        if (Math.Abs(targetZoomSize - virtualCamera.m_Lens.OrthographicSize) < Mathf.Epsilon)
        {
            return;
        }

        // virtualCamera ũ�� ����
        virtualCamera.m_Lens.OrthographicSize = targetZoomSize;
    }

    #endregion

    #region Mobile

    private void TouchMovement()
    {
        // �Ѽհ������� ��ġ���� ������ �Լ��� ����
        if (Input.touchCount != 1)
            return;

        Touch touch = Input.GetTouch(0);

        // ù ��ġ ������ ��
        if (touch.phase == TouchPhase.Began)
        {
            tmpClickPos = touch.position;
            tmpCameraPos = virtualCamera.transform.position;

            ResetAcceleration();
        }
        else if (touch.phase == TouchPhase.Moved)
        {
            Vector3 movePos = mainCamera.ScreenToViewportPoint(tmpClickPos - (Vector3)touch.position);
            movePos = new Vector3(movePos.y * 1.7f, 0, -movePos.x);
            virtualCamera.transform.position = tmpCameraPos + (movePos * moveRate);

            CheckAcceleration();
        }
        else if (touch.phase == TouchPhase.Ended)
        {
            SetAcceleration(distanceMoved);
        }
    }

    private void TouchZoom()
    {
        if (Input.touchCount == 2)
        {
            // �� ���� ��ġ�� ���� ��
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            // ���� �����Ӱ� ���� �������� ��ġ ��ġ ���� ���
            float previousDistance = (touch1.position - touch2.position).magnitude - (touch1.deltaPosition - touch2.deltaPosition).magnitude;
            float currentDistance = (touch1.position - touch2.position).magnitude;

            // ��ġ ���� �Ÿ� ��ȭ�� ���� �� �ӵ� ���
            float deltaDistance = currentDistance - previousDistance;

            // virtualCamera�� ũ�⸦ �� �ӵ��� ���� ����
            var newSize = virtualCamera.m_Lens.OrthographicSize - deltaDistance * zoomSpeed * Time.deltaTime;

            // ī�޶� ũ�� ���� �ּҰ��� �ִ밪 ���̷� ����
            targetZoomSize = Mathf.Clamp(newSize, minZoomSize, maxZoomSize);

            if (Math.Abs(targetZoomSize - virtualCamera.m_Lens.OrthographicSize) > Mathf.Epsilon)
            {
                virtualCamera.m_Lens.OrthographicSize = targetZoomSize;
            }
        }
    }

    #endregion

    private void CheckAcceleration()
    {
        distanceMoved = transform.position - lastPos;
        lastPos = transform.position;
        velocity = Vector3.zero;
    }

    private void ResetAcceleration()
    {
        distanceMoved = Vector3.zero;
        lastPos = Vector3.zero;
        velocity = Vector3.zero;
    }

    private void SetAcceleration(Vector3 acceleration)
    {
        velocity = (acceleration / Time.deltaTime) * accelerationRate;
    }

    private void RunningAccelertion()
    {
        if (velocity.sqrMagnitude == 0)
            return;

        Vector3 deceleration = velocity * (Time.deltaTime * decelerationRate);
        velocity -= deceleration;

        if (velocity.sqrMagnitude < 0.5f && velocity.sqrMagnitude > -0.5f)
        {
            velocity = Vector3.zero;
        }

        virtualCamera.transform.position = transform.position + deceleration;
    }
}
