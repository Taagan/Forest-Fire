using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bossScript : MonoBehaviour
{
    WalkerMovementScript movement;
    public enum Phase { Idle, Slash, Projectile, Jump}
    public Phase currentPhase;
    int phaseCounter;
    public float phaseTime;
    public float timer;
    public float jumpHeight = 5;
    bool turned;
    public GameObject projectile;
    public GameObject slash;
    public GameObject fireSpawnPoint;
    private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        movement = GetComponent<WalkerMovementScript>();
        player = GameObject.FindGameObjectWithTag("Player");
        switch (currentPhase)
        {
            case Phase.Idle:
                    phaseCounter = 0;
                break;
            case Phase.Slash:
                    phaseCounter = 1;
                break;
            case Phase.Projectile:
                    phaseCounter = 2;
                break;
            case Phase.Jump:
                    phaseCounter = 3;
                break;
            default:
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (phaseCounter > 3)
        {
            phaseCounter = 1;
            currentPhase++;
        }
        timer += Time.deltaTime;
        if (timer >= phaseTime / 2 && !turned)
        {
            if (player.transform.position.x > this.transform.position.x)
                transform.rotation = Quaternion.AngleAxis(0, Vector3.up);

            if (player.transform.position.x < this.transform.position.x)
                transform.rotation = Quaternion.AngleAxis(180, Vector3.up);

            turned = true;
        }
        if (timer >= phaseTime)
        {
            switch (phaseCounter)
            {
                case 0:
                    //currentPhase = Phase.Idle;
                    //Do idle stuff
                    break;
                case 1:
                    currentPhase = Phase.Slash;
                    Slash();
                    break;
                case 2:
                    currentPhase = Phase.Projectile;
                    Projectile();
                    break;
                case 3:
                    currentPhase = Phase.Jump;
                    Jump();
                    break;
                default:
                    break;
            }
            timer = 0;
            turned = false;
        }
    }

    public void Slash()
    {
        GameObject flame = Instantiate(slash, fireSpawnPoint.transform.position, this.transform.rotation);
        flame.GetComponent<ProjectileScript>().destination = player.transform.position;
        phaseCounter++;
    }

    public void Projectile()
    {
        GameObject flame = Instantiate(projectile, fireSpawnPoint.transform.position, this.transform.rotation);
        flame.GetComponent<ProjectileScript>().destination = player.transform.position;
        phaseCounter++;
    }

    public void Jump()
    {
        movement.SetVerticalVelocity(jumpHeight);
        phaseCounter++;
    }
}
