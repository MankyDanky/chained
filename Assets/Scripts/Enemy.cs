using System.Collections;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    protected Transform player;
    protected Animator animator;
    protected RectTransform healthBarFill;
    protected Canvas healthBarCanvas;
    protected bool isDead = false;
    [SerializeField] protected GameObject destroyEffect;
    [SerializeField] protected GameObject armature;
    [SerializeField] protected GameObject remains;

    public float maxHealth = 100f;
    public float health;
    public float moveSpeed;

    public abstract void Attack();
    public abstract void TakeDamage(float amount);
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
    }

    protected virtual void Update()
    {
        UpdateHealthBarVisibility();
    }

    protected void UpdateHealthBarVisibility()
    {
        Debug.Log("Updating health bar visibility");
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        Vector2 mousePos = Input.mousePosition;

        float distanceToMouse = Vector2.Distance(screenPos, mousePos);
        float visibilityThreshold = 100f;

        healthBarCanvas.enabled = distanceToMouse < visibilityThreshold;
    }

    protected void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            float healthPercent = Mathf.Clamp01(health / maxHealth);
            healthBarFill.localScale = new Vector3(healthPercent, 1f, 1f);
        }
    }
}