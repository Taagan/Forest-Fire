using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    //0 == no | 1 == yes
    public int hasSword;
    public int hasShield;
    [SerializeField]
    public int maxHitPoints = 10;
    public int hitpoints;
    [HideInInspector]
    public int deathcounter;

    public int facing = 1;//1 = höger, -1 = vänster
    public bool iFrame;
    private float iFrametimer;
    [SerializeField]
    private float iFrameDuration = 1;

    public Text hpTXT;
    public Text deathCountTXT;
    public GameObject currentCheckpoint;

    protected PlayerMovementScript movementScript;
    protected SpriteRenderer sRenderer;
    protected Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        hitpoints = maxHitPoints;
        sRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        movementScript = GetComponent<PlayerMovementScript>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTimers();

        if (animator != null)
            AnimatorUpdate();

        if (facing == 1 && sRenderer.flipX == true)
            sRenderer.flipX = false;
        else if (facing == -1 && sRenderer.flipX == false)
            sRenderer.flipX = true;

        Suicide();
        IFrameMethod();

        hpTXT.text = hitpoints.ToString();
        deathCountTXT.text = deathcounter.ToString();
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
        if (!iFrame && damage > 0)
        {
            hitpoints -= damage;
            iFrame = true;
        }
    }
    
    private void IFrameMethod()
    {
        if (iFrame)
        {
            hpTXT.color = Color.blue;
            iFrametimer += Time.deltaTime;
            if (iFrametimer >= iFrameDuration)
            {
                iFrame = false;
                iFrametimer -= iFrametimer;
                hpTXT.color = Color.red;
            }

        }
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "End")
        {
            SceneManager.LoadScene(0);
        }
        else if (collision.gameObject.tag == "Checkpoint" && currentCheckpoint != collision.gameObject)
        {
            currentCheckpoint = collision.gameObject;
        }
    }


    
    protected void AnimatorUpdate()
    {
        MovementState movementState = movementScript.movementState;

        if (movementState == MovementState.running)
        {
            animator.SetBool("running", true);
            if (movementScript.speedLevel == 0)
                animator.speed = 1;
            else if (movementScript.speedLevel == 1)
                animator.speed = 1.3f;
            else if (movementScript.speedLevel == 2)
                animator.speed = 1.6f;
        }
        else
        {
            animator.SetBool("running", false);
            animator.speed = 1;
        }
    }


    void Suicide()
    {
        if (Input.GetKeyDown("space"))
        {
            Kill();
        }
    }
}
