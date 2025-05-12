using UnityEngine;

public class Brute : Enemy
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
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
        if (stepping)
        {
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        } else if (Vector3.Distance(transform.position, player.position) < attackRange)
        {
            Debug.Log("Grunt is in range to attack!");
            animator.SetBool("isWalking", false);
            animator.SetTrigger("attack");
        }
        else
        {
            Debug.Log("Grunt is moving towards player!");
            animator.SetBool("isWalking", true);
            animator.ResetTrigger("attack");
            return;
        }
    }

    private void StartStep() {
        stepping = true;
    }

    private void StopStep() {
        stepping = false;
    }
}