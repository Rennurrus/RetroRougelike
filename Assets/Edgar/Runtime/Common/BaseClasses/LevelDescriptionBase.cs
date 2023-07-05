﻿using System;
using System.Collections.Generic;
using System.Linq;
using Edgar.GraphBasedGenerator.Grid2D;
using Edgar.Graphs;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;
using Edgar.Unity.Diagnostics;
using UnityEngine;

namespace Edgar.Unity
{
    public abstract class LevelDescriptionBase
    {
        private readonly List<ConnectionBase> connections = new List<ConnectionBase>();
        private readonly List<RoomDescriptionGrid2D> corridorRoomDescriptions = new List<RoomDescriptionGrid2D>();

        private readonly TwoWayDictionary<RoomBase, ConnectionBase> corridorToConnectionMapping = new TwoWayDictionary<RoomBase, ConnectionBase>();
        private readonly LevelDescriptionGrid2D<RoomBase> levelDescription = new LevelDescriptionGrid2D<RoomBase>();
        private readonly TwoWayDictionary<GameObject, RoomTemplateGrid2D> prefabToRoomTemplateMapping = new TwoWayDictionary<GameObject, RoomTemplateGrid2D>();

        /// <summary>
        /// Adds a given room together with a list of available room templates.
        /// </summary>
        /// <param name="room">Room that is added to the level description.</param>
        /// <param name="roomTemplates">Room templates that are available for the room.</param>
        public void AddRoom(RoomBase room, List<GameObject> roomTemplates)
        {
            if (room == null) throw new ArgumentNullException(nameof(room));
            if (roomTemplates == null) throw new ArgumentNullException(nameof(roomTemplates));
            if (roomTemplates.Count == 0) throw new ArgumentException($"There must be at least one room template for each room. Room: {room}", nameof(roomTemplates));

            levelDescription.AddRoom(room, GetBasicRoomDescription(roomTemplates));
        }

        /// <summary>
        /// Adds a given connection without a corridor between the two rooms.
        /// </summary>
        /// <param name="connection">Connection that is added to the level description</param>
        public void AddConnection(ConnectionBase connection)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));

            connections.Add(connection);
            levelDescription.AddConnection(connection.From, connection.To);
        }

        /// <summary>
        /// Adds a given connection together with a corridor room between the two rooms.
        /// </summary>
        /// <param name="connection">Connection that is added to the level description</param>
        /// <param name="corridorRoom">Room that represents the corridor room between the two rooms from the connection</param>
        /// <param name="corridorRoomTemplates">Room templates that are available for the corridor</param>
        public void AddCorridorConnection(ConnectionBase connection, RoomBase corridorRoom, List<GameObject> corridorRoomTemplates)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (corridorRoom == null) throw new ArgumentNullException(nameof(corridorRoom));
            if (corridorRoomTemplates.Count == 0) throw new ArgumentException($"There must be at least one room template for each corridor room. Room: {corridorRoom}", nameof(corridorRoom));

            connections.Add(connection);
            corridorToConnectionMapping.Add(corridorRoom, connection);

            var corridorRoomDescription = GetCorridorRoomDescription(corridorRoomTemplates);
            levelDescription.AddRoom(corridorRoom, corridorRoomDescription);
            levelDescription.AddConnection(connection.From, corridorRoom);
            levelDescription.AddConnection(corridorRoom, connection.To);
        }

        private RoomDescriptionGrid2D GetBasicRoomDescription(List<GameObject> roomTemplatePrefabs)
        {
            return new RoomDescriptionGrid2D(false, roomTemplatePrefabs.Select(GetRoomTemplate).ToList());
        }

        private RoomDescriptionGrid2D GetCorridorRoomDescription(List<GameObject> roomTemplatePrefabs)
        {
            foreach (var existingRoomDescription in corridorRoomDescriptions)
            {
                var existingPrefabs = existingRoomDescription
                    .RoomTemplates
                    .Select(x => prefabToRoomTemplateMapping.GetByValue(x))
                    .ToList();

                if (existingPrefabs.SequenceEqual(roomTemplatePrefabs))
                {
                    return existingRoomDescription;
                }
            }

            var corridorRoomDescription = new RoomDescriptionGrid2D(true, roomTemplatePrefabs.Select(GetRoomTemplate).ToList());
            corridorRoomDescriptions.Add(corridorRoomDescription);

            return corridorRoomDescription;
        }

        protected abstract bool TryGetRoomTemplate(GameObject roomTemplatePrefab, out RoomTemplateGrid2D roomTemplate, out ActionResult result);

        private RoomTemplateGrid2D GetRoomTemplate(GameObject roomTemplatePrefab)
        {
            if (prefabToRoomTemplateMapping.ContainsKey(roomTemplatePrefab))
            {
                return prefabToRoomTemplateMapping[roomTemplatePrefab];
            }

            if (TryGetRoomTemplate(roomTemplatePrefab, out var roomTemplate, out var result))
            {
                prefabToRoomTemplateMapping.Add(roomTemplatePrefab, roomTemplate);
                return roomTemplate;
            }

            Debug.LogError($"There was a problem when loading the room template \"{roomTemplatePrefab.name}\":");
            foreach (var error in result.Errors)
            {
                Debug.LogError($"- {error}");
            }

            throw new ConfigurationException("Please fix all the errors above and try again");
        }

        /// <summary>
        /// Gets the map description.
        /// </summary>
        /// <returns></returns>
        internal LevelDescriptionGrid2D<RoomBase> GetLevelDescription()
        {
            return levelDescription;
        }


        /// <summary>
        /// Gets the mapping from room template game objects to room template instances.
        /// </summary>
        /// <returns></returns>
        internal TwoWayDictionary<GameObject, RoomTemplateGrid2D> GetPrefabToRoomTemplateMapping()
        {
            return prefabToRoomTemplateMapping;
        }

        /// <summary>
        /// Gets the mapping from corridor rooms to corresponding connections.
        /// </summary>
        /// <returns></returns>
        internal TwoWayDictionary<RoomBase, ConnectionBase> GetCorridorToConnectionMapping()
        {
            return corridorToConnectionMapping;
        }

        /// <summary>
        /// Gets the graph of rooms.
        /// </summary>
        /// <remarks>
        /// The graph is not updated when new rooms are added to the level description.
        /// Adding rooms to the graph does not update the level description.
        /// This behaviour may change in the future.
        /// </remarks>
        /// <returns></returns>
        public IGraph<RoomBase> GetGraph()
        {
            return levelDescription.GetGraphWithoutCorridors();
        }

        /// <summary>
        /// Gets the graph of rooms where also corridors are considered to be rooms.
        /// </summary>
        /// <remarks>
        /// The graph is not updated when new rooms are added to the level description.
        /// Adding rooms to the graph does not update the level description.
        /// This behaviour may change in the future.
        /// </remarks>
        /// <returns></returns>
        public IGraph<RoomBase> GetGraphWithCorridors()
        {
            return levelDescription.GetGraph();
        }
    }
}