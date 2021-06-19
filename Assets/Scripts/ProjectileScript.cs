using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    public enum ProjectileType { returning, boss}
    public ProjectileType type;
    public float damage;
    public float speed;
    public float lifetime;
    float timer;
    bool swapped;

    private Rigidbody2D rb;

    private void Start()
    {
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
                break;
            default:
                break;
        }
    }

    void ReturningProjectile()
    {
        timer += Time.deltaTime;
        if (timer >= lifetime / 2 && !swapped)
        {
            speed *= -1;
            swapped = true;
        }
        this.transform.Translate(speed, 0, 0);
    }

    void BossProjectile()
    {
        this.transform.Translate(-speed, 0, 0);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (type == ProjectileType.boss)
        {
            this.transform.rotation = Quaternion.Inverse(this.transform.rotation);
            speed = Random.Range(speed/2, speed);
        }
    }


    void CheckPlayerPosition()
    {

    }
}
