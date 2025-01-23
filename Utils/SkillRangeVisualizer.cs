using UnityEngine;

public class SkillRangeVisualizer : MonoBehaviour
{
    public float skillRange = 5f; // ��ų ����
    public Color gizmoColor = Color.green; // Gizmo ����

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, skillRange);
    }
}
