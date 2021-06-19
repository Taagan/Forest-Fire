using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    public float damage;
    public float speed;
    public float lifetime;
    float switchtimer;
    bool swapped;

    private Rigidbody2D rb;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        switchtimer += Time.deltaTime;
        if (switchtimer >= lifetime/2 && !swapped)
        {
            speed *= -1;
            swapped = true;
        }
        this.transform.Translate(speed, 0, 0);
    }


}
