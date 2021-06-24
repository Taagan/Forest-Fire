using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum GoombaType { Walking, Flying, Stationary}

public class goombaScript : HittableScript
{
    private WalkerMovementScript Movement;
    public GoombaType type;
    public float speed;
    public int damage;
    private float flyingTimer;
    private GameObject player;

    [SerializeField]
    private float flyingCD = 5;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (type != GoombaType.Stationary)
        {
            Movement = GetComponent<WalkerMovementScript>();
        }
        if (type == GoombaType.Flying)
        {
            flyingTimer = flyingCD;
            Movement.gravityConstant = speed;
        }
    }

    // Update is called once per frame
    void Update()
    {
        MovementType();
    }

    private void MovementType()
    {
        switch (type)
        {
            case GoombaType.Walking:
                Movement.SetHorizontalVelocity(-speed);
                break;
            case GoombaType.Flying:
                flyingTimer += Time.deltaTime;
                if (flyingTimer >= flyingCD)
                {
                    Movement.SetVerticalVelocity(speed);
                    flyingTimer -= flyingTimer;
                }
                break;
            case GoombaType.Stationary:
                if (player.transform.position.x > this.transform.position.x)
                    transform.rotation = Quaternion.AngleAxis(0, Vector3.up);
                else
                    transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
                break;
            default:
                break;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerScript>().Hurt(damage);
            Debug.Log("a");
        }
        switch (type)
        {
            case GoombaType.Walking:
                    if (Movement.collisions.left && !collision.gameObject.CompareTag("Player") || (Movement.collisions.right) && !collision.gameObject.CompareTag("Player"))
                    {
                        speed *= -1;
                        transform.Rotate(0, 180f, 0, Space.Self);
                    }
                break;
            case GoombaType.Flying:
                break;
            case GoombaType.Stationary:
                break;
            default:
                break;
        }
        
    }
}
