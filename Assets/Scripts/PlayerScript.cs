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

    public int bubbleShieldMaxHP = 20;
    public bool bubbleShieldActive = false;
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
        if (bubbleShieldActive)
        {
            if (!bubbleObject.activeSelf)
                bubbleObject.SetActive(true);
        }
        else
        {
            if (bubbleObject.activeSelf)
                bubbleObject.SetActive(false);
        }
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
        bubbleShieldActive = true;
        currentBubbleShieldHp = bubbleShieldMaxHP;
    }

    public void DeactivateBubbleShield()
    {
        bubbleShieldActive = false;
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
