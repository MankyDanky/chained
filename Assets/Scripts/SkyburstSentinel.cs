using System.Collections;
using UnityEngine;

public class SkyburstSentinel : Enemy
{
    Rigidbody rb;
    bool flying = true;
    Transform player;
    [SerializeField] ParticleSystem[] thrusterEffects;

    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody>();
        player = FirstPersonController.Instance;
        flying = true;
        rb.useGravity = false;
        foreach (ParticleSystem ps in thrusterEffects)
        {
            ps.gameObject.SetActive(true);
            ps.Play();
        }
    }

    public override void Attack()
    {
        return;
    }
    protected override void Update()
    {
        base.Update();
        if (isDead || fadingIn) return;
        if (flying)
        {
            rb.linearVelocity = (new Vector3(player.position.x, transform.position.y, player.position.z) - transform.position).normalized * 5f;
        }
    }   

    public override void Die()
    {
        isDead = true;
        base.Die();
    }
}
