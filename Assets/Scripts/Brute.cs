using UnityEngine;

public class Brute : Enemy
{
    public float attackDamage;
    public float attackRange;
    bool stepping = false;

    protected override void Update()
    {
        if (isDead) return;
        Move();
        base.Update();
    }

    public override void Attack()
    {
        Debug.Log("Grunt attacks with " + attackDamage + " damage!");
    }

    public override void TakeDamage(float amount)
    {
        
        base.TakeDamage(amount);
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
            animator.SetBool("isWalking", false);
            animator.SetTrigger("attack");
        }
        else
        {
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