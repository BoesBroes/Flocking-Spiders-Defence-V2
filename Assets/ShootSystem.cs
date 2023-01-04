using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootSystem : MonoBehaviour
{
    public float fireRate;
    public float fieldOfView;
    public bool beam;
    public GameObject projectile;
    public GameObject target;
    public List<GameObject> projectileSpawns;

    List<GameObject> lastProjectiles = new List<GameObject>();
    float firetimer = 0.0f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //find target
        if (target == null)
        {
            target = GameObject.FindWithTag("Boid");
            this.GetComponent<Aim>().target = target;
        }


        //shoot at right angle when in sight
        if (beam && lastProjectiles.Count <= 0)
        {

            float angle = Quaternion.Angle(transform.rotation, Quaternion.LookRotation(target.transform.position - transform.position));

            if (angle < fieldOfView)
            {
                SpawnProjectiles();
            }

        }

        //move turret if not in sight
        else if (beam && lastProjectiles.Count > 0)
        {
            float angle = Quaternion.Angle(transform.rotation, Quaternion.LookRotation(target.transform.position - transform.position));

            if (angle > fieldOfView)
            {
                while (lastProjectiles.Count > 0)
                {
                    Destroy(lastProjectiles[0]);
                    lastProjectiles.RemoveAt(0);
                }
            }
        }

        //time between shooting, set at firerate in editor
        else
        {
            firetimer += Time.deltaTime;

            if (firetimer >= fireRate)
            {
                float angle = Quaternion.Angle(transform.rotation, Quaternion.LookRotation(target.transform.position - transform.position));

                if (angle < fieldOfView)
                {
                    SpawnProjectiles();

                    firetimer = 0.0f;
                }
            }
        }
    }

    //initiate the projectile
    void SpawnProjectiles()
    {
        if (!projectile)
        {
            return;
        }

        lastProjectiles.Clear();

        for (int i = 0; i < projectileSpawns.Count; i++)
        {
            if (projectileSpawns[i])
            {
                GameObject proj = Instantiate(projectile, projectileSpawns[i].transform.position, Quaternion.Euler(projectileSpawns[i].transform.forward)) as GameObject;
                proj.GetComponent<Projectile>().FireProjectile(projectileSpawns[i], target, 0);

                lastProjectiles.Add(proj);
            }
        }
    }
}
