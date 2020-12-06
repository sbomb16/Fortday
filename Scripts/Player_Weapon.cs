using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Player_Weapon : MonoBehaviour
{
    [Header("Weapon Stats")]
    public int damage;
    public int curAmmo;
    public int maxAmmo;
    public int ammoReserve;
    public int totalAmmo;
    public float bulletSpeed;
    public float shootRate;
    public float reloadSpeed;

    private float lastShootTime;

    public GameObject bulletPrefab;
    public GameObject rocketPrefab;
    public GameObject[] weapons;
    public GameObject currentWeapon;
    public GameObject magazine;

    public Transform weaponPlacement;
    public Transform bulletSpawnPos;
    public Transform gunLocation;
    public Transform camPlacement;

    private Player_Controller player;

    public bool isAuto = false;

    public AudioClip[] gunSounds;
    public AudioClip gunSound;
    public AudioSource firing;

    private void Awake()
    {
        player = GetComponent<Player_Controller>();
    }

    private void Start()
    {
        if (!player.photonView.IsMine)
        {
            return;
        }
        else
        {
            camPlacement = transform.GetChild(0);
            weaponPlacement = camPlacement.GetChild(0);

            ChangeWeapon(0, 15, 60, 60, 0.2f, false);
        }               
    }

    public void TryShoot()
    {

        if(currentWeapon.name != "Bazooka(Clone)")
        {
            if (curAmmo <= 0 || Time.time - lastShootTime < shootRate)
            {
                return;
            }

            curAmmo--;
            lastShootTime = Time.time;

            player.photonView.RPC("SpawnBullet", RpcTarget.All, bulletSpawnPos.transform.position, Camera.main.transform.forward);

            Game_UI.instance.UpdateAmmoText();
        }
        else
        {
            if (curAmmo <= 0 || Time.time - lastShootTime < shootRate)
            {
                return;
            }

            curAmmo--;
            lastShootTime = Time.time;

            player.photonView.RPC("SpawnRocket", RpcTarget.All, bulletSpawnPos.transform.position, Camera.main.transform.forward);

            Game_UI.instance.UpdateAmmoText();
        }
        
    }

    [PunRPC]
    void SpawnBullet(Vector3 pos, Vector3 dir)
    {

        firing.PlayOneShot(gunSound);

        GameObject bulletObj = Instantiate(bulletPrefab, pos, Quaternion.identity);
        bulletObj.transform.forward = dir;

        Bullet bulletScript = bulletObj.GetComponent<Bullet>();

        bulletScript.Initialize(damage, player.id, player.photonView.IsMine);
        bulletScript.rig.velocity = dir * bulletSpeed;
    
    }

    [PunRPC]
    void SpawnRocket(Vector3 pos, Vector3 dir)
    {

        firing.PlayOneShot(gunSound);

        GameObject rocketObj = Instantiate(rocketPrefab, pos, Quaternion.identity);
        rocketObj.transform.forward = dir;

        Rocket rocketScript = rocketObj.GetComponent<Rocket>();

        rocketScript.Initialize(damage, player.id, player.photonView.IsMine);
        rocketScript.rig.velocity = dir * bulletSpeed;

    }

    [PunRPC]
    public void GiveAmmo()
    {

        int ammoToGive = (int)Mathf.Round(totalAmmo * 0.25f);

        int ammoGiven = ammoReserve + ammoToGive;

        if(ammoGiven > totalAmmo)
        {
            ammoReserve = totalAmmo;
        }
        else
        {
            ammoReserve += ammoToGive;
        }

        Game_UI.instance.UpdateAmmoText();

    }

    [PunRPC]
    public void ChangeWeapon(int weapon, int ammo, int reserve, int focalLength, float fireRate, bool isAuto)
    {

        if (!player.photonView.IsMine)
        {
            return;
        }
        else
        {
            if (currentWeapon)
            {
                PhotonNetwork.Destroy(currentWeapon.gameObject);
            }

            currentWeapon = PhotonNetwork.Instantiate(weapons[weapon].name, weaponPlacement.position, weaponPlacement.transform.rotation);

            currentWeapon.transform.parent = weaponPlacement;

            bulletSpawnPos = currentWeapon.transform.GetChild(0);
            magazine = currentWeapon.transform.GetChild(1).gameObject;

            maxAmmo = ammo;
            curAmmo = (int)(reserve * 0.25f);
            GiveAmmo();

            ammoReserve = reserve;
            totalAmmo = ammoReserve;
            player.focalLength = focalLength;

            if (weapon == 0)
            {
                bulletSpeed = 75f;
                reloadSpeed = 1f;
                damage = 10;
                gunSound = gunSounds[0];
            }
            else if (weapon == 1)
            {
                bulletSpeed = 100f;
                reloadSpeed = 2f;
                damage = 10;
                gunSound = gunSounds[1];
            }
            else if (weapon == 2)
            {
                bulletSpeed = 150f;
                reloadSpeed = 2.5f;
                damage = 34;
                gunSound = gunSounds[2];
            }
            else if (weapon == 3)
            {
                bulletSpeed = 35f;
                reloadSpeed = 3f;
                damage = 50;
                gunSound = gunSounds[3];
            }

            shootRate = fireRate;

            this.isAuto = isAuto;

            Game_UI.instance.UpdateAmmoText();
        }
    }

    [PunRPC]
    public void ReloadWeapon()
    {       

        int ammoReloaded = maxAmmo - curAmmo;
        
        if(ammoReserve == 0 || ammoReloaded == 0)
        {
            return;
        }
        else
        {
            player.isReloading = true;
            StartCoroutine(Reload());
        }

        IEnumerator Reload()
        {          

            if (ammoReserve < ammoReloaded)
            {
                Game_UI.instance.reloadText.text = "Reloading";

                magazine.SetActive(false);

                curAmmo += ammoReserve;
                ammoReserve = 0;
                yield return new WaitForSeconds(reloadSpeed);

                magazine.SetActive(true);

                Game_UI.instance.reloadText.text = "";
            }
            else
            {
                Game_UI.instance.reloadText.text = "Reloading";

                magazine.SetActive(false);

                curAmmo = maxAmmo;
                ammoReserve -= ammoReloaded;
                yield return new WaitForSeconds(reloadSpeed);

                magazine.SetActive(true);

                Game_UI.instance.reloadText.text = "";
            }

            Game_UI.instance.UpdateAmmoText();
            player.isReloading = false;

        }
    }

    public void ScopeIn(int focalLength)
    {
        camPlacement.GetComponent<Camera>().focalLength = focalLength;
    }

    public void ScopeOut()
    {
        camPlacement.GetComponent<Camera>().focalLength = 12;
    }
}
