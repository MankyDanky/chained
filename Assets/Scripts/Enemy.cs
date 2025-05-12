using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    protected Transform player;
    protected Animator animator;

    public float health;
    public float moveSpeed;

    public abstract void Attack();
    public abstract void TakeDamage(float amount);
    public virtual void Die() {
        Destroy(gameObject);
    }

    protected virtual void Start()
    {
        player = FirstPersonController.Instance;
        animator = GetComponent<Animator>();
    }
}