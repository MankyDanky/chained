using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public abstract class Enemy : MonoBehaviour
{
    protected Transform player;
    protected FirstPersonController playerController;
    protected Animator animator;
    protected RectTransform healthBarFill;
    protected float healthBarWidth;
    protected Canvas healthBarCanvas;
    protected RectTransform healthBarIndicatorFill;
    protected Image healthBarFillImage;
    protected Image healthBarIndicatorFillImage;
    protected float healthBarIndicatorWidth;
    protected float healthBarIndicatorTimer = 0f;
    protected bool isDead = false;
    [SerializeField] protected GameObject destroyEffect;
    [SerializeField] protected GameObject remains;
    [SerializeField] protected GameObject hitEffect;
    [SerializeField] float deathDelay = 1.5f;
    protected NavMeshAgent agent;
    [SerializeField] bool isBoss = false;
    TMP_Text bossHealthText;
    TMP_Text bossNameText;

    public float maxHealth = 100f;
    public float health;
    public float moveSpeed;
    


    // Spawning
    [SerializeField] List<Material> spawningMaterials;
    [SerializeField] List<Material> spawnedMaterials;
    [SerializeField] float fadeInDuration = 0.5f;
    protected bool fadingIn = true;

    IEnumerator FadeIn()
    {
        Dictionary<Material, Material> preMap = new Dictionary<Material, Material>();
        Dictionary<Material, Material> postMap = new Dictionary<Material, Material>();
        for (int i = 0; i < spawningMaterials.Count; i++)
        {
            Material spawnedMaterial = spawnedMaterials[i];
            Material spawningMaterial = spawningMaterials[i];
            preMap.Add(spawnedMaterial, spawningMaterial);
            postMap.Add(spawningMaterial, spawnedMaterial);
        }
        foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
        {
            Material[] materials = renderer.materials;
            for (int i = 0; i < materials.Length; i++)
            {
                for (int j = 0; j < spawnedMaterials.Count; j++)
                {
                    if (materials[i].name.Split(' ')[0] == spawnedMaterials[j].name)
                    {
                        materials[i] = spawningMaterials[j];
                    }
                }
            }
            renderer.materials = materials;
        }
        float elapsedTime = 0f;
        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / fadeInDuration);
            foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
            {
                Material[] materials = renderer.materials;
                for (int i = 0; i < materials.Length; i++)
                {
                    materials[i].SetFloat("_CutoffHeight", Mathf.Lerp(renderer.transform.position.y - 2, renderer.transform.position.y + 2, t));
                }
                renderer.materials = materials;
            }
            yield return null;
        }
        foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
        {
            Material[] materials = renderer.materials;
            for (int i = 0; i < materials.Length; i++)
            {
                for (int j = 0; j < spawnedMaterials.Count; j++)
                {
                    if (materials[i].name.Split(' ')[0] == spawningMaterials[j].name)
                    {
                        materials[i] = spawnedMaterials[j];
                    }
                }
            }
            renderer.materials = materials;
        }
        this.GetComponent<Outline>().enabled = true;
        fadingIn = false;
    }
    

    public abstract void Attack();
    public virtual void TakeDamage(float amount, Vector3 hitPoint)
    {
        GameObject effect = Instantiate(hitEffect, hitPoint, Quaternion.identity);
        effect.transform.LookAt(player.position);
        health -= amount;
        UpdateHealthBar();
        if (health <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        isDead = true;
        animator.SetBool("isDead", true);

        Destroy(this.gameObject, deathDelay);

    }

    protected virtual void OnDestroy()
    {
        Instantiate(destroyEffect, transform.position, Quaternion.identity);
        Instantiate(remains, transform.position, transform.rotation);
    }

    protected virtual void Start()
    {
        player = FirstPersonController.Instance;
        playerController = player.GetComponent<FirstPersonController>();
        animator = GetComponent<Animator>();
        if (!isBoss)
        {
            healthBarFill = transform.Find("HealthBar/Health")?.GetComponent<RectTransform>();
            healthBarIndicatorFill = transform.Find("HealthBar/Indicator")?.GetComponent<RectTransform>();
            healthBarCanvas = transform.Find("HealthBar")?.GetComponent<Canvas>();
            healthBarWidth = healthBarFill.sizeDelta.x;
        }
        else
        {
            GameObject bossHealthBar = GameObject.Find("Canvas").transform.Find("BossHealthBar")?.gameObject;
            bossHealthBar.SetActive(true);
            bossHealthText = bossHealthBar.transform.Find("HealthText")?.GetComponent<TMP_Text>();
            bossNameText = bossHealthBar.transform.Find("NameText")?.GetComponent<TMP_Text>();
            bossNameText.text = gameObject.name;
            healthBarFill = bossHealthBar.transform.Find("Health")?.GetComponent<RectTransform>();
            healthBarIndicatorFill = bossHealthBar.transform.Find("Indicator")?.GetComponent<RectTransform>();
            healthBarFillImage = healthBarFill.GetComponent<Image>();
            healthBarIndicatorFillImage = healthBarIndicatorFill.GetComponent<Image>();
        }
        StartCoroutine(FadeIn());
        agent = GetComponent<NavMeshAgent>();
    }

    protected virtual void Update()
    {
        UpdateHealthBarVisibility();

    }

    protected void UpdateHealthBarVisibility()
    {
        if (!isBoss)
        {

            Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
            Vector2 mousePos = Input.mousePosition;

            float distanceToMouse = Vector2.Distance(screenPos, mousePos);
            float visibilityThreshold = 100f;
            healthBarCanvas.enabled = distanceToMouse < visibilityThreshold && (transform.position - player.position).magnitude < 30f;


            healthBarIndicatorTimer += Time.deltaTime;
            healthBarIndicatorTimer = Mathf.Clamp01(healthBarIndicatorTimer);
            healthBarIndicatorFill.sizeDelta = new Vector2(Mathf.Lerp(healthBarIndicatorWidth, healthBarFill.sizeDelta.x, healthBarIndicatorTimer), healthBarIndicatorFill.sizeDelta.y);
        }
        else
        {
            bossHealthText.text = $"{Mathf.CeilToInt(health)} / {Mathf.CeilToInt(maxHealth)}";
            healthBarIndicatorTimer += Time.deltaTime;
            healthBarIndicatorTimer = Mathf.Clamp01(healthBarIndicatorTimer);
            healthBarIndicatorFillImage.fillAmount = Mathf.Lerp(healthBarIndicatorWidth, healthBarFillImage.fillAmount, healthBarIndicatorTimer);
        }
    }

    protected void UpdateHealthBar()
    {
        if (!isBoss)
        {
            float healthPercent = Mathf.Clamp01(health / maxHealth);
            healthBarFill.sizeDelta = new Vector2(healthBarWidth * healthPercent, healthBarFill.sizeDelta.y);
            healthBarIndicatorTimer = 0.0f;
            healthBarIndicatorWidth = healthBarIndicatorFill.sizeDelta.x;
        }
        else
        {
            float healthPercent = Mathf.Clamp01(health / maxHealth);
            healthBarFillImage.fillAmount = healthPercent;
            healthBarIndicatorTimer = 0.0f;
            healthBarIndicatorWidth = healthBarIndicatorFillImage.fillAmount;
        }
        
    }
}