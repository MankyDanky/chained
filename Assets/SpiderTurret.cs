using System.Collections;
using UnityEngine;

public class SpiderTurret : Enemy
{
    [SerializeField] float attackDamage;
    [SerializeField] float attackRange;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Transform[] raycastStarts = new Transform[4];
    [SerializeField] Transform[] targets = new Transform[4];
    Vector3[] positions = new Vector3[4];
    Vector3[] raycastEnds = new Vector3[4];
    bool[] moving = new bool[2];
    int lastMoved = 0;
    [SerializeField] GameObject laserPrefab;
    [SerializeField] Transform laserSpawnPoint;
    bool canShoot = true;
    [SerializeField] float laserCooldown = 1f;


    protected override void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            positions[i] = targets[i].position;
        }
        base.Start();
    }

    public override void Attack()
    {
        // Implement turret attack logic here
        Debug.Log("SpiderTurret attacks!");
    }

    protected override void Update()
    {
        if (isDead) return;
        if ((player.position - transform.position).magnitude > attackRange)
        {
            agent.SetDestination(player.position);
            agent.speed = moveSpeed;
        }
        else
        {
            agent.speed = 0;
            if (canShoot)
            {
                StartCoroutine(ShootLaser());
            }
        }
        for (int i = 0; i < 4; i++)
        {
            targets[i].position = positions[i];
        }
        if (!moving[0] && !moving[1])
        {
            for (int i = 0; i < 4; i++)
            {
                if (Physics.Raycast(raycastStarts[i].position, -raycastStarts[i].up, out RaycastHit hit3, 10f, groundLayer))
                {
                    raycastEnds[i] = hit3.point;
                }
            }
            if (Vector3.Distance(targets[0].position, raycastEnds[0]) > 0.3f && lastMoved != 0)
            {
                StartCoroutine(moveLeg(0));
            }
            else if (Vector3.Distance(targets[1].position, raycastEnds[1]) > 0.3f && lastMoved != 1)
            {
                StartCoroutine(moveLeg(1));
            }
        }

        base.Update();
    }

    IEnumerator moveLeg(int index)
    {
        moving[index] = true;
        Vector3 startPoint1 = targets[index].position;
        Vector3 endPoint1 = raycastEnds[index];
        Vector3 startPoint2 = targets[index + 2].position;
        Vector3 endPoint2 = raycastEnds[index + 2];

        float duration = 0.2f;
        float elapsed = 0;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            Vector3 mid1 = (startPoint1 + endPoint1) / 2 + Vector3.up * 0.1f;
            Vector3 pos1 = Vector3.Lerp(
                Vector3.Lerp(startPoint1, mid1, t),
                Vector3.Lerp(mid1, endPoint1, t),
                t
            );
            positions[index] = pos1;

            Vector3 mid2 = (startPoint2 + endPoint2) / 2 + Vector3.up * 0.1f;
            Vector3 pos2 = Vector3.Lerp(
                Vector3.Lerp(startPoint2, mid2, t),
                Vector3.Lerp(mid2, endPoint2, t),
                t
            );
            positions[index + 2] = pos2;

            yield return null;
        }
        moving[index] = false;
        lastMoved = index;
    }

    IEnumerator ShootLaser()
    {
        GameObject laser = Instantiate(laserPrefab, laserSpawnPoint.position, Quaternion.identity);
        laser.transform.LookAt(player.position);
        canShoot = false;
        yield return new WaitForSeconds(laserCooldown);
        canShoot = true;
    }
}
