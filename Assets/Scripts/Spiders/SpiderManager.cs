using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderManager : MonoBehaviour
{
    public float health = 100;
    private float startHealth;
    public GameObject body;

    // Start is called before the first frame update
    void Start()
    {
        body.GetComponent<MeshRenderer>().material.color = Color.green;
        startHealth = health;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag == "projectile")
        {
            //TODO: do damage based on projectile
            health -= 10;
            if(health > 0)
            {
                ChangeColor();
            }
            else
            {
                //TODO: probably add score
                Destroy(this);
            }
        }
    }

    private void ChangeColor()
    {
        float newColor = (health / startHealth);

        Debug.Log(newColor);

        Color damageColor = new Color(1 - newColor, newColor, 0, 1);

        Debug.Log(damageColor);

        body.GetComponent<MeshRenderer>().material.color = damageColor;
    }    
}
