using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum GoombaType { Walking, Flying }

public class goombaScript : MonoBehaviour
{
    private WalkerMovementScript Movement;
    public GoombaType type;
    public float speed;
    public int damage;
    public int hp;
    private float flyingTimer;

    [SerializeField]
    private float flyingCD = 5;

    private void Start()
    {
        Movement = GetComponent<WalkerMovementScript>();
        if (type == GoombaType.Flying)
        {
            flyingTimer = flyingCD;
            Movement.gravityConstant = speed;
        }
    }

    public void Damage(int dmg)
    {
        hp -= dmg;
        DeathCheck();
    }

    private void DeathCheck()
    {
        if (hp <= 0)
            Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        MovementType();
    }

    private void MovementType()
    {
        if (type == GoombaType.Walking)
        {
            Movement.SetHorizontalVelocity(-speed);
        }
        else if (type == GoombaType.Flying)
        {
            flyingTimer += Time.deltaTime;
            if (flyingTimer >= flyingCD)
            {
                Movement.SetVerticalVelocity(speed);
                flyingTimer -= flyingTimer;
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (Movement.collisions.left && !CompareTag("Player") || (Movement.collisions.right) && !CompareTag("Player"))
        {
            Debug.Log("b");
            speed *= -1;
            transform.Rotate(0, 180f, 0, Space.Self);
        }
        if (CompareTag("Player"))
        {
            //collision.gameObject.GetComponent<PlayerScript>().TakeDamage(damage);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (CompareTag("PlayerAttack"))
        {
            Debug.Log("attacked");
            //collision.gameObject.GetComponent<SaltkristallScript>()
            //^ use something in there later
            Destroy(gameObject);
        }
    }
}
