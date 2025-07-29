using System.Collections;
using UnityEngine;

public class ToxicSplash : DamageArea
{
    MeshRenderer meshRenderer;
    Material toxicSplashMaterial;
    [SerializeField] float lifetime = 5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
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

        yield return new WaitForSeconds(lifetime);

        elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0.1f, 1f, elapsedTime / duration);
            toxicSplashMaterial.SetFloat("_Cutoff", alpha);
            yield return null;
        }
        Destroy(gameObject);
    }
}
