using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : MonoBehaviour
{
    protected Transform player;
    protected FirstPersonController playerController;
    protected Animator animator;
    protected RectTransform healthBarFill;
    protected float healthBarWidth;
    protected Canvas healthBarCanvas;
    protected RectTransform healthBarIndicatorFill;
    protected float healthBarIndicatorWidth;
    protected float healthBarIndicatorTimer = 0f;
    protected bool isDead = false;
    [SerializeField] protected GameObject destroyEffect;
    [SerializeField] protected GameObject remains;
    [SerializeField] protected GameObject hitEffect;
    [SerializeField] float deathDelay = 1.5f;
    protected NavMeshAgent agent;

    public float maxHealth = 100f;
    public float health;
    public float moveSpeed;

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
        Instantiate(remains, transform.position, Quaternion.identity);
    }

    protected virtual void Start()
    {
        player = FirstPersonController.Instance;
        playerController = player.GetComponent<FirstPersonController>();
        animator = GetComponent<Animator>();
        healthBarFill = transform.Find("HealthBar/Health")?.GetComponent<RectTransform>();
        healthBarIndicatorFill = transform.Find("HealthBar/Indicator")?.GetComponent<RectTransform>();
        healthBarCanvas = transform.Find("HealthBar")?.GetComponent<Canvas>();
        healthBarWidth = healthBarFill.sizeDelta.x;
        agent = GetComponent<NavMeshAgent>();
    }

    protected virtual void Update()
    {
        UpdateHealthBarVisibility();
    }

    protected void UpdateHealthBarVisibility()
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

    protected void UpdateHealthBar()
    {
        float healthPercent = Mathf.Clamp01(health / maxHealth);
        healthBarFill.sizeDelta = new Vector2(healthBarWidth * healthPercent, healthBarFill.sizeDelta.y);
        healthBarIndicatorTimer = 0.0f;
        healthBarIndicatorWidth = healthBarIndicatorFill.sizeDelta.x;
    }
}