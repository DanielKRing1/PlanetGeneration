using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSpawner : MonoBehaviour {

	private float cooldown;
	private float lastSpawnTime;
	private GameObject attackObject;

	// Use this for initialization
	void Start () {
		this.cooldown = attackObject.GetComponent<Attack> ().getCooldown ();
		this.lastSpawnTime = 0;
	}

	public bool spawnAttack(ImmutableStats stats, EntityManager[] targetEnemies) {
		bool canSpawn = !this.isOnCooldown (stats);
		if (canSpawn)
			this.spawn (stats);

		return canSpawn;
	}
	private void spawn(ImmutableStats stats) {
		// 1. Update cooldown-related info
		this.lastSpawnTime = Time.time;

		// 2. Spawn Attack GameObject
		var go = Instantiate(this.attackObject, this.transform);
		go.GetComponent<Attack> ().init (this.transform, stats);
	}

	public bool isOnCooldown(ImmutableStats stats) {
		float timeSinceLast = Time.time - this.lastSpawnTime;
		return timeSinceLast < this.scaleCooldown(stats);
	}
	public float scaleCooldown(ImmutableStats stats) {
		// 1.25 ^ atkSpeed - 1
		float atkSpeedBuff = (Mathf.Pow (1.25f, stats.atkSpeed) - 1);
		return this.cooldown / (1 + atkSpeedBuff);
	}
}
