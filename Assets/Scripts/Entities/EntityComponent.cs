using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class EntityComponent : MonoBehaviour {

// WORLD UTILS

	public Heap<EntityManager> getClosestEntities(float vision, Func<Collider, bool> filter = null) {
		// 1. Set default filter
		if (filter == null)
			filter = (Collider col) => true;

		// 2. Find nearby entities
		Collider[] nearbyEntities = Physics.OverlapSphere(this.transform.position, vision, MyLayer.LivingEntities.toLayerMask());

		// 3. Filter nearby entities
		List<Collider> filteredEntities = new List<Collider>();
		foreach (Collider entity in nearbyEntities) {
			if(filter(entity))
				filteredEntities.Add (entity);
		}
		nearbyEntities = filteredEntities.ToArray ();

		// 4. Create heap, prioritizing closer enemies
		Heap<EntityManager> closestEntities = new Heap<EntityManager>(nearbyEntities.Length,
			(e1, e2) => Vector3.Distance(this.transform.position, e2.transform.position) - Vector3.Distance(this.transform.position, e1.transform.position));

		foreach(Collider entity in nearbyEntities) {
			EntityManager enemyManager = entity.GetComponent<EntityManager> ();

			// 5. Add EntityManagers
			if(enemyManager != null)
				closestEntities.add (enemyManager);
		}

		return closestEntities;
	}

	protected float getDistanceToClosest(Heap<EntityManager> closestEnemies) {
		return !closestEnemies.isEmpty() ? Vector3.Distance (this.transform.position, closestEnemies.peek ().transform.position) : 0;
	}
	protected Vector3 getDisplacementToClosest(Heap<EntityManager> closestEnemies) {
		return !closestEnemies.isEmpty() ? closestEnemies.peek ().transform.position - this.transform.position : Vector3.zero;
	}

// ENTITYMANAGER UTILS
	protected EntityManager getEntityManager() {
		return this.GetComponent<EntityManager>();
	}

// ID

	abstract public EntityType getEntityType();

	protected float getCatchHash(EntityManager em) {
		return (this.GetHashCode() + em.GetHashCode() % 10) / 10;
	}
}
