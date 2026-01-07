using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct RoomPrefabEntry
{
    public RoomType type;
    public List<GameObject> prefabs; // Multiple prefabs per type
}
