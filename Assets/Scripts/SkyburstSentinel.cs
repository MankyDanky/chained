using System.Collections;
using UnityEngine;

public class SkyburstSentinel : Enemy
{
    Rigidbody rb;

    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody>();
    }

    public override void Attack()
    {
        return;
    }
    protected override void Update()
    {
        base.Update();
    }

    public override void Die()
    {
        isDead = true;
        base.Die();
    }
}
