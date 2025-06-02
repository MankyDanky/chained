using UnityEngine;

public class Shambler : Enemy
{

    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] GameObject kickSound;

    protected override void Start()
    {
        base.Start();
    }

    public override void Attack()
    {

    }

    protected override void Update()
    {
        base.Update();
        if ((player.position - transform.position).magnitude > attackRange)
        {
            if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Run")
            {
                agent.SetDestination(player.position);
                agent.speed = moveSpeed;
            }
            animator.SetBool("isAttacking", false);
        }
        else
        {
            agent.speed = 0;
            animator.SetBool("isAttacking", true);
            transform.LookAt(Vector3.Lerp(transform.position + transform.forward, player.position, Time.deltaTime * 2f));
        }
    }

    void KickPlayer()
    {
        if ((player.position - transform.position).magnitude < attackRange)
        {
            playerController.TakeDamage(attackDamage);
            Instantiate(kickSound, transform.position, Quaternion.identity);
        }
    }
}
