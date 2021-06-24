using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum ButtonType { Pickup, Shoot}


public class buttonScript : HittableScript
{
    public Tilemap wall;
    public ButtonType type;


    private void Update()
    {
        if (trigger)
        {
            wall.ClearAllTiles();
            Destroy(gameObject);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (type)
        {
            case ButtonType.Pickup:
                if (collision.gameObject.CompareTag("Player"))
                {
                    wall.ClearAllTiles();
                    Destroy(gameObject);
                }
                break;
            case ButtonType.Shoot:
                break;
            default:
                break;
        }
    }
}
