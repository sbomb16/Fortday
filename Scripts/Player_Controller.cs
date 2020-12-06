using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Player_Controller : MonoBehaviourPun
{

    [Header("Stats")]
    public float moveSpeed;
    public float jumpForce;
    public int curHp;
    public int maxHP;
    public int kills;
    public bool dead;
    private bool flashingDamage;
    public MeshRenderer mr;

    [Header("Components")]
    public Rigidbody rig;

    public int id;
    public Player photonPlayer;

    private int curAttackerId;

    public Player_Weapon weapon;

    public bool isReloading = false;
    public int focalLength;

    // Update is called once per frame
    void Update()
    {

        if(!photonView.IsMine || dead)
        {
            return;
        }

        Move();

        bool leftMouseIsDown = Input.GetMouseButton(1);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            TryJump();
        }

        if (Input.GetKeyDown(KeyCode.R) || weapon.curAmmo == 0 && weapon.ammoReserve > 0 && weapon.curAmmo != weapon.maxAmmo)
        {
            weapon.ReloadWeapon();
        }

        if (Input.GetMouseButtonDown(0) && !weapon.isAuto && !isReloading)
        {
            weapon.TryShoot();
        }
        else if(Input.GetMouseButton(0) && weapon.isAuto && !isReloading)
        {
            weapon.TryShoot();
        }

        if(leftMouseIsDown)
        {
            weapon.ScopeIn(focalLength);
        }
        else if(!leftMouseIsDown)
        {
            weapon.ScopeOut();
        }
    }

    void Move()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 dir = (transform.forward * z + transform.right * x) * moveSpeed;
        dir.y = rig.velocity.y;

        rig.velocity = dir;
    }

    void TryJump()
    {
        Ray ray = new Ray(transform.position, Vector3.down);

        if(Physics.Raycast(ray, 1.5f))
        {
            rig.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    [PunRPC]
    public void Initialize(Player player)
    {
        id = player.ActorNumber;
        photonPlayer = player;

        Game_Manager.instance.players[id - 1] = this;

        if (!photonView.IsMine)
        {
            GetComponentInChildren<Camera>().gameObject.SetActive(false);
            rig.isKinematic = true;
        }
        else
        {
            Game_UI.instance.Initialize(this);
        }
    }

    [PunRPC]
    public void TakeDamage(int attackerID, int damage)
    {
        if (dead)
        {
            return;
        }

        curHp -= damage;
        curAttackerId = attackerID;

        photonView.RPC("DamageFlash", RpcTarget.Others);

        if (curHp <= 0)
        {
            photonView.RPC("Die", RpcTarget.All);
        }

        Game_UI.instance.UpdateHealthBar();

    }

    [PunRPC]
    void DamageFlash()
    {
        if (flashingDamage)
        {
            return;
        }

        StartCoroutine(DamageFlashCoRoutine());

        IEnumerator DamageFlashCoRoutine()
        {
            flashingDamage = true;

            Color defaultColor = mr.material.color;
            mr.material.color = Color.red;

            yield return new WaitForSeconds(0.05f);

            mr.material.color = defaultColor;
            flashingDamage = false;
        }
    }

    [PunRPC]
    void Die()
    {
        curHp = 0;
        dead = true;

        Game_Manager.instance.alivePlayers--;

        if (PhotonNetwork.IsMasterClient)
        {
            Game_Manager.instance.CheckWinCondition();
        }

        if (photonView.IsMine)
        {
            if(curAttackerId != 0)
            {
                Game_Manager.instance.GetPlayer(curAttackerId).photonView.RPC("AddKill", RpcTarget.All);
            }

            GetComponentInChildren<Camera_Controller>().SetAsSpectator();

            rig.isKinematic = true;
            transform.position = new Vector3(0, -50, 0);
        }
    }

    [PunRPC]
    public void AddKill()
    {

        Game_UI.instance.UpdatePlayerInfoText();

        kills++;
    }

    [PunRPC]
    public void Heal(int amountToHeal)
    {
        curHp = Mathf.Clamp(curHp + amountToHeal, 0, maxHP);

        Game_UI.instance.UpdateHealthBar();
    }    
}