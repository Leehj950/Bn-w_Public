using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteMaterial : MonoBehaviour
{
    public Material normalMaterial; // 일반 유닛에 사용할 Material
    public Material eliteMaterial; // 엘리트 몹에 사용할 Glow Material

    private SkinnedMeshRenderer skinnedMeshRenderer;

    void Start()
    {
        skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();

        // 유닛이 엘리트 몹인지 확인
        Character character = GetComponent<Character>();
        if (character != null && character.isElite)
        {
            ApplyEliteMaterial(); // 엘리트 Material 적용
        }
        else
        {
            ApplyNormalMaterial(); // 일반 Material 적용
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
