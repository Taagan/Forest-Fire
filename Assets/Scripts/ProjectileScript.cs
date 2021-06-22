using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    public enum ProjectileType { returning, boss}
    WalkerMovementScript movement;
    public ProjectileType type;
    public Vector3 destination;
    Vector2 moveDirection;
    public float damage;
    public float speed;
    public float lifetime;
    float timer;
    bool swapped;

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        movement = GetComponent<WalkerMovementScript>();
        if (type == ProjectileType.boss)
        {
            moveDirection = (destination - this.transform.position).normalized * speed;
        }
        if (type == ProjectileType.returning)
            movement.SetVerticalVelocity(speed);

        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        switch (type)
        {
            case ProjectileType.returning:
                ReturningProjectile();
                break;
            case ProjectileType.boss:
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


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (type == ProjectileType.boss && !collision.CompareTag("Enemy") && !collision.CompareTag("Semisolid"))
        {
            Destroy(gameObject);
        }
    }


    void Explosion()
    {
        if (transform.position.x == destination.x)
        {
            //Do explosion effect
            Destroy(gameObject);
        }
    }
}
