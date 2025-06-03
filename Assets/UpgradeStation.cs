using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeStation : MonoBehaviour
{
    Material[] materials;
    MeshRenderer meshRenderer;
    [SerializeField] private float dissolveDuration = 2f;
    [SerializeField] Material accentMaterial;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        materials = meshRenderer.materials;
        StartCoroutine(Appear());
    }

    IEnumerator Appear()
    {
        float elapsedTime = 0f;
        while (elapsedTime < dissolveDuration)
        {
            float t = elapsedTime / dissolveDuration;
            foreach (Material material in materials)
            {
                material.SetFloat("_CutoffHeight", Mathf.Lerp(transform.position.y - 5, transform.position.y + 5, t));
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        materials[1] = accentMaterial;
        meshRenderer.materials = materials;
    }
}
