using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatComponent {

// PUBLIC STRUCTURES

	public enum StatType {
		Luck,

		Health,
		Defense,
		Attack,

		Speed,
		Vision,

		// Cooldown multiplier
		AtkSpeed
	}

// CLASS IMPLEMENTATION

	private float baseValue;
	private float levelUpExp;

	public float value {
		get {
			float buff = this.buffQ.isEmpty() ? 1 : this.buffQ.peek();
			float debuff = this.debuffQ.isEmpty() ? 1 : this.debuffQ.peek();

			return this.baseValue * buff * debuff;
		}
	}

	public TimeseriesMilestoneList<float, float> buffQ = new TimeseriesMilestoneList<float, float>();
	public TimeseriesMilestoneList<float, float> debuffQ = new TimeseriesMilestoneList<float, float>();

	public StatComponent(float baseValue, float levelUpExp) {
		this.baseValue = baseValue;
		this.levelUpExp = levelUpExp;
	}

	public void buff(float duration, float percent) {
		float key = Time.time + duration;

		this.buffQ.Add (key, percent);
	}

	public void debuff(float duration, float percent) {
		float key = Time.time + duration;

		this.debuffQ.Add (key, percent);
	}

	public void levelUp(float luck) {
		// 1. Calc max stat increase
		int maxInc = (int) (Mathf.Pow(this.baseValue, 1 + this.levelUpExp) - this.baseValue);
		// 2. Calc luck coefficient, between half of luck and 1
		float luckCoeff = Random.Range (0 + luck / 2, 1);

		// 3. Calc new stat, += luck * (val^(1+exp) - val)
		this.baseValue += (luckCoeff * maxInc);
	}

}
