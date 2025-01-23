using UnityEngine;

public class SkillRangeVisualizer : MonoBehaviour
{
    public float skillRange = 5f; // 스킬 범위
    public Color gizmoColor = Color.green; // Gizmo 색상

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, skillRange);
    }
}
