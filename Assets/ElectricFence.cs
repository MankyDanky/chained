using UnityEngine;

public class ElectricFence : MonoBehaviour
{

    public GameObject electricEffect;

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("ElectricFence: Collision with " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("Player"))
        {
            FirstPersonController playerController = collision.gameObject.GetComponent<FirstPersonController>();
            playerController.TakeDamage(10f);
            playerController.ApplyImpulse(Vector3.up * 10f + transform.right * -20f);
            Instantiate(electricEffect, transform.position + transform.forward * 2.2f + Vector3.up * 1.5f, Quaternion.identity);
        }
    }
}
