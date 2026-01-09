using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [Header("Room Setup")]
    [SerializeField] private Transform startPos;
    [SerializeField] private Transform endPos; 
    [SerializeField] private float endPosRadius = 2f; // also show this visually (Teleportation visual)
    [SerializeField] private GameObject teleportParticle;

    public event Action<Room> OnRequestNextRoom;
    
    private RoomSO roomData;
    private bool roomCleard = false;
    private Transform player;

    public void Init(RoomSO roomSOData)
    {
        roomData = roomSOData;
        player = GameObject.FindGameObjectWithTag("Player").transform;

        LevelStart();
    }

    private void Update()
    {

        if(Input.GetKeyDown(KeyCode.V))
            MarkRoomCleard();


        if (!roomCleard) return;

        float distance = Vector3.Distance(player.position, endPos.position);
        //bool temp = true;

        if (distance < endPosRadius)
        {
            // Fire the event once
            OnRequestNextRoom?.Invoke(this);
            roomCleard = false; // prevent double triggers
        }
    }

    public void MarkRoomCleard()
    {
        roomCleard = true;

        if (teleportParticle != null)
            teleportParticle.SetActive(true);

    }


    /// <summary>
    /// Invoke this method to start the level.
    /// this method can notificate GameManager to activate everything (player movement, Enemy spawner etc.)
    /// </summary>
    private void LevelStart()
    {
       player.position = startPos.position;
    }
}
