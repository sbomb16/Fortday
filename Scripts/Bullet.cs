using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    private int damage;
    private int attackerID;
    private bool isMine;

    public Rigidbody rig;

    public void Initialize(int damage, int attackerID, bool isMine)
    {
        this.damage = damage;
        this.attackerID = attackerID;
        this.isMine = isMine;

        Destroy(gameObject, 5.0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && isMine)
        {
            Player_Controller player = Game_Manager.instance.GetPlayer(other.gameObject);

            if(player.id != attackerID)
            {
                player.photonView.RPC("TakeDamage", player.photonPlayer, attackerID, damage);
            }
        }

        Destroy(gameObject);

    }
}
