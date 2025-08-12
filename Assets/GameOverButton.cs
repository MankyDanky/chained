using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameOverButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    AudioSource audioSource;
    float targetScale = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(targetScale, targetScale, targetScale), Time.unscaledDeltaTime * 10f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = 1.2f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = 1f;
    }
    
    public void LoadScene(int sceneIndex)
    {
        audioSource.Play();
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
    }
}
