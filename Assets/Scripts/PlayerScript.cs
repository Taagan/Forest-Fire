using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayerScript : MonoBehaviour
{
    //0 == no | 1 == yes
    public int hasSword;
    public int hasShield;

    public int maxHitPoints { get; private set; } = 10;
    public int hitpoints;

    public int facing = 1;//1 = höger, -1 = vänster
    

    public GameObject currentCheckpoint;

    protected SpriteRenderer sRenderer;

    // Start is called before the first frame update
    void Start()
    {
        hitpoints = maxHitPoints;
        sRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTimers();

        if (facing == 1 && sRenderer.flipX == true)
            sRenderer.flipX = false;
        else if (facing == -1 && sRenderer.flipX == false)
            sRenderer.flipX = true;
        
    }
    
    //samlingsställe för alla timernedräkningar och effekter de medför mest. Kallas i update()
    protected void UpdateTimers()
    {
        float dT = Time.deltaTime;
    }

    /// <summary>
    /// använd för instakills. T.ex. Deathbox, eller spikar eller annat. Inte för vanlig skada från combat.
    /// </summary>
    public void Kill()
    {
        hitpoints = 0;
        Debug.Log("Player killed");
    }

    public void Hurt(int damage)
    {
        hitpoints -= damage;
    }
    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "End")
        {
            SceneManager.LoadScene(0);
        }
        else if (collision.gameObject.tag == "Checkpoint")
        {
            currentCheckpoint = collision.gameObject;
        }
    }
}
