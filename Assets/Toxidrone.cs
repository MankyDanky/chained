using System.Collections;
using UnityEngine;

public class Toxidrone : Enemy
{
    [SerializeField] GameObject toxicDestroyEffect;
    [SerializeField] float attackRange;
    [SerializeField] GameObject toxicGasPrefab;
    Rigidbody rb;


    protected override void Start()
    {
        rb = GetComponent<Rigidbody>();
        base.Start();
    }
    public override void Attack()
    {

    }

    protected override void Update()
    {
        if (isDead) return;
        transform.LookAt(Vector3.Lerp(transform.position + transform.forward, player.position + Vector3.down * (player.position-transform.position).magnitude, Time.deltaTime * 2f));
        if ((player.position - transform.position).magnitude > attackRange)
        {
            rb.AddForce((player.position - transform.position).normalized * moveSpeed);
        }
        else
        {
            Instantiate(toxicGasPrefab, transform.position - Vector3.up, Quaternion.identity);
            isDead = true;
            Destroy(gameObject);
        }

        base.Update();
    }
    
    protected override void OnDestroy()
    {
        if ((player.position - transform.position).magnitude <= attackRange)
        {
            Instantiate(toxicDestroyEffect, transform.position, Quaternion.identity);
            Instantiate(remains, transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(destroyEffect, transform.position, Quaternion.identity);
            Instantiate(remains, transform.position, Quaternion.identity);
        }
    }

    public override void TakeDamage(float amount, Vector3 hitPoint)
    {
        base.TakeDamage(amount, hitPoint);
        rb.AddForce((transform.position - player.position).normalized * 20f, ForceMode.Impulse);
    }
}
