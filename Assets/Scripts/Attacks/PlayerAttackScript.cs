using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackScript : MonoBehaviour
{
    public GameObject projectile;

    public int projectiles = 10;
    public float spread = 30f;//degrees total spread, alltså spread/2 graders spridning från mitten
    public float maxLifeTime = 1f, minLifeTime = .8f;
    public float maxVelocity = 30f, minVelocity = 25f;

    public string[] collisionLayers;

    LayerMask layerMask;

    // Start is called before the first frame update
    void Start()
    {
        layerMask = LayerMask.GetMask(collisionLayers);
    }

    public void Attack(int dir)
    {
        //instantiera alla projectiler, generera slumpade nr mellan värdena angivna i variablerna och ge de till projektilerna

        for(int i = 0; i < projectiles; i++)
        {
            Vector2 vel = Vector2.right;

            //rotera enhetsvektorn vel med +- spread/2 som vinkel
            float randAngle = Random.value * spread - spread/2;
            randAngle *= Mathf.Deg2Rad;
            float s = Mathf.Sin(randAngle);
            float c = Mathf.Cos(randAngle);
            vel = new Vector2(
                vel.x * c - vel.y * s,
                vel.x * s + vel.x * s
                );

            //multiplicera vel med farten den ska ha
            float randSpeed = Random.value * (maxVelocity - minVelocity) + minVelocity;
            vel *= randSpeed;
            vel.x *= dir;//flippa x-fart om dir är åt vänster

            float life = Random.value * (maxLifeTime - minLifeTime) + minLifeTime;

            SaltkristallScript proj = Instantiate(projectile).GetComponent<SaltkristallScript>();
            proj.Init(transform.position, vel, life, layerMask);
        }

    }
}
