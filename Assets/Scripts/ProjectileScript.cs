using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    public enum ProjectileType { Returning, Boss, Foward}
    WalkerMovementScript movement;
    public ProjectileType type;
    public Vector3 destination;
    Vector2 moveDirection;
    public int damage;
    public float speed;
    public float lifetime;
    float timer;
    bool swapped;

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        movement = GetComponent<WalkerMovementScript>();
        if (type == ProjectileType.Boss)
        {
            moveDirection = (destination - this.transform.position).normalized * speed;
        }
        if (type == ProjectileType.Returning)
            movement.SetVerticalVelocity(speed);

        if (type == ProjectileType.Foward)
            movement.SetHorizontalVelocity(-speed);

        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        switch (type)
        {
            case ProjectileType.Returning:
                ReturningProjectile();
                break;
            case ProjectileType.Boss:
                BossProjectile();
                Explosion();
                break;
            default:
                break;
        }
    }

    void ReturningProjectile()
    {
        //Do extra stuff if neccessary
    }

    void BossProjectile()
    {
        rb.velocity = new Vector2(moveDirection.x, moveDirection.y);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerScript>().Hurt(damage);
            Destroy(gameObject);

        }

        if (type == ProjectileType.Returning && collision.gameObject.CompareTag("Solid"))
        {
            Destroy(gameObject);
        }

        else if (type == ProjectileType.Boss && !collision.gameObject.CompareTag("Enemy") && !collision.gameObject.CompareTag("Semisolid"))
        {
            Destroy(gameObject);
        }
    }


    void Explosion()
    {
        if (transform.position.x == destination.x)
        {
            //Do explosion effect
            //Projectile flies towards the players location when fired
            //Destroy(gameObject);
        }
    }
}
