using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovementState
{
    none,
    running,
    wall_gliding,
    hanging, //hänger på väggkant
    jumping,
    falling,
    dashing
}

//Spelarens begränsningar läggs här. Controllern kan se nedräknarvariabler och sådant men den kan kalla alla metoder så mycket den vill också
//om inte spelarens dash cooldown är nedräknad så händer inget.
[RequireComponent(typeof(PlayerScript))]
public class PlayerMovementScript : WalkerMovementScript
{

    public MovementState movementState { get; protected set; } = MovementState.none;
    public int speedLevel { get; protected set; } = 0;

    [HideInInspector] public PlayerScript playerScript;
    SpriteRenderer sRenderer;//antagligen temporär typ, används nu för att ändra färg beroende på movementState

    [Space(10)]
    [Header("               Running/speed")]
    public float[] runSpeed = new float[] { 7, 10, 15 };
    public float groundAcceleration = 100f;
    public float airAcceleration = 50f;
    public float groundDecceleration = 200f;
    public float airDecceleration = 50f;
    
    [Space(10)]
    [Header("               Jump")]
    public float jumpVelocity = 10f;
    public float jumpHoldTime = .15f;//sekunder som man kan hålla nere hoppknappen för att få högre hopphöjd.
    public int airJumps = 1;
    public float jumpFwdBoost = 5;//fart frammåt som man får om man rör vill röra sig medans man gör ett hopp
    public float coyoteTime = .1f;//tid efter man lämnat marken som man fortfarande är grounded.

    [Space(10)]
    [Header("               Dash")]
    public float dashTime = .3f;//Sekunder som man dashar om den inte avbryts.
    public float dashVelocity = 20f;
    public int maxDashes = 1;
    public int dashes { get; protected set; } = 1;

    [Space(10)]
    [Header("               Wall things")]
    [Tooltip("tid som man håller sig stilla efter man börjat hålla sig till en vägg")]
    public float wallGlideHoldTime = 0f;
    [Tooltip("Tid som det tar för spelaren att släppa väggen efter att man slutar ge input mot väggen")]
    public float wallGlideReleaseTime = .5f;
    public float wallJumpOutVelocity = 12f;//velocity ut från väggen när man vägghoppar
    public float wallJumpUpVelocity = 10f;
    public float wallGlideGravity = 10;
    public float wallGlideMaxVelocity = 7;
    public float wallGlideForceDownGravity = 13;
    public float wallGlideForceDownMaxVelocity = 10;
    
    protected const float ignSemisolidsUpTime = .25f;
    protected float ignSemisolidsTimer = 0f;
    protected float duckThroughDownVel = -5f;//velocity applied to more smoothly pass through semisolids

    protected float currentJumpVelocity = 10f;
    protected int airJumpsAvailable = 1;
    protected float jumpingTimer = 0;
    protected const float jumpCooldown = .05f;//liten timer för att förhindra omedelbara "accidental" dubbelhopp
    protected float jumpCooldownTimer = jumpCooldown;
    protected float coyoteTimer = 0;

    protected int forgivnessLevel = 0;//level of speed to get back to if forgiven
    protected int forgivnessDir = 1;//riktningen som man kan få tillbaka sin fart åt.
    protected float forgivnessTime = .4f;//tid som man kan låta bli att ge input utan att man förlorar sin speedlevel
    protected float forgivnessTimer = 0;
    protected float loseSpeedThreshold = .4f;//andel av minfarten man måste hålla sig över för att inte förlora farten.
    protected float loseSpeedTime = .2f;//förlora speedlevel om man har stått stilla så här länge.
    protected float loseSpeedTimer = 0;
    protected int maxSpeedLevel;
    protected int moveDirLast = 1;
    protected int moveDir = 1;
    protected float wantedHorizontalSpeed = 0f;

    public int wallGlideWallDir { get; protected set; } = 1;
    protected float wallGlideHoldTimer = 0;
    protected float wallGlideReleaseTimer = 0;
    protected bool wallGlideDownForced = false;

    protected int dashDir = 1;
    protected float dashTimer;


    //Colors, för debug mest kanske. Om det inte blir snyggt nog att ha i spelet
    private Color noneColor = Color.white;
    private Color wall_glidingColor = Color.blue;
    private Color hangingColor = Color.grey; //hänger på väggkant
    private Color jumpingColor = Color.white;
    private Color fallingColor = Color.white;
    private Color dashingColor = Color.cyan;

    private Color[] speedColor = new Color[] { Color.white, Color.magenta, Color.red };

    // Start is called before the first frame update
    override protected void Start()
    {
        base.Start();
        playerScript = GetComponent<PlayerScript>();
        sRenderer = GetComponent<SpriteRenderer>();

        maxSpeedLevel = runSpeed.Length;
    }

    // Update is called once per frame
    override protected void Update()
    {
        Timers();

        if (grounded)
        {
            airJumpsAvailable = airJumps;
            dashes = maxDashes;
        }
        
        //Movements
        //None och falling gör inget unikt just nu. De bara ändrar kanske movementState och sedan skickar vidare till running 
        //eftersom där hanteras acceleration redan på ett sätt som inte bör vara neutralt för running
        switch (movementState)
        {
            case MovementState.none:
                sRenderer.color = noneColor;

                if (!grounded)
                {
                    movementState = MovementState.falling;
                    goto case MovementState.falling;
                }
                else if (moveDir != 0)
                {
                    movementState = MovementState.running;
                    goto case MovementState.running;
                }

                StandardMovementUpdate();
                break;

            case MovementState.falling:
                sRenderer.color = fallingColor;

                if (grounded && moveDir == 0)
                {
                    movementState = MovementState.none;
                    goto case MovementState.none;
                }
                else if (grounded)
                {
                    movementState = MovementState.running;
                    goto case MovementState.running;
                }

                if (WallGlideCheck())
                {
                    StartWallGlide();
                    goto case MovementState.wall_gliding;
                }

                StandardMovementUpdate();
                break;

            case MovementState.running:
                sRenderer.color = speedColor[speedLevel];

                if (!grounded)
                {
                    movementState = MovementState.falling;
                    goto case MovementState.falling;
                }
                else if (moveDir == 0)
                {
                    movementState = MovementState.none;
                    goto case MovementState.none;
                }

                StandardMovementUpdate();
                break;

            case MovementState.wall_gliding:
                sRenderer.color = wall_glidingColor;

                //Kolla om fortfarande är intill väggen. Detta funkar eftersom en liten fart sätts mot väggen
                if(wallGlideWallDir == 1 && !collisions.right || wallGlideWallDir == -1 && !collisions.left)
                {
                    StopWallGlide();
                    movementState = MovementState.falling;
                    goto case MovementState.falling;
                }

                if (grounded)//om grounded är man inte längre på väggen ju..
                {
                    StopWallGlide();
                    movementState = MovementState.none;
                    goto case MovementState.none;
                }

                if (wallGlideHoldTimer <= 0 && !wallGlideDownForced)//glid neråt
                {
                    velocity.y -= wallGlideGravity * Time.deltaTime;
                    if (velocity.y < -wallGlideMaxVelocity)
                        velocity.y = -wallGlideMaxVelocity;
                }
                else if (wallGlideDownForced)
                    wallGlideDownForced = false;

                //kolla om ger input mot väggen, isåfall håll kvar, annars börja släppa den.
                if (moveDir != wallGlideWallDir && wallGlideReleaseTimer <= 0)
                    wallGlideReleaseTimer = wallGlideReleaseTime;
                else if (moveDir == wallGlideWallDir && wallGlideReleaseTimer > 0)
                    wallGlideReleaseTimer = 0;

                //sätt velocity.x till lite mot väggen så att man kolliderar med den och därmed kan kolla ifall man inte längre kolliderar med den..
                velocity.x = (float)wallGlideWallDir * .5f;

                break;

            case MovementState.hanging:
                sRenderer.color = hangingColor;

                break;

            case MovementState.jumping:
                sRenderer.color = jumpingColor;

                StandardMovementUpdate();
                break;

            case MovementState.dashing:
                sRenderer.color = dashingColor;
                
                sRenderer.color = Color.blue;
                break;
        }
        
        base.Update();

        //resets
        if (collisions.right && velocity.x > 0 || collisions.left && velocity.x < 0)
        {
            velocity.x = 0;
        }
        if (collisions.above && velocity.y > 0)
            velocity.y = 0;

        moveDirLast = moveDir;
        moveDir = 0;
    }
    

    protected void StandardMovementUpdate()
    {
        if (speedLevel > 0 && Mathf.Abs(velocity.x) < runSpeed[0] * loseSpeedThreshold && loseSpeedTimer <= 0)//om står stilla
            loseSpeedTimer = loseSpeedTime;
        else if (Mathf.Abs(velocity.x) > runSpeed[0] * loseSpeedThreshold)
            loseSpeedTimer = 0;

        //Kolla om vänder eller stannar så ska forgivnessTimern starta och man kan då inom tidsramen kunna få tillbaka sin fart åt det hållet.
        if (speedLevel > 0 && forgivnessTimer <= 0)
        {
            if (moveDirLast == moveDir * -1 || moveDir == 0)//om vänder eller stannar starta förlåtelsetiden
            {
                forgivnessTimer = forgivnessTime;
                forgivnessLevel = speedLevel;
                forgivnessDir = moveDirLast;
                speedLevel = 0;
            }
        }
        else if (forgivnessTimer > 0 && moveDir == forgivnessDir)//forgiven
        {
            forgivnessTimer = 0;
            speedLevel = forgivnessLevel;
        }

        Accelerate();
    }

    //om den går från sant till falsk så startar den coyoteTimer istället som sen sätter den till falskt efter asså
    protected override void SetGrounded(bool value)
    {
        if (grounded == value)
            return;
        else if (!value && coyoteTimer <= 0)
        {
            coyoteTimer = coyoteTime;
        }
        else if (value)
        {
            grounded = true;
            coyoteTimer = 0;
        }

    }

    protected void Timers()
    {
        float dT = Time.deltaTime;

        if (wallGlideHoldTimer > 0)
            wallGlideHoldTimer -= dT;

        if(wallGlideReleaseTimer > 0)
        {
            wallGlideReleaseTimer -= dT;
            if (wallGlideReleaseTimer <= 0)
                StopWallGlide();
        }

        if(loseSpeedTimer > 0)
        {
            loseSpeedTimer -= dT;
            if (loseSpeedTimer <= 0)
                speedLevel = 0;
        }

        if (forgivnessTimer > 0)
        {
            forgivnessTimer -= dT;
        }

        if(coyoteTimer > 0)
        {
            coyoteTimer -= dT;
            if (coyoteTimer <= 0)
                grounded = false;
        }

        if (jumpCooldownTimer > 0)
        {
            jumpCooldownTimer -= dT;
        }

        if (jumpingTimer > 0)
        {
            jumpingTimer -= dT;
            SetVerticalVelocity(currentJumpVelocity);
        }

        if (dashTimer > 0)
        {
            dashTimer -= dT;
        }
        if (movementState == MovementState.dashing && dashTimer <= 0)
        {
            StopDash();
        }
        
        if (ignSemisolidsTimer > 0)
        {
            ignoreSemisolid = true;
            ignSemisolidsTimer -= dT;
        }
        else
            ignoreSemisolid = false;
    }
    
    protected void Accelerate()
    {
        if (moveDir != 0)
            wantedHorizontalSpeed = runSpeed[speedLevel] * moveDir;
        else
            wantedHorizontalSpeed = 0;

        //räkna ut om accelererar eller deccelererar.
        //om deccelererar mot att vända så deccelererar man snabbare, kanske inte bör vara så men..
        //farten man accelererar/deccelerar med är ett konstant siffervärde per sekund
        float dT = Time.deltaTime;

        int wantedDir = wantedHorizontalSpeed != 0 ? (int)Mathf.Sign(wantedHorizontalSpeed) : 0;
        int currentDir = velocity.x != 0 ? (int)Mathf.Sign(velocity.x) : 0;
        
        if (wantedDir == 0 && currentDir == 0)
            return;

        //om siktar på noll
        if(wantedDir == 0 || wantedDir == currentDir * -1)
        {
            //alltid positiv version av velocity.x, räkna på den så och flippa till rätt håll sen bara.
            float vel = velocity.x * currentDir;
            float deccel = grounded ? groundDecceleration : airDecceleration;
            vel -= deccel * dT;

            if (vel < 0)
                vel = 0;//kan orsaka problem kanske då detta gör att man står helt stilla en hel frame när man vänder håll, känn efter.

            velocity.x = vel * currentDir;
        }
        //om ska vända sig så deccelererar man snabbare, kanske inte bör vara så egentligen. Blir lite för kontrollerbart i höga farter.
        //else if(wantedDir == currentDir * -1)
        //{
        //    float vel = velocity.x * currentDir;
        //    float deccel = grounded ? groundDecceleration + groundAcceleration : airDecceleration + airAcceleration;
        //    vel -= deccel * dT;

        //    velocity.x = vel * currentDir;
        //}
        //accelerera
        else if (currentDir == 0 || (wantedHorizontalSpeed * wantedDir > velocity.x * currentDir && wantedDir == currentDir))
        {
            float vel = currentDir * velocity.x;
            float accel = grounded ? groundAcceleration : airAcceleration;
            vel += accel * dT;

            if (vel > wantedHorizontalSpeed * wantedDir)
                vel = wantedHorizontalSpeed * wantedDir;

            velocity.x = vel * wantedDir;
        }
        //överfartsdeccelerera
        else if (wantedDir == currentDir && velocity.x * currentDir > wantedHorizontalSpeed * wantedDir)
        {
            float vel = currentDir * velocity.x;
            float deccel = grounded ? groundDecceleration : airDecceleration;
            vel -= deccel * dT;

            if (vel < wantedHorizontalSpeed * wantedDir)
                vel = wantedHorizontalSpeed * wantedDir;

            velocity.x = vel * wantedDir;
        }
    }
    
    protected bool WallGlideCheck()
    {
        if (moveDir == 0)
            return false;

        if (moveDir == 1 && collisions.right || moveDir == -1 && collisions.left && velocity.y < 0)
        {
            wallGlideWallDir = moveDir;
            return true;
        }

        return false;
    }

    protected void StartWallGlide()
    {
        wallGlideHoldTimer = wallGlideHoldTime;

        velocity.y = 0;
        affectedByGravity = false;
        movementState = MovementState.wall_gliding;
    }

    public void StopWallGlide()
    {
        if (movementState != MovementState.wall_gliding)
            return;
        affectedByGravity = true;
        movementState = MovementState.falling;
    }

    public void WallGlideForceDown() //ökar acceleration och maxfart neråt under en uppdateringsloop
    {
        if (movementState != MovementState.wall_gliding || wallGlideDownForced)
            return;

        wallGlideDownForced = true;

        if (wallGlideHoldTimer > 0)
            wallGlideHoldTimer = 0;

        if(velocity.y > -wallGlideForceDownMaxVelocity)
        {
            velocity.y -= wallGlideForceDownGravity * Time.deltaTime;
            if (velocity.y < wallGlideForceDownMaxVelocity)
                velocity.y = -wallGlideForceDownMaxVelocity;
        }
    }

    protected void WallJump()
    {
        //Set velocity.x bort från väggen ett värde
        currentJumpVelocity = wallJumpUpVelocity;
        velocity.x = wallGlideWallDir * -1 * wallJumpOutVelocity;
        velocity.y = currentJumpVelocity;
        
        StopWallGlide();
        jumpingTimer = jumpHoldTime;
        jumpCooldownTimer = jumpCooldown;
        movementState = MovementState.jumping;
    }

    

    public void StartJump()
    {
        if (movementState == MovementState.wall_gliding)
            WallJump();
        else if ((grounded || airJumpsAvailable > 0) && jumpCooldownTimer <= 0 && movementState != MovementState.jumping)
        {    
            if (movementState == MovementState.dashing)
                StopDash();

            if (!grounded)
                airJumpsAvailable--;

            currentJumpVelocity = jumpVelocity;

            jumpingTimer = jumpHoldTime;
            jumpCooldownTimer = jumpCooldown;
            movementState = MovementState.jumping;

            if (moveDir != 0)
                velocity.x += moveDir * jumpFwdBoost;
        }
    }

    public void StopJump()
    {
        jumpingTimer = 0;
        if(movementState == MovementState.jumping)
            movementState = MovementState.falling;
    }

    public void DuckThroughSemisolid()
    {
        if (!grounded)
            return;

        ignSemisolidsTimer = ignSemisolidsUpTime;
        if (velocity.y > duckThroughDownVel)
            velocity.y = duckThroughDownVel;
    }

    //konstant snabb fart, studsbar, timer, ska gå att avsluta när som??
    public void StartDash(int dir)
    {
        if (movementState == MovementState.dashing || dashes < 1)
            return;

        if (movementState == MovementState.jumping)
        {
            StopJump();
        }

        if (dir != 0)
            dir = (int)Mathf.Sign(dir);//ett eller minus ett..
        else
            dir = playerScript.facing;
        
        if (speedLevel < maxSpeedLevel - 1)
            speedLevel++;

        velocity.x = dir * dashVelocity;
        velocity.y = 0;
        affectedByGravity = false;

        dashDir = dir;
        dashes--;
        dashTimer = dashTime;
        movementState = MovementState.dashing;
    }

    //händer automatiskt när dashtimer är slut eller när controllern kör den.
    public void StopDash()
    {
        if(movementState == MovementState.dashing)
            movementState = MovementState.none;
        affectedByGravity = true;
    }

    public void Move(int dir)//dir == -1 = vänster, dir == 1 = höger
    {
        moveDir = dir;
    }

}
