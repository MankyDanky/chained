using System.Collections;
using UnityEngine;

public class ToxicSplash : MonoBehaviour
{
    MeshRenderer meshRenderer;
    Material toxicSplashMaterial;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        toxicSplashMaterial = meshRenderer.material;
        toxicSplashMaterial.SetFloat("_Cutoff", 1f);
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        float duration = 0.5f;
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0.1f, elapsedTime / duration);
            toxicSplashMaterial.SetFloat("_Cutoff", alpha);
            yield return null;
        }
    }
}
