using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMovementScript))]
[RequireComponent(typeof(PlayerScript))]
public class PlayerControllerScript : MonoBehaviour
{
    protected PlayerMovementScript playerMover;
    protected PlayerScript playerScript;

    // Start is called before the first frame update
    void Start()
    {
        playerMover = GetComponent<PlayerMovementScript>();
        playerScript = GetComponent<PlayerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        float xAxis = Input.GetAxisRaw("Horizontal");
        float yAxis = Input.GetAxisRaw("Vertical");

        if (xAxis != 0)
            playerMover.Move((sbyte)xAxis);

        if (Input.GetButtonDown("Jump"))
        {
            if (yAxis == -1)
                playerMover.DuckThroughSemisolid();
            else
                playerMover.StartJump();
        }
        else if (Input.GetButtonUp("Jump"))
            playerMover.StopJump();

        if (Input.GetButtonDown("Block"))
            playerScript.ActivateBubbleShield();
        else if (Input.GetButtonUp("Block"))
            playerScript.DeactivateBubbleShield();

        //osäker om sköld och studs ska vara så direkt kopplade men nu är de det.
        playerMover.bounceActive = playerScript.bubbleShieldActive;
    }
    
}
