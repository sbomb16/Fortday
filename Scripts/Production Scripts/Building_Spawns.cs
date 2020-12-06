using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Building_Spawns : MonoBehaviour
{

    public GameObject[] spawnPoints;
    public GameObject[] buildingPrefabs;

    // Start is called before the first frame update
    void Start()
    {

        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < spawnPoints.Length - 1; i++)
            {

                int buildingRand = Random.Range(0, 3);
                Quaternion rot = Quaternion.Euler(0, Random.Range(0, 359), 0);

                if (buildingRand == 1)
                {
                    spawnPoints[i] = PhotonNetwork.Instantiate(buildingPrefabs[buildingRand].name, new Vector3(spawnPoints[i].transform.position.x, spawnPoints[i].transform.position.y + 3, spawnPoints[i].transform.position.z), rot);
                }
                else
                {
                    spawnPoints[i] = PhotonNetwork.Instantiate(buildingPrefabs[buildingRand].name, spawnPoints[i].transform.position, rot);
                }



            }
        }        
    }
}
