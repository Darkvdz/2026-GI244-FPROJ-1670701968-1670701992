using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviourPunCallbacks
{
    public List<Transform> spawnPoints;
    public float minTime = 3f;
    public float maxTime = 8f;

    public float waitTimeSpawn = 5f;
    
    public int maxItemSpawn = 2;
    public int currentItem = 0;

    public static ItemManager instance;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {

            waitTimeSpawn = Random.Range(minTime, maxTime);

            StartCoroutine(SpawnLoop());


        }
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            if (currentItem < maxItemSpawn) 
            {
                float waitTime = Random.Range(minTime, maxTime);
                yield return new WaitForSeconds(waitTime);

                SpawnItem();
            }
            
        }
    }

    private void SpawnItem()
    {
        int index = Random.Range(0, spawnPoints.Count);

        var go = PhotonNetwork.Instantiate(
            "Item/ItemTest",
            spawnPoints[index].position,
            Quaternion.identity
        );

        Item goItem = go.GetComponent<Item>();

        if (goItem) 
        {
            goItem.returnSpawn = spawnPoints[index];
            spawnPoints.RemoveAt(index);
        }
        else 
        {
            Debug.Log("error Item spawn not found Item script");
        }

        currentItem++;

    }

    public void ReturnSpawnPointItem(Transform positionSpawn) 
    {
        spawnPoints.Add(positionSpawn);
    }



}