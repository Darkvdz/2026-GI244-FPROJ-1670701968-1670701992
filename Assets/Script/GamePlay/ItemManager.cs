using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviourPun
{
    public List<Transform> spawnPoints;
    public float minTime = 3f;
    public float maxTime = 8f;

    public float waitTimeSpawn = 5f;
    
    public int maxItemSpawn = 2;
    public int currentItem = 0;
    public string[] itemPrefabs = { "Item/ItemGun", "Item/ItemSword", "Item/ItemHeavyGun" };

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
                yield return new WaitForSeconds(waitTimeSpawn);
                waitTimeSpawn = Random.Range(minTime, maxTime);

                SpawnItem();
            }
            else 
            {
                yield return null;
                Debug.Log("spawn Full");
            }
        }
    }

    private void SpawnItem()
    {
        int index = Random.Range(0, spawnPoints.Count);

        int randomItemIndex = Random.Range(0, itemPrefabs.Length);
        string itemToSpawn = itemPrefabs[randomItemIndex];

        var go = PhotonNetwork.Instantiate(
             itemToSpawn,
             spawnPoints[index].position,
             Quaternion.identity
         );

        Item goItem = go.GetComponent<Item>();

        if (goItem) 
        {
            goItem.returnSpawn = spawnPoints[index];
            spawnPoints.RemoveAt(index);

            currentItem++;
        }
        else 
        {
            Debug.Log("error Item spawn not found Item script");
        }

    }

    public void ReturnSpawnPointItem(Transform positionSpawn) 
    {
        currentItem--;
        spawnPoints.Add(positionSpawn);
    }



}