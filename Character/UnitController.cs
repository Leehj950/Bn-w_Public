using UnityEngine;

public class UnitController : MonoBehaviour
{
    public Transform TopCheckPoint;
    public Transform DownCheckPoint;
    public float checkPointRadius = 2f;
    private bool isCapturing = false;

    private void Update()
    {
        float topDistance = Vector3.Distance(transform.position, TopCheckPoint.position);

        if(topDistance <= checkPointRadius && !isCapturing)
        {
            isCapturing = true;
           // CheckpointManager.Instance.SendCaptureRequest(true, )
        }
    }
}