using UnityEngine;

public class LookGrow : MonoBehaviour
{

    [SerializeField] float originalScale;
    [SerializeField] RectTransform[] rectTransforms;
    bool isBeingAimedAt = false;

    // Update is called once per frame
    void Update()
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        Vector2 mousePos = Input.mousePosition;

        float distanceToMouse = Vector2.Distance(screenPos, mousePos);
        float visibilityThreshold = 50f;
        Debug.Log(distanceToMouse);
        isBeingAimedAt = distanceToMouse < visibilityThreshold;
        foreach (RectTransform rectTransform in rectTransforms)
        {
            if (isBeingAimedAt)
            {
                float scaleFactor = Mathf.Lerp(rectTransform.localScale.x, originalScale * 1.6f, Time.deltaTime * 5f);
                rectTransform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
            }
            else
            {
                float scaleFactor = Mathf.Lerp(rectTransform.localScale.x, originalScale, Time.deltaTime * 5f);
                rectTransform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
            }
        }
        
    }
}
