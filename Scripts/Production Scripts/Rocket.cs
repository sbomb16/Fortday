using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    
    public int explosionSize;

    public Rigidbody rig;
    public GameObject explosion;
    public Explosion rocket;

    public void Initialize(int damage, int attackerID, bool isMine)
    {
        //rocket = explosion.GetComponent<Explosion>();

        rocket.damage = damage;
        rocket.attackerID = attackerID;
        rocket.isMine = isMine;

        Destroy(gameObject, 5.0f);
    }

    private void OnTriggerEnter(Collider other)
    {      
        Explode();
        rig.isKinematic = true;
        gameObject.GetComponent<Collider>().enabled = false;
    }

    [PunRPC]
    void Explode()
    {
        StartCoroutine(CauseExplosion());

        IEnumerator CauseExplosion()
        {
            AudioSource explosionSound = explosion.GetComponent<AudioSource>();
            explosionSound.Play();

            for (int i = 1; i < explosionSize; i++)
            {
                //Debug.Log("Bababooey");
                yield return new WaitForSeconds(.001f);
                explosion.transform.localScale = new Vector3(i, i, i);
            }
            yield return new WaitForSeconds(.5f);

            for (int j = explosionSize; j > 1; j--)
            {
                //Debug.Log("Bababooey");
                yield return new WaitForSeconds(.001f);
                explosion.transform.localScale = new Vector3(j, j, j);
            }
            yield return new WaitForSeconds(.5f);
            Destroy(gameObject);
        }
        //Destroy(gameObject);
    }
}
