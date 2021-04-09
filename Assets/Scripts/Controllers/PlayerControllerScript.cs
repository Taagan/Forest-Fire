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
        if (xAxis != 0)
            playerMover.Move((sbyte)xAxis);

        if (Input.GetButton("Jump"))
            playerMover.SetVerticalVelocity(playerMover.jumpVelocity);

        if (Input.GetAxisRaw("Vertical") < 0)
        {
            playerMover.ignoreSemisolid = true;
            playerMover.SetVerticalVelocity(-5);
        }
        else
            playerMover.ignoreSemisolid = false;
    }
}
