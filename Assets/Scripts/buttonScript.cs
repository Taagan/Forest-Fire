using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class buttonScript : MonoBehaviour
{
    public Tilemap wall;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            wall.ClearAllTiles();
            Destroy(gameObject);
        }
    }
}
