using UnityEngine;

public class Grunt : Enemy
{
    public float attackDamage;
    public float attackRange;
    bool stepping = false;

    private void Update()
    {
        Move();
    }

    public override void Attack()
    {
        Debug.Log("Grunt attacks with " + attackDamage + " damage!");
    }

    public override void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Die();
        }
    }

    public override void Die()
    {
        base.Die(); 
    }

    private void Move()
    {
        if (stepping)
        {
            transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }
        animator.SetBool("isWalking", true);
    }

    private void StartStep() {
        stepping = true;
    }

    private void StopStep() {
        stepping = false;
    }
}