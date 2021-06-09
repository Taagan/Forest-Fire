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
        Vector2 signCurrentInput = new Vector2(Mathf.Sign(currentInput.x), Mathf.Sign(currentInput.y));//uselt namn men kommer inte på bättre

        if (currentInput.x == 0)
            signCurrentInput.x = 0;
        if (currentInput.y == 0)
            signCurrentInput.y = 0;


        if (signCurrentInput.x != 0)
        {
            playerMover.Move((sbyte)signCurrentInput.x);
            playerScript.facing = (int)signCurrentInput.x;
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
            playerMover.StartDash((int)signCurrentInput.x);

        if (Input.GetButtonDown("Attack1"))
        {
            attack.Attack(playerScript.facing);
        }
    }

    

    
}
