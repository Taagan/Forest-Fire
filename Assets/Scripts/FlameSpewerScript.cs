using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameSpewerScript : MonoBehaviour
{
    public float attackcd;
    float timer;
    public GameObject projectile;
    public GameObject fireSpawnPoint;


    void Update()
    {
        if (timer >= attackcd)
        {
            Instantiate(projectile, fireSpawnPoint.transform.position, this.transform.rotation);
            timer = 0;
        }
        timer += Time.deltaTime;
    }
}
