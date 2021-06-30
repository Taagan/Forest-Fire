using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BossStartPointScript : MonoBehaviour
{
    public GameObject boss;
    public TilemapCollider2D boss_Door_Coll;
    public TilemapRenderer boss_Door_Rend;
    public GameObject respawn_Point;
    GameObject player;
    bool used;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!used)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                boss_Door_Coll.enabled = true;
                boss_Door_Rend.enabled = true;
                used = true;
                boss.GetComponent<bossScript>().phaseCounter = 1;
                player.GetComponent<PlayerScript>().currentCheckpoint = respawn_Point;
            }
        }
    }
}
