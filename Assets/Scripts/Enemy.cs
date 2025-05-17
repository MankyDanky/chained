using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    protected Transform player;
    protected Animator animator;
    protected RectTransform healthBarFill;
    protected Canvas healthBarCanvas;

    public float maxHealth = 100f;
    public float health;
    public float moveSpeed;

    public abstract void Attack();
    public abstract void TakeDamage(float amount);
    public virtual void Die()
    {
        Destroy(gameObject);
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