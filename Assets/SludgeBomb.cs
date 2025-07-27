using UnityEngine;

public class SludgeBomb : MonoBehaviour
{
    [SerializeField] GameObject sludgeBombEffect;
    [SerializeField] GameObject toxicSplashPrefab;

    void Start()
    {
        GetComponent<Rigidbody>().angularVelocity = Random.insideUnitSphere * 10f;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy")) return;

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 10f))
        {   
            Vector3 hitPoint = hit.point + Vector3.up * 0.02f;
            Instantiate(toxicSplashPrefab, hitPoint, Quaternion.Euler(90f, 0f, 0f));
            Instantiate(sludgeBombEffect, hitPoint, Quaternion.identity); 
        }
        Destroy(gameObject);
    }
}
