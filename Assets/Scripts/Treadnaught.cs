using UnityEngine;

public class Treadnaught : Enemy
{
    [SerializeField] private float stompAttackRange = 5f;
    [SerializeField] Transform stompRightSpawn;
    [SerializeField] Transform stompLeftSpawn;
    [SerializeField] GameObject stompEffectPrefab;
    [SerializeField] float stompDamage = 30f;

    public override void Attack()
    {
        return;
    }
    protected override void Update()
    {
        if (isDead) return;
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance < stompAttackRange)
        {
            animator.SetBool("Stomping", true);
        }
        else
        {
            animator.SetBool("Stomping", false);
        }
        base.Update();
    }

    void StompRight()
    {
        Instantiate(stompEffectPrefab, stompRightSpawn.position, stompRightSpawn.rotation);
        if ((stompRightSpawn.position - player.position).magnitude < 5f)
        {
            playerController.TakeDamage(stompDamage);
            playerController.ApplyImpulse((player.position - stompRightSpawn.position) * 10f);
        }
    }

    void StompLeft()
    {
        Instantiate(stompEffectPrefab, stompLeftSpawn.position, stompLeftSpawn.rotation);
        if ((stompLeftSpawn.position - player.position).magnitude < 5f)
        {
            playerController.TakeDamage(stompDamage);
            playerController.ApplyImpulse((player.position - stompRightSpawn.position) * 10f);
        }
    }
}
