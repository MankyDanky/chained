using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] GameObject destroyEffect;

    void OnCollisionEnter(Collision collision)
    {
        GameObject effect = Instantiate(destroyEffect, transform.position, Quaternion.identity);
        Destroy(effect, 2f);
        Destroy(gameObject);
    }
}
