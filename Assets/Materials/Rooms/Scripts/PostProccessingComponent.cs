using Edgar.Unity;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "LevelGenerator/PostProccessingComponent", fileName = "PostProccessingComponent")]
public class PostProccessingComponent : DungeonGeneratorPostProcessingGrid2D 
{
    public GameObject[] Enemies;

    public override void Run(DungeonGeneratorLevelGrid2D level)
    {   
        Enemies = Resources.LoadAll<GameObject>("Enemies");

        foreach (var roomInstance in level.RoomInstances)
        {
            var roomTemplateInstance = roomInstance.RoomTemplateInstance;

            // Найдём слой тайлсета задней стенки комнаты
            var tilemaps = RoomTemplateUtilsGrid2D.GetTilemaps(roomTemplateInstance);
            var background = tilemaps.Single(x => x.name == "Background").gameObject;

            // Добавим коллайдер для задней стенки комнаты
            AddBackgroundCollider(background);

            // Добавим компонент обнаружения различных объектов в коллайдере
            background.AddComponent<RoomDetectionTriggerHandler>();

            // Добавим менеджер комнаты
            var roomManager = roomTemplateInstance.AddComponent<RoomManager>();
            roomManager.RoomInstance = roomInstance;
        }

        //Transform spawnPoint = GameObject.Find("SpawnPoint").transform;
        //GameObject player = GameObject.FindWithTag("Player");
        //player.transform.position = spawnPoint.position;

        SetSpawnPosition(level);
    }

    /*private void SpawnEnemies(DungeonGeneratorLevelGrid2D level)
    {
        // Check that we have at least one enemy to choose from
        if (Enemies == null || Enemies.Length == 0)
        {
            throw new InvalidOperationException("There must be at least one enemy prefab to spawn enemies");
        }

        // Go through individual rooms
        foreach (var roomInstance in level.RoomInstances)
        {
            var roomTemplate = roomInstance.RoomTemplateInstance;

            // Find the game object that holds all the spawn points
            var enemySpawnPoints = roomTemplate.transform.Find("EnemySpawnPoints");

            if (enemySpawnPoints != null)
            {
                // Go through individual spawn points and choose a random enemy to spawn
                foreach (Transform enemySpawnPoint in enemySpawnPoints)
                {
                    var enemyPrefab = Enemies[Random.Next(Enemies.Length)];
                    var enemy = Instantiate(enemyPrefab);
                    enemy.transform.parent = roomTemplate.transform;
                    enemy.transform.position = enemySpawnPoint.position;
                }
            }
        }
    }*/

    private void AddBackgroundCollider(GameObject background)
    {
        var tilemapCollider2D = background.AddComponent<TilemapCollider2D>();
        tilemapCollider2D.usedByComposite = true;

        var compositeCollider2d = background.AddComponent<CompositeCollider2D>();
        compositeCollider2d.geometryType = CompositeCollider2D.GeometryType.Polygons;
        compositeCollider2d.isTrigger = true;
        compositeCollider2d.generationType = CompositeCollider2D.GenerationType.Manual;

        background.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
    }

    private void SetSpawnPosition(DungeonGeneratorLevelGrid2D level)
    {
        // Ищем комнату входа на уровень
        var entranceRoomInstance = level
            .RoomInstances
            .FirstOrDefault(x => ((LevelOneRoom) x.Room).Type == LevelOneRoomType.Entrance);

        if (entranceRoomInstance == null)
        {
            throw new InvalidOperationException("Could not find Entrance room");
        }

        var roomTemplateInstance = entranceRoomInstance.RoomTemplateInstance;

        // Найдём позицию маркера Spawn
        var spawnPosition = roomTemplateInstance.transform.Find("SpawnPosition");

        // Сместим позицию игрока на позицию маркера появления
        var player = GameObject.FindWithTag("Player");
        player.transform.position = spawnPosition.position;
    }
}
