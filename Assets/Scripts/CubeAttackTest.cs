using UnityEngine;

public class CubeAttackTest : MonoBehaviour
{

    public int damage = 10;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Attack(GameObject target) { 
        Health health = target.GetComponent<Health>();

        if (health != null)
        {
            health.takeDamage(damage);
        }

        else
        {
            Debug.LogWarning("No Health componend found on " + target.name);
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        GameObject target = collision.gameObject;
        Attack(target);
    }
}
