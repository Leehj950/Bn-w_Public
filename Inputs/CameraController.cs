using System;
using UnityEngine;
using Cinemachine;
using UnityEngine.EventSystems; // Cinemachine 패키지 사용

public class CameraController : MonoBehaviour
{
    private CinemachineVirtualCamera virtualCamera;
    private Camera mainCamera;

    [SerializeField] private float moveRate = 1f; // 이동 거리 비율
    [SerializeField] private float accelerationRate = 1f; // 가속 비율
    [SerializeField] private float decelerationRate = 1f; // 감속 비율

    private Vector3 tmpClickPos;
    private Vector3 tmpCameraPos;
    private Vector3 distanceMoved;
    private Vector3 lastPos;
    private Vector3 velocity;

    // 상수 : 줌 관련
    [SerializeField] private float zoomSpeed = 1.0f; // 한번의 줌 입력의 줌 되는 정도
    [SerializeField] private float minZoomSize = -20f; // 최소 카메라 사이즈
    [SerializeField] private float maxZoomSize = 0.0f; //  최대 카메라 사이즈

    // 변수 : 줌 관련
    private float targetZoomSize; // 목표 카메라 크기

    private void Start()
    {
        mainCamera = GetComponentInChildren<Camera>();
        virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>(); // virtualCamera 참조
    }

    private void Update()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            // 모바일 플랫폼에서 동작하는 로직

            // UI 위에있으면 리턴
            if (EventSystem.current.IsPointerOverGameObject())
            {
                // 동작 멈춤
                return;
            }

            TouchMovement();
            TouchZoom(); // TODO 줌안됨
        }
        else if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.LinuxPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {
            // PC 플랫폼에서 동작하는 로직

            // UI 위에있으면 리턴
            if (EventSystem.current.IsPointerOverGameObject())
            {
                // 동작 멈춤
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
            // 카메라 이동 (저장한 클릭 위치 - 현재 마우스 위치)
            Vector3 movePos = mainCamera.ScreenToViewportPoint(tmpClickPos - Input.mousePosition);
            movePos = new Vector3(movePos.y * 1.7f, 0, -movePos.x); // y축 대신 z축 사용
            virtualCamera.transform.position = tmpCameraPos + movePos * moveRate;
            // 가속도 계산
            CheckAcceleration();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            // 가속도 적용
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

        // virtualCamera의 줌을 마우스 스크롤 입력에 따라 변경하여 확대/축소
        var newSize = virtualCamera.m_Lens.OrthographicSize - scrollInput * zoomSpeed;

        // 카메라 크기 값을 최소값과 최대값 사이로 유지
        targetZoomSize = Mathf.Clamp(newSize, minZoomSize, maxZoomSize);

        if (Math.Abs(targetZoomSize - virtualCamera.m_Lens.OrthographicSize) < Mathf.Epsilon)
        {
            return;
        }

        // virtualCamera 크기 갱신
        virtualCamera.m_Lens.OrthographicSize = targetZoomSize;
    }

    #endregion

    #region Mobile

    private void TouchMovement()
    {
        // 한손가락으로 터치하지 않으면 함수를 종료
        if (Input.touchCount != 1)
            return;

        Touch touch = Input.GetTouch(0);

        // 첫 터치 상태일 때
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
            // 두 개의 터치가 있을 때
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            // 이전 프레임과 현재 프레임의 터치 위치 차이 계산
            float previousDistance = (touch1.position - touch2.position).magnitude - (touch1.deltaPosition - touch2.deltaPosition).magnitude;
            float currentDistance = (touch1.position - touch2.position).magnitude;

            // 터치 간의 거리 변화에 따른 줌 속도 계산
            float deltaDistance = currentDistance - previousDistance;

            // virtualCamera의 크기를 줌 속도에 따라 변경
            var newSize = virtualCamera.m_Lens.OrthographicSize - deltaDistance * zoomSpeed * Time.deltaTime;

            // 카메라 크기 값을 최소값과 최대값 사이로 제한
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
