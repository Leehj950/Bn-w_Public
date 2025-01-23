#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class MapPointRecorder : MonoBehaviour
{
    [ContextMenu("Record Points")]
    void RecordPoints()
    {
        Transform[] children = GetComponentsInChildren<Transform>();
        foreach (var child in children)
        {
            Debug.Log($"Point: {child.name}, Position: {child.position}");
        }
    }
}
#endif

