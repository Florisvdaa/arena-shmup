using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelFlowManager : MonoBehaviour
{
    public RoomType roomType;
    public DificultyLevel DificultyLevel;

    private Room currentRoom;

    private void Start()
    {
        GenerateRoom();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
            GenerateRoom();
    }

    private void GenerateRoom()
    {
        // Destory old room if it exists
        if (currentRoom != null)
        {
            currentRoom.OnRequestNextRoom -= HandleNextRoomRequest;
            Destroy(currentRoom.gameObject);
        }

        // Pick random room data
        RoomSO randomRoom = RoomManager.Instance.GetRandomRoomData(roomType, DificultyLevel);

        // spawn new room
        currentRoom = RoomManager.Instance.SpawnRoom(randomRoom, Vector3.zero);

        currentRoom.OnRequestNextRoom += HandleNextRoomRequest;
    }

    private void HandleNextRoomRequest(Room room) { GenerateRoom(); }
}
