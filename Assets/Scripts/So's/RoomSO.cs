using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Room", menuName = "SO's/RoomSO")]
public class RoomSO : ScriptableObject
{
    public string roomName = "1-1";
    public RoomType roomType;
    public DificultyLevel dificultyLevel;
}

public enum RoomType { Default_Room, Corrupted_Room /* Heavy room, with extra enemies and more difficult */, BossRoom, ShopRoom}
public enum DificultyLevel { Default_1, Default_2 } // etc
