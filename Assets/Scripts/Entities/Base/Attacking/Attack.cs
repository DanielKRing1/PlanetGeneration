using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Attack : AttackingEntity {

	private bool inited = false;

	protected bool rotateWithParent = false;
	protected bool moveWithParent = false;

	private Transform parentTransform;
	private ImmutableStats attackerStats;

	public void init(Transform parentTransform, ImmutableStats attackerStats) {
		this.inited = true;

		this.parentTransform = parentTransform;
		this.attackerStats = attackerStats;
	}
	private void validateInit() {
		if (!this.inited)
			throw new UnityException ("Must call Attack.init() before the Attack object can function");
	}

	override protected AttackType executeAttack(StatManager statMan, EntityManager[] targetEnemies) {
		this.validateInit ();

		// 1. Apply necessary parent transforms
		if(rotateWithParent) this.transform.rotation = this.parentTransform.rotation;
		if(moveWithParent) this.transform.position = this.parentTransform.position;

		// 2. Deal damage to selected enemies
		float damage = statMan.stats.attack * this.attackerStats.attack;
		foreach (EntityManager enemy in targetEnemies) {
			enemy.startTakeDamage (damage, this.getElementType());
		}

		return AttackType.Basic;
	}

	override protected void animateAttack(AttackType atkType) {
		// TODO: Do something
	}

	abstract protected ElementType getElementType();
	abstract public float getCooldown ();
}
