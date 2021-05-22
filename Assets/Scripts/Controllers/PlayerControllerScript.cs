using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMovementScript))]
[RequireComponent(typeof(PlayerScript))]
[RequireComponent(typeof(PlayerAttackScript))]
public class PlayerControllerScript : MonoBehaviour
{
    protected PlayerMovementScript playerMover;
    protected PlayerScript playerScript;

    PlayerAttackScript attack;
    //public AttackScript attack2;

    DashArrowScript dashArrow;

    bool dashAiming = false;
    Vector2 velocityPreAim;//ifall man inte siktar så sätts ens velocity tillbaka till detta värdet
    Vector2 dashArrowAim;

    public float dashAimTime = 1;
    protected float dashAimTimer = 0f;

    Vector2 currentInput;

    // Start is called before the first frame update
    void Start()
    {
        playerMover = GetComponent<PlayerMovementScript>();
        playerScript = GetComponent<PlayerScript>();
        attack = GetComponent<PlayerAttackScript>();
        dashArrow = GetComponentInChildren<DashArrowScript>();
    }

    // Update is called once per frame
    void Update()
    {
        currentInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector2 signCurrentInput = new Vector2(Mathf.Sign(currentInput.x), Mathf.Sign(currentInput.y));//uselt namn men kommer inte på bättre
        if (currentInput.x == 0)
            signCurrentInput.x = 0;
        if (currentInput.y == 0)
            signCurrentInput.y = 0;


        if (signCurrentInput.x != 0 && !dashAiming)
        {
            playerMover.Move((sbyte)signCurrentInput.x);
            playerScript.facing = (int)signCurrentInput.x;
        }

        if (dashAimTimer > 0)
            dashAimTimer -= Time.deltaTime;
        if (dashAimTimer <= 0 && dashAiming)
            StopDashPause();

        if (dashAiming)
            dashArrow.SetPosition(dashArrowAim);

        if (Input.GetButtonDown("Jump"))
        {
            if (currentInput.y == -1)
                playerMover.DuckThroughSemisolid();
            else
            {
                if (dashAiming)
                    StopDashPause();
                playerMover.StartJump();
            }
        }
        else if (Input.GetButtonUp("Jump"))
            playerMover.StopJump();

        //fixa namngivning och så..
        if (Input.GetButtonDown("Dash"))
            StartDashPause();
        else if (Input.GetButtonUp("Dash") && dashAiming)
            StopDashPause();

        if (Input.GetButtonDown("Attack1"))
        {
            attack.Attack(playerScript.facing);
            if (dashAiming)
                StopDashPause(true);
        }

        if (currentInput.magnitude != 0 && dashAiming)
            dashArrowAim = signCurrentInput;
    }

    protected void StartDashPause()
    {
        if (dashAiming || playerMover.dashes <= 0)
            return;
        dashArrowAim = Vector2.zero;

        dashAiming = true;
        dashArrow.SetVisible(true);
        dashAimTimer = dashAimTime;
        velocityPreAim = playerMover.velocity;
        playerMover.velocity = Vector2.zero;
        playerMover.affectedByGravity = false;

    }

    protected void StopDashPause(bool cancelAim = false)
    {
        if (!dashAiming)
            return;

        dashAiming = false;
        playerMover.affectedByGravity = true;
        dashArrow.SetVisible(false);
        dashAimTimer = 0;

        if (dashArrowAim.magnitude == 0 || cancelAim)
        {
            playerMover.SetHorizontalVelocity(velocityPreAim.x);
            playerMover.SetVerticalVelocity(velocityPreAim.y);
        }
        else
            playerMover.StartDash(dashArrowAim);
    }

    
}
