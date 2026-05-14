using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviourPun
{
    public List<Transform> spawnPoints;
    public float minTime = 3f;
    public float maxTime = 8f;

    private float waitTimeSpawn;
    public int maxItemSpawn = 2;

    public string[] itemPrefabs = {
        "Item/ItemGun", "Item/ItemSword",
        "Item/ItemHeavyGun","Item/ItemAxe","Item/ItemBoomerang"
    };

    public static ItemManager instance;

    private float currentTimer = 0f;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        waitTimeSpawn = Random.Range(minTime, maxTime);
    }

    void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        currentTimer += Time.deltaTime;

        if (currentTimer >= waitTimeSpawn)
        {
            CheckAndSpawnItem();

            currentTimer = 0f;
            waitTimeSpawn = Random.Range(minTime, maxTime);
        }
    }

    private void CheckAndSpawnItem()
    {

        Item[] activeItemsInScene = FindObjectsByType<Item>(FindObjectsSortMode.None);

        if (activeItemsInScene.Length < maxItemSpawn)
        {
            SpawnItem();
        }
    }

    private void SpawnItem()
    {
       
        List<Transform> availablePoints = new List<Transform>();

        foreach (Transform pt in spawnPoints)
        {
            Collider2D col = Physics2D.OverlapCircle(pt.position, 0.5f);
            if (col == null || col.GetComponent<Item>() == null)
            {
                availablePoints.Add(pt); 
            }
        }

        if (availablePoints.Count > 0)
        {
            int index = Random.Range(0, availablePoints.Count);
            int randomItemIndex = Random.Range(0, itemPrefabs.Length);
            string itemToSpawn = itemPrefabs[randomItemIndex];

            var go = PhotonNetwork.InstantiateRoomObject(
                 itemToSpawn,
                 availablePoints[index].position,
                 Quaternion.identity
             );

            Item goItem = go.GetComponent<Item>();
            if (goItem != null)
            {
                goItem.returnSpawn = availablePoints[index];
            }

        }
    }
}