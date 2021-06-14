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

    Vector2 currentInput;

    // Start is called before the first frame update
    void Start()
    {
        playerMover = GetComponent<PlayerMovementScript>();
        playerScript = GetComponent<PlayerScript>();
        attack = GetComponent<PlayerAttackScript>();
    }

    // Update is called once per frame
    void Update()
    {
        currentInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector2 signedCurrentInput = new Vector2(Mathf.Sign(currentInput.x), Mathf.Sign(currentInput.y));//uselt namn men kommer inte på bättre

        if (currentInput.x == 0)
            signedCurrentInput.x = 0;
        if (currentInput.y == 0)
            signedCurrentInput.y = 0;


        if (signedCurrentInput.x != 0)
        {
            playerMover.Move((sbyte)signedCurrentInput.x);
            if (playerMover.movementState == PlayerMovementScript.MovementState.wall_gliding || playerMover.movementState == PlayerMovementScript.MovementState.hanging)
                playerScript.facing = playerMover.wallGlideWallDir * -1;
            else
                playerScript.facing = (int)signedCurrentInput.x;
        }

        if (playerMover.movementState == PlayerMovementScript.MovementState.wall_gliding)
        {
            if (signedCurrentInput.y == -1 && Input.GetButtonDown("Jump"))
                playerMover.StopWallGlide();
            else if (signedCurrentInput.y == -1)
                playerMover.WallGlideForceDown();

        }
        

        if (Input.GetButtonDown("Jump"))
        {
            if (currentInput.y == -1)
                playerMover.DuckThroughSemisolid();
            else
            {
                playerMover.StartJump();
            }
        }
        else if (Input.GetButtonUp("Jump"))
            playerMover.StopJump();

        //fixa namngivning och så..
        if (Input.GetButtonDown("Dash"))
            playerMover.StartDash((int)signedCurrentInput.x);

        if (Input.GetButtonDown("Attack1"))
        {
            attack.Attack(playerScript.facing);
        }
    }

    

    
}
