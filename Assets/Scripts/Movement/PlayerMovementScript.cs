using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Spelarens begränsningar läggs här. Controllern kan se nedräknarvariabler och sådant men den kan kalla alla metoder så mycket den vill också
//om inte spelarens dash cooldown är nedräknad så händer inget.
public class PlayerMovementScript : WalkerMovementScript
{
    public float groundFriction = .9f;//ny fart = fart du har + (fart du vill ha - fart du har) * (ground||air)Friction * Time.deltaTime
    public float airFriction = .4f;

    public float runSpeed = 10f;
    public float jumpVelocity = 20f;
    public float jumpHoldTime = .5f;//sekunder som man kan hålla nere hoppknappen för att få högre hopphöjd.
    public int airJumps = 1;
    
    public int airJumpsAvailable = 1;
    protected const float jumpCooldown = .05f;//liten timer för att förhindra omedelbara dubbelhopp
    protected float jumpCooldownTimer = jumpCooldown;
    protected float currentFriction; //den som används i ekvationerna, byts mellan groundFriction och airFriction

    protected sbyte moveDir = 1;
    protected bool jumping = false;
    protected bool dashing = false;

    // Start is called before the first frame update
    override protected void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    override protected void Update()
    {
        if (jumpCooldownTimer > 0)
            jumpCooldownTimer -= Time.deltaTime;

        if (grounded)
            airJumpsAvailable = airJumps;

        //placeholderkod, lägg till mer avancerad rörelsefysik
        if (moveDir != 0)
            SetHorizontalVelocity(runSpeed * moveDir);
        else
            SetHorizontalVelocity(0);
        
        base.Update();
        moveDir = 0;
    }

    public void Jump()
    {
        if(grounded || airJumpsAvailable > 0)
        {
            SetVerticalVelocity(jumpVelocity);
            if (!grounded)
                airJumpsAvailable--;
            jumpCooldownTimer = jumpCooldown;
        }
    }

    public void Dash()
    {

    }

    public void Move(sbyte dir)//dir == -1 = vänster, dir == 1 = höger
    {
        moveDir = dir;
    }

}
