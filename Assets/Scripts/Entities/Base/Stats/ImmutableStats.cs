using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ImmutableStats {
	public readonly float luck;

	public readonly float health;
	public readonly float defense;
	public readonly float attack;
	public readonly float atkSpeed;

	public readonly float speed;
	public readonly float vision;

	public ImmutableStats(float luck, float health, float defense, float attack, float atkSpeed, float speed, float vision) {
		this.luck = luck;

		this.health = health;
		this.defense = defense;
		this.attack = attack;
		this.atkSpeed = atkSpeed;

		this.speed = speed;
		this.vision = vision;
	}


}
