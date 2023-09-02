using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class AttackingEntity : ObservantEntityComponent {

	public GameObject basicAtkSpawner;
	public GameObject chargeAtkSpawner;

	// Use this for initialization
	void Start () {
		this.validateAttacks ();
	}

// ATTACK VALIDATION

	private void validateAttacks() {
		this.validateAttackGO (basicAtkSpawner);
		this.validateAttackGO (chargeAtkSpawner);
	}

	private void validateAttackGO(GameObject atkSpawner) {
		AttackSpawner atkSpawnerScript = atkSpawner.GetComponent<AttackSpawner> ();
		if (atkSpawner == null)
			throw new UnityException ("AttackSpawner GameObject does not have an AttackSpawner script attached: " + atkSpawner);
	}

// OBSERVANT API

	sealed override protected bool filterClosestEnemies(EntityManager em) {
		return em.GetComponent<CatchableEntity>() == null || this.getCatchHash(em) >= this.getEntityManager().getEntityConstants().catchRatio;
	}

	// By default, select only the closest enemy
	sealed override protected EntityManager[] selectTargetEntities (Heap<EntityManager> closestEnemies) {
		return !closestEnemies.isEmpty() ? new EntityManager[] { closestEnemies.peek() } : new EntityManager[0];
	}
	sealed override protected void executeAction(StatManager statMan, EntityManager[] targetEnemies) {
		// 1. Execute action (Spawn attack, Attack)
		AttackType atkType = this.executeAttack(statMan, targetEnemies);

		// 2. Animate action
		this.animateAttack (atkType);

		// 3. Do something after attack
		this.afterAttack (statMan, targetEnemies, atkType);
	}

// OVERRIDABLE

	abstract protected AttackType executeAttack(StatManager statMan, EntityManager[] targetEnemies);

	virtual protected void afterAttack (StatManager stats, EntityManager[] targetEnemies, AttackType atkType) {
		// Do nothing
	}

	virtual protected void animateAttack (AttackType atkType) {
		// Do nothing
	}

// ID

	override public EntityType getEntityType() {
		return EntityType.Attacking;
	}
}