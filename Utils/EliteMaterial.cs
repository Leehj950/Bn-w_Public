using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteMaterial : MonoBehaviour
{
    public Material normalMaterial; // �Ϲ� ���ֿ� ����� Material
    public Material eliteMaterial; // ����Ʈ ���� ����� Glow Material

    private SkinnedMeshRenderer skinnedMeshRenderer;

    void Start()
    {
        skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();

        // ������ ����Ʈ ������ Ȯ��
        Character character = GetComponent<Character>();
        if (character != null && character.isElite)
        {
            ApplyEliteMaterial(); // ����Ʈ Material ����
        }
        else
        {
            ApplyNormalMaterial(); // �Ϲ� Material ����
        }
    }

    public void ApplyEliteMaterial()
    {
        if (skinnedMeshRenderer != null && eliteMaterial != null)
        {
            skinnedMeshRenderer.material = eliteMaterial;
        }
    }

    public void ApplyNormalMaterial()
    {
        if (skinnedMeshRenderer != null && normalMaterial != null)
        {
            skinnedMeshRenderer.material = normalMaterial;
        }
    }
}
