﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Force_Field : MonoBehaviour
{

    public float shrinkWaitTime;
    public float shrinkAmount;
    public float shrinkDuration;
    public float minShrinkAmount;

    public int playerDamage;

    private float lastShrinkingEndTime;
    private bool shrinking;
    private float targetDiameter;
    private float lastPlayerCheckTime;

    // Start is called before the first frame update
    void Start()
    {
        lastShrinkingEndTime = Time.time;
        targetDiameter = transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        if (shrinking)
        {
            transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.one * targetDiameter, (shrinkAmount / shrinkDuration) * Time.deltaTime);
            if(transform.localScale.x == targetDiameter)
            {
                shrinking = false;
            }
        }
        else
        {
            if(Time.timeSinceLevelLoad - lastShrinkingEndTime >= shrinkWaitTime && transform.localScale.x > minShrinkAmount)
            {
                Shrink();
            }
        }

        CheckPlayers();

    }

    void Shrink()
    {
        shrinking = true;

        if(transform.localScale.x - shrinkAmount > minShrinkAmount)
        {
            targetDiameter -= shrinkAmount;
        }
        else
        {
            targetDiameter = minShrinkAmount;
        }
        lastShrinkingEndTime = Time.time + shrinkDuration;
    }

    void CheckPlayers()
    {
        if(Time.time - lastPlayerCheckTime > 1.0f)
        {
            lastPlayerCheckTime = Time.time;

            foreach(Player_Controller player in Game_Manager.instance.players)
            {
                if(player.dead || !player)
                {
                    continue;
                }
                if(Vector3.Distance(Vector3.zero, player.transform.position) >= transform.localScale.x)
                {
                    player.photonView.RPC("TakeDamage", player.photonPlayer, 1, playerDamage);
                }
            }
        }
    }
}
