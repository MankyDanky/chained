using System.Collections;
using UnityEngine;

public class SpiderTurret : Enemy
{

    [SerializeField] LayerMask groundLayer;
    [SerializeField] Transform frontLeftRaycastStart;
    [SerializeField] Transform frontLeftTarget;
    [SerializeField] Vector3 frontLeftPosition;
    Vector3 frontLeftRaycastEnd;
    bool frontLeftMoving = false;
    [SerializeField] Transform frontRightRaycastStart;
    [SerializeField] Transform frontRightTarget;
    [SerializeField] Vector3 frontRightPosition;
    Vector3 frontRightRaycastEnd;
    bool frontRightMoving = false;


    protected override void Start()
    {
        frontRightPosition = frontRightTarget.position;
        frontLeftPosition = frontLeftTarget.position;
        base.Start();
    }

    public override void Attack()
    {
        // Implement turret attack logic here
        Debug.Log("SpiderTurret attacks!");
    }

    protected override void Update()
    {
        frontLeftTarget.position = frontLeftPosition;
        if (!frontLeftMoving)
        {
            if (Physics.Raycast(frontLeftRaycastStart.position, -frontLeftRaycastStart.up, out RaycastHit hit, 10f, groundLayer))
            {
                frontLeftRaycastEnd = hit.point;
            }
            if (Vector3.Distance(frontLeftTarget.position, frontLeftRaycastEnd) > 0.3f)
            {
                StartCoroutine(moveFrontLeft());
            }
        }
        frontRightTarget.position = frontRightPosition;
        if (!frontRightMoving)
        {
            if (Physics.Raycast(frontRightRaycastStart.position, -frontRightRaycastStart.up, out RaycastHit hit, 10f, groundLayer))
            {
                frontRightRaycastEnd = hit.point;
            }
            if (Vector3.Distance(frontRightTarget.position, frontRightRaycastEnd) > 0.3f)
            {
                StartCoroutine(moveFrontRight());
            }
        }

        base.Update();
    }

    IEnumerator moveFrontLeft()
    {
        frontLeftMoving = true;
        while (Vector3.Distance(frontLeftTarget.position, frontLeftRaycastEnd) > 0.01f)
        {
            frontLeftPosition = Vector3.MoveTowards(frontLeftTarget.position, frontLeftRaycastEnd, Time.deltaTime * 2f);
            yield return null;
        }
        yield return new WaitForSeconds(0.35f);
        frontLeftMoving = false;
    }

    IEnumerator moveFrontRight()
    {
        frontRightMoving = true;
        while (Vector3.Distance(frontRightTarget.position, frontRightRaycastEnd) > 0.01f)
        {
            frontRightPosition = Vector3.MoveTowards(frontRightTarget.position, frontRightRaycastEnd, Time.deltaTime * 2f);
            yield return null;
        }
        yield return new WaitForSeconds(0.35f);
        frontRightMoving = false;
    }
}
