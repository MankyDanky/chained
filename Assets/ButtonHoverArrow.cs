using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class ButtonHoverArrow : MonoBehaviour
{
    public GameObject arrow;
    Image arrowImage;
    public RectTransform[] buttons;
    public Animator canvasAnimator;

    [Header("Scaling Settings")]
    public float hoverScale = 1.2f;
    public float scaleSpeed = 5f;

    private Dictionary<RectTransform, Coroutine> scaleCoroutines = new Dictionary<RectTransform, Coroutine>();

    void Start()
    {
        arrowImage = arrow.GetComponent<Image>();
        arrowImage.enabled = false;

        foreach (var btn in buttons)
        {
            var trigger = btn.gameObject.AddComponent<EventTrigger>();

            // OnPointerEnter
            var enter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            enter.callback.AddListener((e) =>
            {
                OnHoverEnter(btn);
                StartScaling(btn, hoverScale);
            });
            trigger.triggers.Add(enter);

            // OnPointerExit
            var exit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
            exit.callback.AddListener((e) =>
            {
                OnHoverExit(btn);
                StartScaling(btn, 1f);
            });
            trigger.triggers.Add(exit);
        }
    }

    void OnHoverEnter(RectTransform btn)
    {
        arrowImage.enabled = true;
        Vector3 btnPos = btn.position;
        arrow.transform.position = new Vector3(btnPos.x - 15, btnPos.y, btnPos.z);
    }

    void OnHoverExit(RectTransform btn)
    {
        arrowImage.enabled = false;
    }

    void StartScaling(RectTransform btn, float targetScale)
    {
        // Stop only this button's coroutine
        if (scaleCoroutines.ContainsKey(btn) && scaleCoroutines[btn] != null)
            StopCoroutine(scaleCoroutines[btn]);

        scaleCoroutines[btn] = StartCoroutine(ScaleTo(btn, targetScale));
    }

    IEnumerator ScaleTo(RectTransform target, float targetScale)
    {
        Vector3 startScale = target.localScale;
        Vector3 endScale = Vector3.one * targetScale;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * scaleSpeed;
            target.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }
        target.localScale = endScale;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void PlayGame()
    {
        StartCoroutine(LoadGame());
    }

    public void LoadScene(int sceneIndex)
    {
        StartCoroutine(LoadSceneCoroutine(sceneIndex));
    }

    IEnumerator LoadGame()
    {
        canvasAnimator.SetTrigger("Disappear");
        yield return new WaitForSeconds(1f);
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    IEnumerator LoadSceneCoroutine(int sceneIndex)
    {
        canvasAnimator.SetTrigger("Disappear");
        yield return new WaitForSeconds(1f);
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
    }
}
