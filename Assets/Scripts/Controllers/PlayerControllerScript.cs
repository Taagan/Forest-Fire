using UnityEngine;

[RequireComponent(typeof(PlayerMovementScript))]
[RequireComponent(typeof(PlayerScript))]
[RequireComponent(typeof(PlayerAttackScript))]
public class PlayerControllerScript : MonoBehaviour
{
    protected PlayerMovementScript playerMover;
    protected PlayerScript playerScript;
    float solidDropBuffer;
    public float solidDropBufferTime = 1;
    bool solidDropCheck;
    bool solidDropWindow;
    float lastMoveY;
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
        lastMoveY = currentInput.y;
        currentInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector2 signedCurrentInput = new Vector2(Mathf.Sign(currentInput.x), Mathf.Sign(currentInput.y));//uselt namn men kommer inte på bättre

        if (currentInput.x == 0)
            signedCurrentInput.x = 0;
        if (currentInput.y == 0)
            signedCurrentInput.y = 0;


        if (signedCurrentInput.x != 0)
        {
            playerMover.Move((sbyte)signedCurrentInput.x);
            if (playerMover.movementState == MovementState.wall_gliding || playerMover.movementState == MovementState.hanging)
                playerScript.facing = playerMover.wallGlideWallDir * -1;
            else
                playerScript.facing = (int)signedCurrentInput.x;
        }

        if (playerMover.movementState == MovementState.wall_gliding)
        {
            if (signedCurrentInput.y == -1 && Input.GetButtonDown("Jump"))
                playerMover.StopWallGlide();
            else if (signedCurrentInput.y == -1)
                playerMover.WallGlideForceDown();

        }

        SemiSolidDrop();


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


    void SemiSolidDrop()
    {
        if (lastMoveY != -1)
            solidDropCheck = true;
        else
        {
            solidDropCheck = false;
        }
        if (solidDropWindow)
        {
            solidDropBuffer += Time.deltaTime;
            if (solidDropBuffer >= solidDropBufferTime)
            {
                solidDropWindow = false;
                solidDropBuffer -= solidDropBuffer;
            }
        }
        Debug.Log("Didn't hold down last frame " + solidDropCheck);
        Debug.Log("We clicked down in the last 0.1s " + solidDropWindow);
        Debug.Log("our current input is " + currentInput.y);
        if (currentInput.y == -1)
        {
            solidDropWindow = true;

            //currentInput.y == -1 is if we're holding down
            //solidDropCheck is true if we did not hold down last frame
            //solidDropWindow is ture if we did hold down last frame
            if (currentInput.y == -1 && solidDropCheck && solidDropWindow && solidDropBuffer >= solidDropBufferTime / 4)
            {
                playerMover.DuckThroughSemisolid();
                solidDropCheck = false;
                solidDropWindow = false;
                solidDropBuffer -= solidDropBuffer;
            }
        }
    }




}
