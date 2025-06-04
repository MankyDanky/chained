using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeStation : MonoBehaviour
{
    Material[] materials;
    MeshRenderer meshRenderer;
    [SerializeField] private float dissolveDuration = 2f;
    [SerializeField] Material accentMaterial;
    Material oldMaterial;
    [SerializeField] Image[] holograms; // n is the center and n + 1 is the border where n % 2 == 0
    [SerializeField] int selectedHologramIndex = -1;
    Transform playerTransform;
    bool doneAppearing = false;
    [SerializeField] GameObject[] keyIndicatorObjects;
    [SerializeField] GameObject upgradeOrbPrefab;
    [SerializeField] GameObject upgradeAppearEffectPrefab;
    [SerializeField] Transform[] upgradeOrbSpawnPoints;
    [SerializeField] GameObject descriptionPanel;
    [SerializeField] TMP_Text descriptionText;
    [SerializeField] TMP_Text nameText;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        materials = meshRenderer.materials;
        foreach (Image hologram in holograms)
        {
            hologram.material.SetFloat("_HologramCutoff", 1f);
        }
        playerTransform = FirstPersonController.Instance.transform;
        StartCoroutine(Appear());
    }

    void Update()
    {
        if (!doneAppearing) return;
        if ((playerTransform.position - transform.position).magnitude < 5f)
        {
            float minDistance = float.MaxValue;
            selectedHologramIndex = -1;
            for (int i = 0; i < holograms.Length; i += 2)
            {
                Vector3 screenPos = Camera.main.WorldToScreenPoint(holograms[i].transform.position);
                Vector2 mousePos = Input.mousePosition;

                float distance = Vector2.Distance(screenPos, mousePos);
                if (distance < 100 && distance < minDistance)
                {
                    minDistance = distance;
                    selectedHologramIndex = i;
                }
            }
            foreach (GameObject keyIndicator in keyIndicatorObjects)
            {
                keyIndicator.SetActive(false);
            }
            if (selectedHologramIndex != -1)
            {
                descriptionPanel.SetActive(true);
                holograms[selectedHologramIndex].rectTransform.localScale = Vector3.Lerp(holograms[selectedHologramIndex].rectTransform.localScale, new Vector3(0.0006f, 0.0006f, 0.0006f), Time.deltaTime * 5f);
                holograms[selectedHologramIndex + 1].rectTransform.localScale = Vector3.Lerp(holograms[selectedHologramIndex].rectTransform.localScale, new Vector3(0.0006f, 0.0006f, 0.0006f), Time.deltaTime * 5f);
                keyIndicatorObjects[selectedHologramIndex / 2].SetActive(true);
                if (Input.GetKeyDown(KeyCode.F))
                {
                    doneAppearing = false;
                    keyIndicatorObjects[selectedHologramIndex / 2].SetActive(false);
                    descriptionPanel.SetActive(false);
                    StartCoroutine(Disappear());
                    return;
                }
            }
            else
            {
                descriptionPanel.SetActive(false);
            }
            for (int i = 0; i < holograms.Length; i++)
            {
                if (i != selectedHologramIndex && i != selectedHologramIndex + 1)
                {
                    holograms[i].rectTransform.localScale = Vector3.Lerp(holograms[i].rectTransform.localScale, new Vector3(0.0004f, 0.0004f, 0.0004f), Time.deltaTime * 5f);
                }
            }
        }
        else
        {
            selectedHologramIndex = -1;
            descriptionPanel.SetActive(false);
            foreach (Image hologram in holograms)
            {
                hologram.rectTransform.localScale = Vector3.Lerp(hologram.rectTransform.localScale, new Vector3(0.0004f, 0.0004f, 0.0004f), Time.deltaTime * 5f);
            }
            foreach (GameObject keyIndicator in keyIndicatorObjects)
            {
                keyIndicator.SetActive(false);
            }
        }
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
        oldMaterial = materials[1];
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
        doneAppearing = true;
    }

    IEnumerator Disappear()
    {
        float elapsedTime = 0f;
        while (elapsedTime < dissolveDuration)
        {
            float t = elapsedTime / dissolveDuration;
            foreach (Image hologram in holograms)
            {
                hologram.material.SetFloat("_HologramCutoff", Mathf.Lerp(0.2f, 1f, t));
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        materials[1] = oldMaterial;
        meshRenderer.materials = materials;
        elapsedTime = 0f;
        Instantiate(upgradeOrbPrefab, upgradeOrbSpawnPoints[selectedHologramIndex/2].position, Quaternion.identity);
        Instantiate(upgradeAppearEffectPrefab, upgradeOrbSpawnPoints[selectedHologramIndex/2].position, Quaternion.identity);
        while (elapsedTime < dissolveDuration)
        {
            float t = elapsedTime / dissolveDuration;
            foreach (Material material in materials)
            {
                material.SetFloat("_CutoffHeight", Mathf.Lerp(transform.position.y + 5, transform.position.y - 5, t));
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }
}
