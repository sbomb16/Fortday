using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{

    public int damage;
    public int attackerID;
    public bool isMine;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && isMine)
        {
            Player_Controller player = Game_Manager.instance.GetPlayer(other.gameObject);

            if (player.id != attackerID)
            {
                player.photonView.RPC("TakeDamage", player.photonPlayer, attackerID, damage);
            }
        }
    }
}
