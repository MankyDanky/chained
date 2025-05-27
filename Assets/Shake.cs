using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Shake : MonoBehaviour
{
    public bool start = false;
    public bool startHurt = false;
    [SerializeField] AnimationCurve curve;
    [SerializeField] float duration = 1f;
    [SerializeField] Volume volume;
    [SerializeField] float hurtDuration = 1f;
    [SerializeField] AnimationCurve hurtCurve;


    void Start()
    {
        volume = FindAnyObjectByType<Volume>();
    }

    void Update()
    {
        if (start)
        {
            start = false;
            StartCoroutine(Shaking());
        }
        if (startHurt)
        {
            startHurt = false;
            StartCoroutine(Hurt());
        }
    }

    IEnumerator Shaking()
    {
        Vector3 startPos = transform.localPosition;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float strength = curve.Evaluate(elapsedTime / duration);
            transform.localPosition = startPos + Random.insideUnitSphere * strength;
            yield return null;
        }
        transform.localPosition = startPos;
    }

    IEnumerator Hurt()
    {
        float elapsedTime = 0f;

        while (elapsedTime < hurtDuration)
        {
            elapsedTime += Time.deltaTime;
            float smoothness = hurtCurve.Evaluate(elapsedTime / hurtDuration);
            float strength = Mathf.Lerp(0.1f, 0.2f, smoothness);
            volume.profile.TryGet(out Vignette vignette);
            vignette.intensity.Override(strength);
            vignette.smoothness.Override(smoothness);
            yield return null;
        }
    }
}
