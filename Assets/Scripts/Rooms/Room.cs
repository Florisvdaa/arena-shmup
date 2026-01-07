using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    private RoomSO roomData;

    public void Init(RoomSO roomSOData)
    {
        roomData = roomSOData;
    }
}
