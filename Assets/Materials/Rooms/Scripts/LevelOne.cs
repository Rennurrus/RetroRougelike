using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Edgar.Unity;

[CreateAssetMenu(menuName = "LevelGenerator", fileName = "LevelOneInputSetup")]
public class LevelOneInput : DungeonGeneratorInputBaseGrid2D
{
    public LevelGraph LevelGraph;
    public LevelOneRoomTemplatesConfig RoomTemplates;

    protected override LevelDescriptionGrid2D GetLevelDescription()
    {
        var levelDescription = new LevelDescriptionGrid2D();

        // Go through individual rooms and add each room to the level description
        // Room templates are resolved based on their type
        foreach (var room in LevelGraph.Rooms.Cast<LevelOneRoom>())
        {
            levelDescription.AddRoom(room, RoomTemplates.GetRoomTemplates(room).ToList());
        }

        // Go through individual connections and for each connection create a corridor room
        foreach (var connection in LevelGraph.Connections.Cast<LevelOneConnection>())
        {
            var corridorRoom = ScriptableObject.CreateInstance<LevelOneRoom>();
            corridorRoom.Type = LevelOneRoomType.Corridor;
            levelDescription.AddCorridorConnection(connection, corridorRoom, RoomTemplates.CorridorTemplates.ToList());
        }

        return levelDescription;
    }

    /// <summary>
    /// Gets room templates for a given room.
    /// </summary>
    private List<GameObject> GetRoomTemplates(RoomBase room)
    {
        // Get room templates from a given room
        var roomTemplates = room.GetRoomTemplates();

        // If the list is empty, we use the defaults room templates from the level graph
        if (roomTemplates == null || roomTemplates.Count == 0)
        {
            var defaultRoomTemplates = LevelGraph.DefaultIndividualRoomTemplates;
            var defaultRoomTemplatesFromSets = LevelGraph.DefaultRoomTemplateSets.SelectMany(x => x.RoomTemplates);

            // Combine individual room templates with room templates from room template sets
            return defaultRoomTemplates.Union(defaultRoomTemplatesFromSets).ToList();
        }

        return roomTemplates;
    }

    /// <summary>
    /// Gets corridor room templates.
    /// </summary>
    private List<GameObject> GetCorridorRoomTemplates()
    {
        var defaultRoomTemplates = LevelGraph.CorridorIndividualRoomTemplates;
        var defaultRoomTemplatesFromSets = LevelGraph.CorridorRoomTemplateSets.SelectMany(x => x.RoomTemplates);

        return defaultRoomTemplates.Union(defaultRoomTemplatesFromSets).ToList();
    }
}