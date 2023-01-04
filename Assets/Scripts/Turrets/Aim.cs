using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aim : MonoBehaviour
{
    public GameObject target;
    public float AimSpeed;
    Vector3 LastKnownPos = Vector3.zero;
    Quaternion LookAtRotation;
    Quaternion LookAtRotationOnly_XY;

    // Start is called before the first frame update
    void Start()
    {

    }

    //TODO: find closest boid
    private void Update()
    {
        if (LastKnownPos != target.transform.position)
        {
            LastKnownPos = target.transform.position;
            LookAtRotation = Quaternion.LookRotation(LastKnownPos - transform.position);
            LookAtRotationOnly_XY = Quaternion.Euler(LookAtRotation.eulerAngles.x, LookAtRotation.eulerAngles.y, LookAtRotation.eulerAngles.z);
        }
        if (transform.rotation != LookAtRotation)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, LookAtRotationOnly_XY, AimSpeed * Time.deltaTime);
        }

    }
}
