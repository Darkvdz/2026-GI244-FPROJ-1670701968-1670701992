using System.Collections.Generic;
using UnityEngine;

public class CameraControll : MonoBehaviour
{

    public static CameraControll instance;
    private Vector3 offset = new Vector3(0, 0, 0);

    List<Transform> players = new List<Transform>();


    public void Awake()
    {
        instance = this;
    }

    public void LateUpdate()
    {
        if (players.Count == 0) return;

        Vector3 center = GetCenterPosition();

        transform.position = Vector3.Lerp(
            transform.position,
            new Vector3(center.x, center.y, transform.position.z) + offset,
            Time.deltaTime * 5f
        );
    }



    public void AddPlayer(Transform player)
    {
        players.Add(player);
    }

    public void RemovePlayer(Transform player)
    {
        players.Remove(player);
    }

    private Vector3 GetCenterPosition()
    {
        if (players.Count == 0)
            return Vector3.zero;

        Vector3 sum = Vector3.zero;

        foreach (var player in players)
        {
            sum += player.position;
        }

        return sum / players.Count;
    }

}
