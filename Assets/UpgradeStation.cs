using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeStation : MonoBehaviour
{
    Material[] materials;
    MeshRenderer meshRenderer;
    [SerializeField] private float dissolveDuration = 2f;
    [SerializeField] Material accentMaterial;
    [SerializeField] Image[] holograms;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        materials = meshRenderer.materials;
        foreach (Image hologram in holograms)
        {
            hologram.material.SetFloat("_HologramCutoff", 1f);
        }
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
        elapsedTime = 0f;
        while (elapsedTime < dissolveDuration)
        {
            float t = elapsedTime / dissolveDuration;
            foreach (Image hologram in holograms)
            {
                hologram.material.SetFloat("_HologramCutoff", Mathf.Lerp(1, 0.2f, t));
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
