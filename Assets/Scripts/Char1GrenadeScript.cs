using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Char1GrenadeScript : MonoBehaviour
{
    public int damage = 30;
    public float explosionDelay = 2f;
    public Collider explosionTrigger;
    private bool hasExploded = false;

    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {


        Health health = other.GetComponent<Health>();

        if (health != null)
        {
            health.takeDamage(damage);
        }

        else
        {
            Debug.LogWarning("No Health componend found on " + other.name);
        }
    }

    

    public IEnumerator ExplosionStart()
    {
        hasExploded = true;
        yield return new WaitForSeconds(explosionDelay);
        if (explosionTrigger != null) {
            explosionTrigger.enabled = true;
            Debug.Log("trigger enabled");
        }
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }
}
