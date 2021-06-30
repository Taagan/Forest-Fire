using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HittableScript : MonoBehaviour
{
    public int hp = 1;
    public bool trigger;
    public bool isEnemy;

    public void TakeDamage(int damage)
    {
        if (isEnemy && !trigger)
        {
            hp -= damage;
            DeathCheck();
        }
    }


    public void ActiveTrigger()
    {
        if (!trigger)
            trigger = true;
        //else
        //    trigger = false;
        //Do not need this atm can uncomment if needed
    }

    private void DeathCheck()
    {
        if (hp <= 0 && isEnemy)
            Destroy(gameObject);
    }
}
