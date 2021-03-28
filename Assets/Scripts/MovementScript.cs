using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementScript : MonoBehaviour
{

    public float speed = 20f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("a"))
        {
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(-speed, gameObject.GetComponent<Rigidbody2D>().velocity.y);
        }
        else if (Input.GetKey("d"))
        {
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(speed, gameObject.GetComponent<Rigidbody2D>().velocity.y);
        }
        else
        {
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, gameObject.GetComponent<Rigidbody2D>().velocity.y);

        }
        if (Input.GetKey("space"))
        {
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(gameObject.GetComponent<Rigidbody2D>().velocity.x, speed);

        }
    }
}
