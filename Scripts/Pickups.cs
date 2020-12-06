using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public enum PickupType
{
    Health,
    Ammo,
    Rifle,
    Sniper,
    Bazooka
}

public class Pickups : MonoBehaviour
{

    public PickupType type;
    public int value;
    public int ammo;
    public int reserve;
    public bool isAuto;
    public float fireRate;
    public int focalLength;

    private void OnTriggerEnter(Collider other)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        if (other.CompareTag("Player"))
        {
            Player_Controller player = Game_Manager.instance.GetPlayer(other.gameObject);

            if(type == PickupType.Health)
            {
                player.photonView.RPC("Heal", player.photonPlayer, value);
            }
            else if(type == PickupType.Ammo)
            {
                player.photonView.RPC("GiveAmmo", player.photonPlayer);
            }
            else if(type == PickupType.Rifle)
            {
                player.photonView.RPC("ChangeWeapon", player.photonPlayer, value, ammo, reserve, focalLength, fireRate, isAuto);
            }
            else if(type == PickupType.Sniper)
            {
                player.photonView.RPC("ChangeWeapon", player.photonPlayer, value, ammo, reserve, focalLength, fireRate, isAuto);
            }
            else if(type == PickupType.Bazooka)
            {
                player.photonView.RPC("ChangeWeapon", player.photonPlayer, value, ammo, reserve, focalLength, fireRate, isAuto);
            }

            //gameObject.SetActive(false);
            PhotonNetwork.Destroy(gameObject);

        }
    }

    [PunRPC]
    private void Update()
    {
        if(type == PickupType.Rifle || type == PickupType.Sniper || type == PickupType.Bazooka)
        {
            gameObject.transform.Rotate(0, 1, 0, Space.Self);
        }        
    }
}
