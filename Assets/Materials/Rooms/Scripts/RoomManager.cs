using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Edgar.Unity;

public class RoomManager : MonoBehaviour
{
    public RoomInstanceGrid2D RoomInstance;
    
    public void OnRoomEnter(GameObject player)
    {
        Debug.Log($"Room enter. Room name: {RoomInstance.Room.GetDisplayName()}, Room template: {RoomInstance.RoomTemplatePrefab.name}");
        //RoomManager.Instance.OnRoomEnter(RoomInstance);
    }

    public void OnRoomLeave(GameObject player)
    {
        Debug.Log($"Room leave {RoomInstance.Room.GetDisplayName()}");
        //RoomManager.Instance.OnRoomLeave(RoomInstance);
    }
}
