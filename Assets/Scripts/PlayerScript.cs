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

    public float bubbleShieldActiveTime = 1f;//sekunder som bubbelskölden är aktiv
    public float bubbleShieldCooldown = 1f;//sekunder efter den deaktiverades som den måste vänta innan den kan aktiveras igen
    public int bubbleShieldMaxHP = 20;
    public bool bubbleShieldActive = false;

    protected float bubbleShieldActiveTimer = 0;
    protected float bubbleShieldCooldownTimer = 0;
    protected int currentBubbleShieldHp;

    [SerializeField]
    protected GameObject bubbleObject;//för att visa bubblan bara. än så länge iaf. behöver nog ändras senare antar jag..

    public GameObject currentCheckpoint;

    // Start is called before the first frame update
    void Start()
    {
        hitpoints = maxHitPoints;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTimers();


        //kontrollerar om bubbelskölden syns eller inte
        if (bubbleShieldActive)
        {
            if (!bubbleObject.activeSelf)
                bubbleObject.SetActive(true);
        }
        else if(!bubbleShieldActive)
            if (bubbleObject.activeSelf)
                bubbleObject.SetActive(false);
    }
    
    //samlingsställe för alla timernedräkningar och effekter de medför mest. Kallas i update()
    protected void UpdateTimers()
    {
        float dT = Time.deltaTime;

        if (bubbleShieldCooldownTimer > 0)
            bubbleShieldCooldownTimer -= dT;

        if (bubbleShieldActiveTimer > 0)
            bubbleShieldActiveTimer -= dT;
        else if(bubbleShieldActiveTimer <= 0 && bubbleShieldActive)
            DeactivateBubbleShield();
        
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
        if (bubbleShieldActive)
        {
            currentBubbleShieldHp -= damage;
            if (currentBubbleShieldHp > 0)
                damage = 0;
            else if(currentBubbleShieldHp <= 0)
            {
                damage = -1 * currentBubbleShieldHp;
                DestroyBubbleShield();
            }
        }
        hitpoints -= damage;
    }
    
    public void ActivateBubbleShield()
    {
        if (bubbleShieldActive || bubbleShieldCooldownTimer > 0)
            return;

        Debug.Log("ActivateBubbleShield");

        bubbleShieldActive = true;
        currentBubbleShieldHp = bubbleShieldMaxHP;
        bubbleShieldActiveTimer = bubbleShieldActiveTime;
    }

    public void DeactivateBubbleShield()
    {
        Debug.Log("DeactivateBubbleShield");

        bubbleShieldActive = false;
        bubbleShieldCooldownTimer = bubbleShieldCooldown;
        bubbleShieldActiveTimer = 0;
    }

    protected void DestroyBubbleShield()
    {
        DeactivateBubbleShield();
        //Animation, stun, eller annan effekt här??
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
