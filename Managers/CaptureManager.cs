using CatDogEnums;
using Ironcow.WebSocketPacket;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class CaptureManager : Singleton<CaptureManager>
{
    private float captureTimer = 0;
    private bool isCapturing = false;
    private bool isPaused = false;
    private bool isUpperPoint = false;

    public CapturePoint topCapturePoint;
    public CapturePoint bottomCapturePoint;

    private Dictionary<bool, CapturePoint> capturePoints = new Dictionary<bool, CapturePoint>();

    private void Start()
    {
        capturePoints.Add(true, topCapturePoint);
        capturePoints.Add(false, bottomCapturePoint);

    }

    public void SendCaptureRequest(int unitId)
    {

        Debug.Log("unitID" + unitId);

        // C2S 거점점령시도 요청 전송
        if (SocketManager.Instance.isConnected)
        {
            GamePacket packet = new GamePacket();
            var checkpointRequest = new C2SEnterCheckpointNotification();
            checkpointRequest.UnitId = unitId;

            packet.EnterCheckpointNotification = checkpointRequest;
            DebugOpt.Log("패킷 보냄 ");

            SocketManager.Instance.Send(packet);
        }
        // 실제 네트워크 전송 로직 구현
    }

    public void SendCaptureExitRequest(int UnitId)
    {
        GamePacket packet = new GamePacket();
        var checkpointExitRequest = new C2SExitCheckpointNotification();
        checkpointExitRequest.UnitId = UnitId;

        packet.ExitCheckpointNotification = checkpointExitRequest;
        DebugOpt.Log("유닛 거점 이탈");

        SocketManager.Instance.Send(packet);
    }

    public void StopCapture()
    {
        isCapturing = false;
        Debug.Log("Capture Stopped");
    }

    public void RegisterCapturePoint(bool isTop, CapturePoint capturePoint)
    {
        if (!capturePoints.ContainsKey(isTop))
        {
            capturePoints.Add(isTop, capturePoint);
        }
    }

    public void OnTryOccupation(bool isTop, bool isOpponent)
    {
        if (capturePoints.TryGetValue(isTop, out CapturePoint capturePoint))
        {
            capturePoint.StartCapture(isOpponent);
        }
    }

    public void OnPauseOccupation(bool isTop)
    {
        if (capturePoints.TryGetValue(isTop, out CapturePoint capturePoint))
        {
            capturePoint.PauseCapture();
        }
    }

    public void OnResetOccupationTimer(bool isTop)
    {
        if (capturePoints.TryGetValue(isTop, out CapturePoint capturePoint))
        {
            capturePoint.ResetTimer();
        }
    }

    public void OnOccupationSuccess(bool isTop, bool isOpponent)
    {
        if (capturePoints.TryGetValue(isTop, out CapturePoint capturePoint))
        {
            capturePoint.CaptureSuccess(isOpponent);
        }
    }
}