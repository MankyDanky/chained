using System.Collections;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    protected Transform player;
    protected Animator animator;
    protected RectTransform healthBarFill;
    protected float healthBarWidth;
    protected Canvas healthBarCanvas;
    protected bool isDead = false;
    [SerializeField] protected GameObject destroyEffect;
    [SerializeField] protected GameObject armature;
    [SerializeField] protected GameObject remains;
    [SerializeField] protected GameObject hitEffect;

    public float maxHealth = 100f;
    public float health;
    public float moveSpeed;

    public abstract void Attack();
    public virtual void TakeDamage(float amount, Vector3 hitPoint) {
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

        Destroy(this.gameObject, 1.5f);

    }

    void OnDestroy()
    {
        Instantiate(destroyEffect, transform.position, Quaternion.identity);
        Instantiate(remains, transform.position, Quaternion.identity);
    }

    protected virtual void Start()
    {
        player = FirstPersonController.Instance;
        animator = GetComponent<Animator>();
        healthBarFill = transform.Find("HealthBar/Health")?.GetComponent<RectTransform>();
        healthBarCanvas = transform.Find("HealthBar")?.GetComponent<Canvas>();
        healthBarWidth = healthBarFill.sizeDelta.x;
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
    }

    protected void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            float healthPercent = Mathf.Clamp01(health / maxHealth);
            healthBarFill.sizeDelta = new Vector2(healthBarWidth * healthPercent, healthBarFill.sizeDelta.y);
        }
    }
}