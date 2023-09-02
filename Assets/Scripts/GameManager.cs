using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour {

// SPAWNING BOUNDS
	static public int ENEMY_QUOTA = 100;
	private bool shouldReplenishEnemies = true;

// IMPORTANT FOR SPAWNING ENEMIES
	// Enemy prefabs
	public GameObject[] enemies;
	// EntityName to GameObject map
	private Dictionary<StaticEntityConstants.EntityName, GameObject> enemyMap = new Dictionary<StaticEntityConstants.EntityName, GameObject>();

	// Spawn range, GameObject
	private SortedDictionary<float, StaticEntityConstants.EntityName> spawnRangeMap = new SortedDictionary<float, StaticEntityConstants.EntityName>();
	// List for binary search
	private List<float> spawnRangeKeys;

// TRACK ALL ENTITIES
	private EntityManager player;
	private Dictionary<int, EntityManager> entityManagers = new Dictionary<int, EntityManager>();
	public int entityManagerCount = 0;

// INIT

	// Use this for initialization
	void Awake () {
		this.mapEnemyNames();
		this.createSpawnRangeMap();

		this.trackPlayer();

		StartCoroutine(this.replenishEnemies());
	}
	
	private void trackPlayer() {
		this.player = GameObject.FindGameObjectWithTag("Player").GetComponent<EntityManager>();
	}

// SPAWN ENEMIES UTILS

	private IEnumerator replenishEnemies() {
		while(this.shouldReplenishEnemies) {
			int enemyCount = this.getEnemies().Count;

			this.entityManagerCount = enemyCount;

			int spawnCount = GameManager.ENEMY_QUOTA - enemyCount;
			// print("Spawn count: " + spawnCount);
			for(int i = 0; i < spawnCount; i++) {
				this.spawnRandEnemy();
			}

			yield return new WaitForSeconds(1);
		}
	}

	private void spawnRandEnemy() {
		// 1. Choose an Entity to spawn
		StaticEntityConstants.EntityName enemyNameToSpawn = this.chooseRandEnemyToSpawn();
		
		// 2. Get GameObject
		GameObject enemyGO = this.enemyMap[enemyNameToSpawn];

		// 3. Instantiate GameObject
		Vector3 spawnPoint = this.getRandSpawnPosition();
		Quaternion spawnRotation = this.getRandSpawnRotation();
		var enemy = Instantiate(enemyGO, spawnPoint, spawnRotation);
		
		// 4. Init
		int spawnLevel = this.getRandLevel(spawnPoint);
		enemy.GetComponent<StatManager>().init(spawnLevel);
	}

	private List<EntityManager> getEnemies() {
		// 1. Get Player's CatchingEntity
		CatchingEntity playerCatchingEntity = this.player.GetComponent<CatchingEntity>();

		// print(this.player);
		// print(playerCatchingEntity);

		// 2. Enemies are Entities that are not that Player and that have not been caught by the Player
		Func<EntityManager, bool> filteForEnemies = (EntityManager em) => em.GetComponent<StatManager>().getEntityName() != StaticEntityConstants.EntityName.Player && !playerCatchingEntity.capturedEntity(em);

		return this.filterEntityManagers(filteForEnemies);
	}

// SPAWN RANGE UTILS

	/**
	* Map EntityNames to public enemy GameObjects
	*/
	private void mapEnemyNames() {
		foreach(GameObject enemy in this.enemies) {
			this.enemyMap.Add(enemy.GetComponent<StatManager>().getEntityName(), enemy);
		}
	}

	/**
	* Read StaticEntityConstants, and map spawnRanges to each EntityNames
	*
	* Entities with 0 spawnRate are not added
	* spawnRates are summed to divide the resultant map into "spawn ranges",
	* 	where a value falling below value B and above value A will put you in B's range:
	* 	[ ... A ... x ... B ... ],
	* 	so B's entity should be spawned
	*/
	private void createSpawnRangeMap() {
		// 1. Short circuit if map already built
		if(this.spawnRangeMap.Count > 0) return;

		// 2. Get individual spawn rates
		Dictionary<StaticEntityConstants.EntityName, float> spawnRates = StaticEntityConstants.getSpawnRates();

		// 3. Sum spawn rates to create "spawn ranges" for each EntityName
		float sum = 0;
		foreach(KeyValuePair<StaticEntityConstants.EntityName, float> entry in spawnRates) {
			sum += entry.Value;

			this.spawnRangeMap.Add(sum, entry.Key);
		}

		// 4. Cache sorted spawn range keys
		this.spawnRangeKeys = new List<float>(this.spawnRangeMap.Keys);
	}

	private StaticEntityConstants.EntityName chooseRandEnemyToSpawn() {
		// 1. Generate random number in spawn range
		float maxRange = this.spawnRangeKeys.Last();
		float randNumber = UnityEngine.Random.Range(0, maxRange);

		// 2. Find index in spawnRangeKeys
		int index = this.spawnRangeKeys.BinarySearch(randNumber);

		// 3. Exact match found, else complement to get "index where it would be"
		index = index >= 0 ? index : ~index;

		// 4. Out of bounds
		index = index < this.spawnRangeKeys.Count() ? index : index - 1;

		// 5. Convert to EntityName
		float key = this.spawnRangeKeys[index];
		return this.spawnRangeMap[key];
	}

// RANDOM SPAWN UTILS

	private Vector3 getRandSpawnPosition() {
		Vector3 position = this.player.transform.position;

		float maxRange = Mathf.Sqrt(MovingEntityAi.MAX_DIST_SQR_FROM_PLAYER);
		position.x += UnityEngine.Random.Range(0, maxRange);
		position.z += UnityEngine.Random.Range(0, maxRange);
		// TODO Track y position of terrain at (x, z) coordinate and apply y position to this spawn position
		// position.y += UnityEngine.Random.Range(0, maxRange);

		return position;
	}

	private Quaternion getRandSpawnRotation() {
		float yDeg = UnityEngine.Random.Range(-360, 360) % 360;
		
		return Quaternion.Euler(0, yDeg, 0);
	}

	private int getRandLevel(Vector3 spawnPoint) {
		// TODO Create R-Tree to track enemies in different parts of terrain
		// TODO Use average level of enemies in this part of terrain at spawnPoint to choose level range
		// TODO Add a way to jump up in levels
		return UnityEngine.Random.Range(0, 5);
	}

// ENTITY MANAGER UTILS

	public void addEntity(int goId, EntityManager entityManager) {
		// 1. Track Player
		// if(entityManager.GetComponent<StatManager>().getEntityName() == StaticEntityConstants.EntityName.Player) {
		// 	this.player = entityManager;
		// }
		
		// 2. Track all
		this.entityManagers.Add (goId, entityManager);
	}

	public void rmEntity(int goId) {
		this.entityManagers.Remove (goId);
	}
	
	protected List<EntityManager> filterEntityManagers(Func<EntityManager, bool> filter) {
		List<EntityManager> ems = this.getEntityManagers();

		return ems.Where<EntityManager>(filter).ToList<EntityManager>();
	}

	protected List<EntityManager> getEntityManagers() {
		return new List<EntityManager>(this.entityManagers.Values);
	}

	protected EntityManager getEntityManager(int managerId) {
		EntityManager entityManager;
		return this.entityManagers.TryGetValue (managerId, out entityManager) ? entityManager : null;
	}

// ENTITY COMPONENT UTILS

	public List<EntityComponent> getEntityComponents(EntityType entityType) {
		// 1. Create list to return
		List<EntityComponent> entities = new List<EntityComponent> ();

		// 2. For each tracked EntityManager
		foreach (EntityManager manager in this.getEntityManagers()) {
			// 3. Get manager's Entity component of type EntityType
			EntityComponent entity = manager.getEntityComponent (entityType);

			// 4. Add if not null
			if(entity != null)
				entities.Add (entity);
		}

		return entities;
	}
	public EntityComponent getEntityComponent(int managerId, EntityType entityType) {
		// 1. Get EntityManager by id
		EntityManager entityManager = this.getEntityManager (managerId);

		// 2. Return manager's Entity component
		return entityManager.getEntityComponent (entityType);
	}
}
