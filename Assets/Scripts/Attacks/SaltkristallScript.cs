using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaltkristallScript : MonoBehaviour
{
    Vector2 velocity;
    LayerMask layerMask;
    float lifeTime = 1000;

    float freezeTime = .1f;
    bool dead = false;

    public void Init(Vector2 position, Vector2 velocity, float lifeTime, LayerMask layerMask)
    {
        transform.position = position;
        this.velocity = velocity;
        this.lifeTime = lifeTime;
        this.layerMask = layerMask;
    }

    // Update is called once per frame
    void Update()
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
            Expire();

        if (dead)
            return;

        Vector2 moveBy = velocity * Time.deltaTime;

        //Gör raycast här, om träffar kalla Hit(collider);
        //eller kanske bara ska köra med vanlig hit-detection, då behöver detta föremålet dock en rigidbody, kan fixa snart
        //äh.. raycast än så länge
        
        //raycast
        RaycastHit2D hit = Physics2D.Raycast(transform.position, moveBy.normalized, moveBy.magnitude, layerMask);
        if (hit)
        {
            moveBy = moveBy * hit.distance;//ÄNDRA SÅ STANNAR I HIT()
        }

        //move
        transform.Translate(moveBy);

        //här efter för att den ska flytta sig innan den säger att den träffar, kanske.. tror egentligen inte att det gör någon visuell skillnad för spelaren
        if(hit)
            Hit(hit.collider);
    }

    void Expire()
    {
        //jag vet inte, gör kanske nåt mer fancy här? nån fin animation kanske. Skulle kunna få plats nya features här också..
        velocity = Vector2.zero;
        dead = true;
        Destroy(gameObject, freezeTime);
    }

    public void Hit(Collider2D collider)
    {
        if (collider.CompareTag("Enemy"))
        {
            Debug.Log("Hit enemy");
            collider.GetComponent<HittableScript>().TakeDamage(1);
            collider.GetComponent<HittableScript>().ActiveTrigger();
        }
        else
        {
            Debug.Log("Hit ground, tror jag...");
            Expire();
        }

    }

}
