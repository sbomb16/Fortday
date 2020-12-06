using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class Game_Manager : MonoBehaviourPun
{

    [Header("Players")]
    public string playerPrefabLocation;
    public Player_Controller[] players;
    public Transform[] spawnPoints;
    public int alivePlayers;

    private int playersInGame;   

    public static Game_Manager instance;

    public float postGameTime;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        players = new Player_Controller[PhotonNetwork.PlayerList.Length];
        alivePlayers = players.Length;

        photonView.RPC("ImInGame", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void ImInGame()
    {
        playersInGame++;

        if (PhotonNetwork.IsMasterClient && playersInGame == PhotonNetwork.PlayerList.Length)
        {
            photonView.RPC("SpawnPlayer", RpcTarget.All);
        }
    }

    [PunRPC]
    void SpawnPlayer()
    {
        GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabLocation, spawnPoints[Random.Range(0, spawnPoints.Length)].position, Quaternion.identity);

        playerObj.GetComponent<Player_Controller>().photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
    }
    
    public Player_Controller GetPlayer(int playerID)
    {

        foreach(Player_Controller player in players)
        {
            if(player != null && player.id == playerID)
            {
                return player;
            }
        }

        return null;
    }

    public Player_Controller GetPlayer(GameObject playerObj)
    {

        foreach(Player_Controller player in players)
        {
            if(player != null && player.gameObject == playerObj)
            {
                return player;
            }
        }

        return null;
    }

    public void CheckWinCondition()
    {
        if(alivePlayers == 1)
        {
            photonView.RPC("WinGame", RpcTarget.All, players.First(x => !x.dead).id);
        }
    }

    [PunRPC]
    void WinGame(int winningPlayer)
    {

        Game_UI.instance.SetWinText(GetPlayer(winningPlayer).photonPlayer.NickName);

        Invoke("GoBackToMenu", postGameTime);
    }

    void GoBackToMenu()
    {
        Network_Manager.instance.ChangeScenes("Menu_Scene");
    }
}
