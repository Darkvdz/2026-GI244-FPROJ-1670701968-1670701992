using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PhotonPoolManager : MonoBehaviour, IPunPrefabPool
{
    public GameObject bulletPrefab; 
    public int poolSize = 100;

    private List<GameObject> bulletPool = new List<GameObject>();

    void Start()
    {
        
        PhotonNetwork.PrefabPool = this;

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(bulletPrefab);

            obj.transform.SetParent(this.transform);//hide in poolmng

            obj.SetActive(false);
            bulletPool.Add(obj);
        }
    }

    public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
    {
        if (prefabId == "Bullet")
        {
            foreach (GameObject bullet in bulletPool)
            {
                if (!bullet.activeSelf)
                {
                    bullet.transform.position = position;
                    bullet.transform.rotation = rotation;
                        
                    return bullet;
                }
            }

            //---//
            GameObject newBullet = Instantiate(bulletPrefab, position, rotation);
            newBullet.transform.SetParent(this.transform);
            newBullet.SetActive(false); 
            bulletPool.Add(newBullet);
            return newBullet;
        }

        GameObject fallbackObj = (GameObject)UnityEngine.Object.Instantiate(Resources.Load(prefabId), position, rotation);
        fallbackObj.SetActive(false);
        return fallbackObj;
    }

    public void Destroy(GameObject gameObject)
    {
        
        if (gameObject.GetComponent<Bullet>() != null)
        {
            gameObject.SetActive(false);
        }
        else
        {
            UnityEngine.Object.Destroy(gameObject);
        }
    }
}