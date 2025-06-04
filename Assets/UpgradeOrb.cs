using System.Collections;
using UnityEngine;

public class UpgradeOrb : MonoBehaviour
{
    Rigidbody rb;
    Transform playerTransform;
    bool moving = false;
    [SerializeField] GameObject upgradeEffectPrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(Vector3.up * 3f, ForceMode.Impulse);
        playerTransform = FirstPersonController.Instance.transform;
        StartCoroutine(StartMoving());
    }

    // Update is called once per frame
    void Update()
    {
        if (!moving) return;
        rb.AddForce((playerTransform.position - transform.position).normalized * 2f, ForceMode.Force);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Instantiate(upgradeEffectPrefab, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }

    IEnumerator StartMoving()
    {
        yield return new WaitForSeconds(0.5f);
        moving = true;
    }
}
