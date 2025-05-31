using System.Collections;
using UnityEngine;

public class Water : MonoBehaviour
{
    [SerializeField] private float hurtDelay = 1f;
    bool playerInWater = false;
    bool canHurt = true;
    Transform player;
    FirstPersonController playerController;

    void Start()
    {
        player = FirstPersonController.Instance;
        playerController = player.GetComponent<FirstPersonController>();
    }

    void Update()
    {
        if (playerInWater && canHurt)
        {
            StartCoroutine(HurtPlayer());
        }
    }

    IEnumerator HurtPlayer()
    {
        playerController.TakeDamage(10f);
        canHurt = false;
        yield return new WaitForSeconds(hurtDelay);
        canHurt = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInWater = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInWater = false;
        }
    }
}
