using System.Collections;
using UnityEngine;

public class Shake : MonoBehaviour
{
    public bool start = false;
    [SerializeField] AnimationCurve curve;
    [SerializeField] float duration = 1f;

    void Update()
    {
        if (start)
        {
            start = false;
            StartCoroutine(Shaking());
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
}
