using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Vector3 Direction;
    bool Fired;

    public float speed;

    GameObject Score;

    // Start is called before the first frame update
    void Start()
    {
        //destroy gameobject if after x time if nothing happens to it
        StartCoroutine(KillTimer());

    }

    // Update is called once per frame
    //direction and speed 
    void Update()
    {
        if (Fired)
        {
            transform.position += Direction * (speed * Time.deltaTime);
        }
    }

    //find direction of target
    public void FireProjectile(GameObject Launcher, GameObject Target, int Damage)
    {
        if (Launcher && Target)
        {
            Direction = (Target.transform.position - Launcher.transform.position).normalized;
            Fired = true;
        }
    }

    IEnumerator KillTimer()
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);

    }

    //what happens if ground or player gets hit
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Boid")
        {
            Destroy(gameObject);
        }
        else if (collision.gameObject.tag == "Ground")
        {
            Destroy(gameObject);
        }
    }
}
