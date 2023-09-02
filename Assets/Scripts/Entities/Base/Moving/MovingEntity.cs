using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingEntity : EntityComponent, Updatable<MoveData, Object> {

// MOVEMENT LOGIC

	public Object onUpdate(MoveData moveData) {
		// print ("MovingEntity onUpdate");

		this.executeMovement(moveData);

		return null;
	}

	private void executeMovement(MoveData moveData) {
		// 1. Get movement bias
		Vector3 moveBias = this.calcMoveBias(moveData.mFactors);

		// 2. Get the movement normal, given a movement bias
		Vector3 normal = this.calcMoveNormal(moveBias);

		// 3. Use a "roam" vector if the moevement normal is Vector3.zero
		normal = this.applyRoamNormal(normal);

		// print("Final move vector: " + normal);

		// 4. Move
		this.move (moveData.stats, normal);
	}

	private void move(ImmutableStats stats, Vector3 normal) {
		// print("Speed: " + stats.speed);
		// print(this.getEntityManager().getEntityConstants().stats.speed);

		// 1. Move in normal direction
		this.transform.position = new Vector3 (
			transform.position.x + (normal.x * stats.speed),
			transform.position.y + (normal.y * stats.speed),
			transform.position.z + (normal.z * stats.speed)
		);

		// 2. Do something while moving
		this.onMove (stats);
	}
	virtual protected void onMove(ImmutableStats stats) {
		// Do nothing for now
	}

// MOVEMENT NORMAL CALCULATION

	// MOVE BIAS
	private Vector3 calcMoveBias(List<MovementFactor> mFactors) {
		// 1. Init move normal
		Vector3 moveBias = Vector3.zero;
		
		// 2. Use each Updatable EntityComponent to determine how to move
		foreach (MovementFactor mFactor in mFactors) {
			// 3. Weight each movement factor
			moveBias += this.weightMvmntFactor(mFactor);
		}

		return moveBias;
	}

	/**
	 * Apply weight to each "movement factor" EntityComponent's input movement vector
	 *
	 * Can be overridden to define custom weights to apply to each movement factor entity type
	 */
	virtual protected Vector3 weightMvmntFactor(MovementFactor mvmntFactor) {
		switch (mvmntFactor.entityType) {
		case EntityType.Attacking:
			return mvmntFactor.normal * 1;

		case EntityType.Catchable:
			return mvmntFactor.normal * 3;

		case EntityType.Catching:
			return mvmntFactor.normal * 2;

		default:
			return mvmntFactor.normal;
		}
	}

	// MOVE NORMAL
	/**
	 * Given some movement direction bias, calculate the normal direction to move
	 * By default, returns the normalized input Vector3, but can be overridden to return some more specialized Vector3
	 */
	virtual protected Vector3 calcMoveNormal(Vector3 moveBias) {
		return moveBias.normalized;
	}

	// ROAM NORMAL
	/**
	* Returns moveNormal if not Vecotr3.zero,
	* else uses roamNormal
	*/
	private Vector3 applyRoamNormal(Vector3 moveNormal) {
		return moveNormal == Vector3.zero ? this.getRoamNormal () : moveNormal;
	}

	/**
	 * Get a normal direction to move in when no movement is needed (MovementFactors are all Vector3.zero)
	 *
	 * Override to define a custom roam normal
	 */
	virtual protected Vector3 getRoamNormal() {
		return this.transform.forward;
	}

// ID
	override public EntityType getEntityType() {
		return EntityType.Moving;
	}

// JUMP LOGIC
	protected bool isGrounded() {
		float distanceToGround = this.GetComponent<Collider> ().bounds.extents.y + 0.1f;
		return Physics.Raycast (transform.position, Vector3.down, distanceToGround);
	}
}
