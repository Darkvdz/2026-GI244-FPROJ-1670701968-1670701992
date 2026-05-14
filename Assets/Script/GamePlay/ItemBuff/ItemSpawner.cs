using UnityEngine;
using Photon.Pun;
using System.Collections.Generic; 
public class ItemSpawner : MonoBehaviourPun
{
    
    public Transform[] spawnPoints;
    public string[] itemPrefabNames;

    public float minSpawnTime = 5f;
    public float maxSpawnTime = 10f;

    private GameObject[] spawnedItems;
    private float spawnTimer;

    private void Start()
    {
        if (spawnPoints == null || spawnPoints.Length == 0) 
            return;

        spawnedItems = new GameObject[spawnPoints.Length];

        if (PhotonNetwork.IsMasterClient)
        {
            spawnTimer = Random.Range(minSpawnTime, maxSpawnTime);
            
            //InitialFullSpawn();
        }
    }

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient) 
            return;

        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            SpawnItemUntilFull();
            spawnTimer = Random.Range(minSpawnTime, maxSpawnTime);
        }
    }

    void SpawnItemUntilFull()
    {
        List<int> availableIndices = new List<int>();
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (spawnedItems[i] == null)
            {
                availableIndices.Add(i);
            }
        }

        if (availableIndices.Count > 0)
        {
          
            int randomListIndex = Random.Range(0, availableIndices.Count);
            int chosenSpawnIndex = availableIndices[randomListIndex];

            int randomItemIndex = Random.Range(0, itemPrefabNames.Length);
            string selectedItem = itemPrefabNames[randomItemIndex];

            GameObject newItem = PhotonNetwork.InstantiateRoomObject(selectedItem, spawnPoints[chosenSpawnIndex].position, Quaternion.identity);
            spawnedItems[chosenSpawnIndex] = newItem;

            Debug.Log($"[Spawner] Spawned {selectedItem} at Point {chosenSpawnIndex}");
        }
    }
    //for test
    //void InitialFullSpawn()
    //{
    //    for (int i = 0; i < spawnPoints.Length; i++)
    //    {
    //        int randomItemIndex = Random.Range(0, itemPrefabNames.Length);
    //        string selectedItem = itemPrefabNames[randomItemIndex];

    //        GameObject newItem = PhotonNetwork.InstantiateRoomObject(selectedItem, spawnPoints[i].position, Quaternion.identity);
    //        spawnedItems[i] = newItem;
    //    }
    //}
}