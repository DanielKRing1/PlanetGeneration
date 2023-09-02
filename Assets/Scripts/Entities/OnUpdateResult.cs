using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnUpdateResult<T> {

	public MovementFactor mvmntFactor = new MovementFactor();
	public T status = default(T);

	public void copy(OnUpdateResult<T> other) {
		if (other.hasMovementFactor ())
			this.setMvmntFactor (other.mvmntFactor);

		if (other.hasStatus ())
			this.setStatus (other.status);
	}

	public OnUpdateResult<T> setMvmntFactor(MovementFactor mvmntFactor) {
		this.mvmntFactor = mvmntFactor;

		return this;
	}

	public OnUpdateResult<T> setStatus(T status) {
		this.status = status;

		return this;
	}

	public bool hasMovementFactor() {
		return this.mvmntFactor.isNull();
	}

	public bool hasStatus() {
		return EqualityComparer<T>.Default.Equals (this.status, default(T));
	}

// OnUpdateResults COLLECTORS

	/**
	 * Collect OnUpdateResults into a list of MovementFactors
	 */
	public static List<MovementFactor> collectMovementFactors(Dictionary<EntityType, OnUpdateResult<Object>> onUpdateResults) {
		// 1. Init list of MovementFactors
		List<MovementFactor> mvmntFactors = new List<MovementFactor>();

		// 2. Use each Updatable EntityComponent to determine how to move
		foreach (KeyValuePair<EntityType, OnUpdateResult<Object>> entry in onUpdateResults) {

			// 3. Add MovementFactor
			if(entry.Value.hasMovementFactor())
				mvmntFactors.Add(entry.Value.mvmntFactor);
		}

		return mvmntFactors;
	}
}
