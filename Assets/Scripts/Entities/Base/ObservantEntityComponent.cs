using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class ObservantEntityComponent : EntityComponent, Updatable<StatManager, OnUpdateResult<Object>> {

	public OnUpdateResult<Object> onUpdate(StatManager statMan) {
		// 1. Get closest enemies, filter non-living entities
		Heap<EntityManager> closestEnemies = this.getClosestEntities(statMan);

		// 2. Filter closest enemies
		closestEnemies.filter(this.filterClosestEnemies);

        // 3. Select target enemies
        EntityManager[] targetEneemies = this.selectTargetEntities(closestEnemies);

		// 4. Perform Spawn attack, Attack, etc
		this.executeAction (statMan, targetEneemies);

		// 5. Return normal to closest enemy
		Vector3 normal = this.getMoveNormal(closestEnemies);

		// 6. Affect movement bias
		OnUpdateResult<Object> onUpdateResult = new OnUpdateResult<Object>();
		onUpdateResult.mvmntFactor.fill(normal, null, this.getEntityType());

		return onUpdateResult;
	}
	protected Heap<EntityManager> getClosestEntities(StatManager statMan) {
		return this.getClosestEntities(statMan.stats.vision, (Collider col) => col.GetComponent<LivingEntity>() != null);
	}

// OVERRIDABLE

	virtual protected bool filterClosestEnemies(EntityManager em) {
		return true;
	}
    virtual protected EntityManager[] selectTargetEntities (Heap<EntityManager> closestEnemies) {
		// By default, select all closest enemy
		return closestEnemies.toArray();
	}
	abstract protected void executeAction(StatManager statMan, EntityManager[] targetEneemies);

    virtual protected Vector3 getMoveNormal(Heap<EntityManager> closestEnemies) {
		// By default, do not affect movement
		return Vector3.zero;
	}
}
