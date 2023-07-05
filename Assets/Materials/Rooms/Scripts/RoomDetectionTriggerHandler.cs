using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Edgar.Unity;

public class RoomDetectionTriggerHandler : MonoBehaviour
{
    private RoomManager roomManager;

    public void Start()
    {
        roomManager = transform.parent.parent.gameObject.GetComponent<RoomManager>();
    }

    public void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if (otherCollider.gameObject.tag == "Player")
        {
            roomManager?.OnRoomEnter(otherCollider.gameObject);
        }
    }

    public void OnTriggerExit2D(Collider2D otherCollider)
    {
        if (otherCollider.gameObject.tag == "Player")
        {
            roomManager?.OnRoomLeave(otherCollider.gameObject);
        }
    }
}
