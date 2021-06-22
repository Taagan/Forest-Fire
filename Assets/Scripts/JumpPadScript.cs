using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPadScript : MonoBehaviour
{

    public int jumpForce = 20;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerMovementScript>().SetVerticalVelocity(jumpForce);
            //collision.gameObject.GetComponent<PlayerMovementScript>().velocity.y += jumpForce;
        }
    }
}
