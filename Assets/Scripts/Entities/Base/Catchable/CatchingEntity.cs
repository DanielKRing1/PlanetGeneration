using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatchingEntity : ObservantEntityComponent {

	// Dict for tracking the time at which this entity began trying to catch any PassiveCatchableEntities
	private Dictionary<int, EntityManager> caughtEntities = new Dictionary<int, EntityManager> ();
	private HashSet<EntityManager> prevCatching = new HashSet<EntityManager>();

	private float vision;

// OBSERVANT API

	sealed override protected bool filterClosestEnemies(EntityManager em) {
		return em.GetComponent<CatchableEntity>() != null && this.getCatchHash(em) < this.getEntityManager().getEntityConstants().catchRatio;
	}

	override protected void executeAction(StatManager statMan, EntityManager[] targetEntities) {
		int myId = this.GetInstanceID();
		HashSet<EntityManager> nowCatching = new HashSet<EntityManager> ();

		// 1. Notify the CatchableEntity that it is being caught by this
		CatchAttempt catchAttempt = new CatchAttempt(myId, this.getEntityManager().getEntityConstants().color, Time.time);
		foreach (EntityManager entityToCatch in targetEntities) {
			int id = entityToCatch.GetInstanceID ();

			entityToCatch.startAddCatcher(catchAttempt);
			nowCatching.Add (entityToCatch);
		}

		// 2. Notify any CatchableEntities that are no longer being caught
		foreach (EntityManager entityToCatch in this.prevCatching) {
			if(!nowCatching.Contains(entityToCatch))
				entityToCatch.startRmCatcher(myId);
		}

		// 3. Update the prev set of entities being caught
		this.prevCatching = nowCatching;
	}

    override protected Vector3 getMoveNormal(Heap<EntityManager> closestEnemies) {
		return this.getDisplacementToClosest(closestEnemies);
	}

	// CATCHING API

	private void capture(EntityManager captive) {

	}

	public void free() {

	}

// CAPTURED DICT UTILS

	private void captureEntity(EntityManager em) {
		this.caughtEntities.Add(em.GetInstanceID(), em);
	}

	public bool capturedEntity(EntityManager em) {
		return this.caughtEntities.ContainsKey(em.GetInstanceID());
	}


// ENTITY TYPE

	override public EntityType getEntityType() {
		return EntityType.Catchable;
	}
}
