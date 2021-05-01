using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    public float damage;
    public float speed;
    public float lifetime;


    private Rigidbody2D rb;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        this.transform.Translate(speed, 0, 0);
    }


}
