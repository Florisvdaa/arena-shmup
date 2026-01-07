using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{

    [SerializeField] private List<RoomSO> roomTypes = new();
    [SerializeField] private List<RoomPrefabEntry> roomPrefabs;

    /// <summary>
    /// Gives you full control over which prefab matches which room type
    /// </summary>
    /// <param name="roomData"></param>
    /// <param name="position"></param>
    /// <returns></returns>

    public Room SpawnRoom(RoomSO roomData, Vector3 position)
    {
        GameObject roomPrefab = GetPrefabForRoomType(roomData.roomType);
        GameObject instance = Instantiate(roomPrefab, position, Quaternion.identity);

        Room room = instance.GetComponent<Room>();
        room.Init(roomData);

        return room;
    }


    /// <summary>
    /// When RoomType == Default_Room this will return a default room prefab.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public GameObject GetPrefabForRoomType(RoomType type)
    {
        foreach (var entry in roomPrefabs)
        {
            if (entry.type == type)
            {
                if (entry.prefabs.Count == 0)
                {
                    Debug.LogError($"No prefabs assigned for RoomType: {type}");
                    return null;
                }

                int index = Random.Range(0, entry.prefabs.Count); // random room choice per room type
                return entry.prefabs[index];
            }
        }

        Debug.LogError($"RoomType not found: {type}");
        return null;

    }

    public RoomSO GetRandomRoomData(RoomType type, DificultyLevel difficulty)
    {
        List<RoomSO> filtered = new();

        foreach (var room in roomTypes)
        {
            if (room.roomType == type && room.dificultyLevel == difficulty)
                filtered.Add(room);
        }

        if (filtered.Count == 0)
        {
            Debug.LogError($"No RoomSO found for type {type} and difficulty {difficulty}");
            return null;
        }

        return filtered[Random.Range(0, filtered.Count)];
    }
}
