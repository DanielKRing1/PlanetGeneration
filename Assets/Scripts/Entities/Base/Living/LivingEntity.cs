using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * This class maanges an entity's health (taking damage/healing), element type
 */
abstract public class LivingEntity : EntityComponent{

	public int curHealth { get; private set; }
	protected ElementType elementType;

	public enum AlterHealth {
		Damage,
		Heal
	}
	protected static class AlterHealthUtils {
		public static int setSign(int value, AlterHealth alteration) {
			// 1. Get absolute value
			value = Mathf.Abs (value);

			// 2. Set sign, negative for damage and positive for heal
			return alteration == AlterHealth.Damage ? -1 * value : value;
		}
	}

// TAKING DAMAGE LOGIC

	/**
	 * Returns true if still alive, else false
	 */
	public LifeStatus takeDamage(ImmutableStats stats, float damage, ElementType elementType) {
		// 1. Scale damage and calculate remaining health
		int actualDamage = this.scaleDamage (damage, elementType);
		this.alterCurHealth (stats, actualDamage, AlterHealth.Damage);

		// 2. Animate taking damage
		this.animateTakingDamage();

		// 3. Potentially do something after taking damage
		return this.respondToDamage ();
	}

	/**
	 * Helper method before calling actual scaling implementation
	 */
	private int scaleDamage(float damage, ElementType atkType) {
		// 1. Check effectiveness of attack
		ElementEffectiveness elementEffectiveness= ElementTypeUtils.getEffectiveness (atkType, this.elementType);

		// 2. Perform scaling
		return (int) this.onScaleDamage (damage, elementEffectiveness);
	}
	/**
	 * Actual scaling implementation,
	 * Scale damage based on element type effectiveness
	 */
	virtual protected int onScaleDamage(float damage, ElementEffectiveness elementEffectiveness) {
		switch (elementEffectiveness) {

		case ElementEffectiveness.Effective:
			return (int) (2 * damage);

		case ElementEffectiveness.NonEffective:
			return (int) (0.5f * damage);

		default:
			return (int) damage;

		}
	}

	/**
	 * Check if dead, and
	 * Potentially do something after taking damage
	 * 
	 * Returns true if still alive, else false
	 */
	private LifeStatus respondToDamage () {
		bool isAlive = this.curHealth > 0;

		if (isAlive)
			this.onRespondToDamage  ();
		else
			this.die ();

		return isAlive ? LifeStatus.Alive : LifeStatus.Dead;
	}
	virtual protected void onRespondToDamage () {
		// Do nothing here... Maybe heal or counter
	}
	private void die() {
		// Maybe shouldn't Destroy GameObject here,
		// Entity components should only track the state of their component, not interact with the actual GameObject, other than through animations
		// 1. Destroy this GameObject
		// Destroy(this);

		// 3. Animate death
		this.animateDeath();

		// 4. Potentially do something else before dying
		this.onDie ();
	}
	virtual protected void onDie() {
		// Do nothing here... Maybe explode
	}

// HEALING LOGIC
	public void heal(ImmutableStats stats, int amountToHeal) {
		this.alterCurHealth (stats, amountToHeal, AlterHealth.Heal);
	}

// CURRENT HEALTH
	/**
	 * Provide a positive value to increase the current health and
	 * A negative value to decrease the current health
	 */
	private void alterCurHealth(ImmutableStats stats, int amount, AlterHealth alteration) {
		// 1. Add correct sign to alteration amount
		amount = AlterHealthUtils.setSign (amount, alteration);

		// 2. Alter current health, clamping to max health
		this.curHealth = Mathf.Clamp(this.curHealth + amount, 0, (int) stats.health);
	}
		
// ANIMATIONS
	abstract protected void animateTakingDamage();

	abstract protected void animateDeath ();

// ID
	override public EntityType getEntityType() {
		return EntityType.Living;
	}
	public ElementType getElementType() {
		return this.elementType;
	}
}
