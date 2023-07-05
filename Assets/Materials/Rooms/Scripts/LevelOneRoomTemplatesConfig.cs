using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Edgar.Unity;

[Serializable]
public class LevelOneRoomTemplatesConfig 
{
    public GameObject[] DefaultRoomTemplates;
    //public GameObject[] ShopRoomTemplates;
    //public GameObject[] LockdownRoomTemplates;
    //public GameObject[] RewardRoomTemplates;
    //public GameObject[] BossRoomTemplates;
    public GameObject[] EntranceRoomTemplates;
    public GameObject[] ExitRoomTemplates;
    //public GameObject[] SecretRoomTemplates;

    public GameObject[] CorridorTemplates;

    public GameObject[] GetRoomTemplates(LevelOneRoom room)
    {
        switch (room.Type)
        {
            /*case LevelOneRoomType.Shop:
                return ShopRoomTemplates;

            case LevelOneRoomType.Lockdown:
                return LockdownRoomTemplates;

            case LevelOneRoomType.Reward:
                return RewardRoomTemplates;

            case LevelOneRoomType.Boss:
                return BossRoomTemplates;*/

            case LevelOneRoomType.Entrance:
                return EntranceRoomTemplates;

            case LevelOneRoomType.Exit:
                return ExitRoomTemplates;

            /*case LevelOneRoomType.Secret:
                return SecretRoomTemplates;*/

            case LevelOneRoomType.Corridor:
                return CorridorTemplates;
            

            default:
                return DefaultRoomTemplates;
        }
    }
}
