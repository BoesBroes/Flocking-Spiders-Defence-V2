using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingAnimation : MonoBehaviour
{
    public Transform[] legTargets;
    public float stepDistance;
    public float stepHeight;
    public int smoothness;
    public float castRadius;
    public bool bodyOrientation;

    public float raycastRange;
    private Vector3[] defaultLegPosition;
    private Vector3[] lastLegPosition;
    private Vector3 lastBodyUp;
    private bool[] legMoving;
    private int spiderLegs;

    private Vector3 velocity;
    private Vector3 lastVelocity;
    private Vector3 lastBodyPos;

    private float velocityMultiplier = 20f;

    Vector3[] MatchToSurfaceFromAbove(Vector3 point, float halfRange, Vector3 up)
    {
        Vector3[] res = new Vector3[2];
        res[1] = Vector3.zero;
        RaycastHit hit;
        Ray ray = new Ray(point + halfRange * up / 2f, -up);

        if (Physics.SphereCast(ray, castRadius, out hit, 2f * halfRange))
        {
            res[0] = hit.point;
            res[1] = hit.normal;
        }
        else
        {
            res[0] = point;
        }
        return res;
    }

    // Start is called before the first frame update
    void Start()
    {
        spiderLegs = legTargets.Length;
        defaultLegPosition = new Vector3[spiderLegs];
        lastLegPosition = new Vector3[spiderLegs];
        legMoving = new bool[spiderLegs];
        for(int i = 0; i < spiderLegs; i++)
        {
            defaultLegPosition[i] = legTargets[i].localPosition;
            lastLegPosition[i] = legTargets[i].position;
            legMoving[i] = false;
        }
        lastBodyPos = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        velocity = transform.position - lastBodyPos;
        velocity = (velocity + smoothness * lastVelocity) / (smoothness +1f);

        if(velocity.magnitude < 0.00005)
        {
            velocity = lastVelocity;
        }
        else
        {
            lastVelocity = velocity;
        }

        Vector3[] desiredPositions = new Vector3[spiderLegs];
        int indexToMove = -1;
        float maxDistance = stepDistance;

        for(int i = 0; i < spiderLegs; i++)
        {
            desiredPositions[i] = transform.TransformPoint(defaultLegPosition[i]);

            float distance = Vector3.ProjectOnPlane(desiredPositions[i] + velocity * velocityMultiplier - lastLegPosition[i], transform.up).magnitude;
            if(distance > maxDistance)
            {
                maxDistance = distance;
                indexToMove = i;
            }
        }

        for(int i = 0; i < spiderLegs; i++)
        {
            if(i != indexToMove)
            {
                legTargets[i].position = lastLegPosition[i];
            }
        }

        if (indexToMove != -1 && !legMoving[0])
        {
            Vector3 targetPoint = desiredPositions[indexToMove] + Mathf.Clamp(velocity.magnitude * velocityMultiplier, 0.0f, 1.5f) * (desiredPositions[indexToMove] - legTargets[indexToMove].position) + velocity * velocityMultiplier;

            Vector3[] positionAndNormalFwd = MatchToSurfaceFromAbove(targetPoint + velocity * velocityMultiplier, raycastRange, (transform.parent.up - velocity * 100).normalized);
            Vector3[] positionAndNormalBwd = MatchToSurfaceFromAbove(targetPoint + velocity * velocityMultiplier, raycastRange * (1f + velocity.magnitude), (transform.parent.up + velocity * 75).normalized);

            legMoving[0] = true;

            if (positionAndNormalFwd[1] == Vector3.zero)
            {
                StartCoroutine(PerformStep(indexToMove, positionAndNormalBwd[0]));
            }
            else
            {
                StartCoroutine(PerformStep(indexToMove, positionAndNormalFwd[0]));
            }
        }

        lastBodyPos = transform.position;
        if (spiderLegs > 3 && bodyOrientation)
        {
            Vector3 v1 = legTargets[0].position - legTargets[1].position;
            Vector3 v2 = legTargets[2].position - legTargets[3].position;
            Vector3 normal = Vector3.Cross(v1, v2).normalized;
            Vector3 up = Vector3.Lerp(lastBodyUp, normal, 1f / (float)(smoothness + 1));
            transform.up = up;
            transform.rotation = Quaternion.LookRotation(transform.parent.forward, up);
            lastBodyUp = transform.up;
        }
    }

    IEnumerator PerformStep(int index, Vector3 targetPoint)
    {
        Vector3 startPos = lastLegPosition[index];
        for (int i = 1; i <= smoothness; ++i)
        {
            legTargets[index].position = Vector3.Lerp(startPos, targetPoint, i / (float)(smoothness + 1f));
            legTargets[index].position += transform.up * Mathf.Sin(i / (float)(smoothness + 1f) * Mathf.PI) * stepHeight;
            yield return new WaitForFixedUpdate();
        }
        legTargets[index].position = targetPoint;
        lastLegPosition[index] = legTargets[index].position;
        legMoving[0] = false;
    }

}
