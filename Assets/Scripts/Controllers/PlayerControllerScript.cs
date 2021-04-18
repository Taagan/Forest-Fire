using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMovementScript))]
public class PlayerControllerScript : MonoBehaviour
{
    protected PlayerMovementScript playerMover;

    // Start is called before the first frame update
    void Start()
    {
        playerMover = GetComponent<PlayerMovementScript>();
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
    }

    protected void StartJump()
    {
        
    }
}
