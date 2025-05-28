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
        while (Vector3.Distance(targets[index].position, raycastEnds[index]) > 0.01f ||
               Vector3.Distance(targets[index + 2].position, raycastEnds[index + 2]) > 0.01f)
        {
            positions[index] = Vector3.MoveTowards(targets[index].position, raycastEnds[index], Time.deltaTime * 4f);
            positions[index + 2] = Vector3.MoveTowards(targets[index + 2].position, raycastEnds[index + 2], Time.deltaTime * 4f);
            yield return null;
        }
        moving[index] = false;
        lastMoved = index;
    }
}
