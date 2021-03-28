﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public GameObject start;
    public GameObject End;
    public GameObject player;
    public GameObject checkpoint;
    float[] playerStuff = new float[3];
    // Start is called before the first frame update
    void Start()
    {
        player.transform.position = start.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (checkpoint != player.GetComponent<PlayerScript>().currentCheckpoint)
        {
            Checkpoint();
        }
        Death();
    }


    public void Checkpoint()
    {
        //0 == no | 1 == yes
        playerStuff[0] = player.GetComponent<PlayerScript>().hasSword;
        playerStuff[1] = player.GetComponent<PlayerScript>().hasShield;
        playerStuff[2] = player.GetComponent<PlayerScript>().hitpoints;
        checkpoint = player.GetComponent<PlayerScript>().currentCheckpoint;
    }


    public void LoadCheckPoint()
    {
        if (checkpoint)
        {
            player.transform.position = checkpoint.transform.position;
            player.GetComponent<PlayerScript>().hasSword = playerStuff[0];
            player.GetComponent<PlayerScript>().hasShield = playerStuff[1];
            player.GetComponent<PlayerScript>().hitpoints = playerStuff[2];
        }
    }


    private void Death()
    {
        if (player.GetComponent<PlayerScript>().hitpoints < 1)
        {
            LoadCheckPoint();
        }
    }
}
