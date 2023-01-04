using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Behaviour : MonoBehaviour
{
    public Controller controllerAgent;

    private float noiseOffset;

    private int nearbyLength;

    //KEEP NAVMESHAGENT TO KEEP ON THE GROUND
    //oops caps 

    Vector3 GetSeparationVector(Transform target)
    {
        var diff = transform.position - controllerAgent.transform.position;
        var diffLen = diff.magnitude;
        var scaler = Mathf.Clamp01(1.0f - diffLen / controllerAgent.neighborDist);
        return diff * (scaler / diffLen);
    }

    Vector3 GetSeparationVectorBoid(Transform target)
    {
        var diff = transform.position - target.transform.position;
        var diffLen = diff.magnitude;
        var scaler = Mathf.Clamp01(1.0f - diffLen / controllerAgent.neighborDist);
        return diff * (scaler / diffLen);
    }

    // Start is called before the first frame update
    void Start()
    {
        noiseOffset = Random.value * 10.0f;

        var animator = GetComponent<Animator>();
        if (animator)
            animator.speed = Random.Range(-1.0f, 1.0f) + 1.0f;
    }

    void Update()
    {
        var currentPosition = transform.position;
        var currentRotation = transform.rotation;

        // Current velocity randomized with noise.
        var noise = Mathf.PerlinNoise(Time.time, noiseOffset) * 2.0f - 1.0f;
        var velocity = controllerAgent.velocity * (1.0f + noise * controllerAgent.velocityVariation);

        // Initializes the vectors.
        var separation = Vector3.zero;
        var alignment = controllerAgent.transform.forward;
        var cohesion = controllerAgent.transform.position;

        // Looks up nearby boids.
        var nearbyBoids = Physics.OverlapSphere(currentPosition, controllerAgent.neighborDist, controllerAgent.searchLayer);

        nearbyLength = 0;

        //Accumulates the vectors.
        foreach (var boid in nearbyBoids)
        {
            if (boid.tag == "Boid")
            {
                var t = controllerAgent.transform;
                separation += GetSeparationVector(t);
                nearbyLength++;


                //alignment += t.forward;
                //cohesion += t.position;

                var r = boid.transform;
                if (GetSeparationVectorBoid(r).x > 0 && GetSeparationVectorBoid(r).z > 0)
                {
                    alignment -= r.forward;
                    cohesion -= r.position;
                }
                else
                {
                    alignment += t.forward;
                    cohesion += t.position;
                }
            }
            else
            {
            }
        }

        var avg = 1.0f / nearbyLength;
        alignment *= avg;
        cohesion *= avg;
        cohesion = (cohesion - currentPosition).normalized;

        // Calculates a rotation from the vectors.
        var direction = separation + alignment + cohesion;
        var rotation = Quaternion.FromToRotation(Vector3.forward, direction.normalized);

        // Applys the rotation with interpolation.
        if (rotation != currentRotation)
        {
            var ip = Mathf.Exp(-controllerAgent.rotationCoeff * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(rotation, currentRotation, ip);
        }

        // Moves forawrd.
        transform.position = currentPosition + transform.forward * (velocity * Time.deltaTime);
    }
}
