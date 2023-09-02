using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attacker : AttackingEntity {

	// Do not override this
	override protected AttackType executeAttack(StatManager statMan, EntityManager[] targetEnemies) {
		// 1. Choose attack type
		AttackType atkType = this.chooseAttack (statMan);

		// 2. Spawn Attack
		this.useAtkSpawner (statMan, targetEnemies, atkType);

		return atkType;
	}
	override protected Vector3 getMoveNormal(Heap<EntityManager> closestEnemies) {
			return !closestEnemies.isEmpty() ? this.getDisplacementToClosest(closestEnemies) : Vector3.zero;
	}

// OVERRIDABLE

	virtual protected AttackType chooseAttack (StatManager statMan) {
		// 1. No attacks available
		if (this.basicAtkSpawner.GetComponent<AttackSpawner> ().isOnCooldown (statMan.stats) &&
			this.chargeAtkSpawner.GetComponent<AttackSpawner> ().isOnCooldown (statMan.stats))
			return AttackType.None;

		float chance = Random.Range (0, 1);

		// 2. Chose basic attack
		if (chance < this.getBasicAtkChance())
			return AttackType.Basic;

		// 3. Chose charge attack OR basic attack was on cooldown
		return AttackType.Charge;
	}
	virtual protected float getBasicAtkChance() {
		return 0.75f;
	}

	protected void useAtkSpawner(StatManager statMan, EntityManager[] targetEnemies, AttackType atkType) {
		// 1. Short-circuit; Entity may not be able to	 attack if attacks are on cooldown
		if (atkType == AttackType.None) return;

		// 2. Spawn Attack
		switch (atkType) {
		case AttackType.Basic:
			this.basicAtkSpawner.GetComponent<AttackSpawner> ().spawnAttack (statMan.stats, targetEnemies);
			break;

		case AttackType.Charge:
		case AttackType.Angry:
			this.chargeAtkSpawner.GetComponent<AttackSpawner> ().spawnAttack (statMan.stats, targetEnemies);
			break;
		}
	}

	override protected void animateAttack(AttackType atkType) {
		// TODO: Do something
	}
}
