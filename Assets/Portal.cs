using System.Collections;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public Material portalDistortionMaterial;
    Transform player;
    public int nextScene;
    bool isLoading = false;
    Animator canvas;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FirstPersonController.Instance;
        canvas = GameObject.Find("Canvas").GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        float distanceSquared = Vector3.SqrMagnitude(player.position - transform.position);

        if (isLoading)
        {
            return;
        }

        if (distanceSquared < 16 || isLoading)
        {
            if (distanceSquared < 2f)
            {
                distanceSquared = 0.1f;
                StartCoroutine(LoadNextScene());
            }
            distanceSquared *= 0.5625f;
            float distortionStrength = 20 * Mathf.Pow(2.71828f, -distanceSquared);
            portalDistortionMaterial.SetFloat("_Strength", distortionStrength);

        }
        else
        {
            portalDistortionMaterial.SetFloat("_Strength", 0f);
        }

    }

    void OnDestroy()
    {
        portalDistortionMaterial.SetFloat("_Strength", 0f);
    }

    IEnumerator LoadNextScene()
    {
        isLoading = true;
        canvas.SetTrigger("Disappear");
        yield return new WaitForSeconds(1f);
        portalDistortionMaterial.SetFloat("_Strength", 0f);
        UnityEngine.SceneManagement.SceneManager.LoadScene(nextScene);
    }
}
