using UnityEngine;

public class DamageArea : MonoBehaviour
{

    [SerializeField] float damageAmount = 10f;
    [SerializeField] float damageInterval = 1f;
    [SerializeField] float damageRange = 5f;
    [SerializeField] float damageTime = 6f;
    float timer;
    float totalTimer;
    FirstPersonController playerController;

    protected virtual void Start()
    {
        playerController = FirstPersonController.Instance.GetComponent<FirstPersonController>();
    }

    protected virtual void Update()
    {
        if (totalTimer > damageTime) return;
        totalTimer += Time.deltaTime;
        if (timer < damageInterval)
        {
            timer += Time.deltaTime;
        }
        else
        {
            if ((transform.position - playerController.transform.position).magnitude <= damageRange)
            {
                playerController.TakeDamage(damageAmount);
                timer = 0f;
            }
        }
    }
}
