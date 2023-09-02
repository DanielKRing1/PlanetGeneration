using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class CatchableEntity : ObservantEntityComponent {

	private Dictionary<int, CatchAttempt> catcherDict;
	protected EntityManager captor = null;

	// Use this for initialization
	void Start () {
		this.catcherDict = new Dictionary<int, CatchAttempt> ();
	}


// OBSERVANT API

	override protected void executeAction(StatManager statMan, EntityManager[] targetEntities) {
		// Do nothing
	}

	private Vector3 getMoveNormal(Heap<EntityManager> closestEnemies) {
		return -1 * this.getDisplacementToClosest(closestEnemies);
	}

// CATCHING API

	protected CatchAttempt getCatcher(int catcherId) {
		CatchAttempt dataInDict;
		this.catcherDict.TryGetValue (catcherId, out dataInDict);

		return dataInDict;
	}
	private void addCatcher(CatchAttempt newCatchAttempt) {
		// 1. If not already tracking the Catcher
		if (!this.catcherDict.ContainsKey (newCatchAttempt.catcherId)) {
			// 2. Start tracking the Catcher, using now as time at which the Catcher started catching
			newCatchAttempt.startTime = Time.time;
			this.catcherDict.Add (newCatchAttempt.catcherId, newCatchAttempt);
		}
	}
		/**
	 * Check if some entity has successfully captured this entity,
	 * Not already captured + some condition met
	 *
	 * @return True if successfully captured, else False; This should only happen once, unless this CatchableEntity is subsequently freed
	 */
	public bool attemptCapture(CatchAttempt CatchAttempt) {
		if (!this.isCaptured ()) {
			this.addCatcher (CatchAttempt);

			return this.isCatchConditionMet (CatchAttempt);
		}

		return false;
	}

	public void rmCatcher(int catcherId) {
		if (this.catcherDict.ContainsKey (catcherId))
			this.catcherDict.Remove (catcherId);
	}

	public bool isCaptured() {
		return this.captor != null;
	}
	abstract protected bool isCatchConditionMet (CatchAttempt catchArg);

	public void capture(EntityManager captor) {
		this.captor = captor;
	}

	public void free() {
		this.captor = null;
	}

// GETTER/SETTER

	override public EntityType getEntityType() {
		return EntityType.Catchable;
	}
}
