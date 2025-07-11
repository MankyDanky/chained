using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BlurIn : MonoBehaviour
{
    public Volume volume;
    private DepthOfField dof;

    public float startFocalLength = 0f;
    public float targetFocalLength = 10f;
    public float duration = 1f;

    void Start()
    {
        // Get the DepthOfField component from the volume
        if (volume.profile.TryGet(out dof))
        {
            StartCoroutine(AnimateFocalLength());
        }
        else
        {
            Debug.LogError("DepthOfField not found in Volume profile.");
        }
    }

    System.Collections.IEnumerator AnimateFocalLength()
    {
        float elapsed = 0f;

        dof.focalLength.Override(startFocalLength);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float newFocalLength = Mathf.Lerp(startFocalLength, targetFocalLength, t);
            dof.focalLength.value = newFocalLength;
            yield return null;
        }

        dof.focalLength.Override(targetFocalLength);
    }
}
