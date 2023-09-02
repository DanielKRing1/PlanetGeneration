using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTracker {

	public enum XpType
	{
		Easy,
		Normal,
		Medium,
		Hard
	}

	public int level { get; private set; }

	private float xpLvlExp;
	public int xpCur { get; private set; }
	public int xpNeeded { get; private set; }

	public LevelTracker(XpType xpType, int level) {
		this.xpLvlExp = LevelTracker.getXpLvlExp(xpType);
		this.level = level;

		for (int i = 0; i < level; i++) {
			this.levelUp ();
		}
	}

	public bool gainXp(int xp) {
		this.xpCur+= xp;

		bool leveledUp = this.xpCur >= xpNeeded;
		if (leveledUp)
			this.levelUp ();

		return leveledUp;
	}

	private void levelUp() {
		// 1. Increase level
		this.level++;

		// 2. Update needed xp
		this.xpNeeded = this.calcNewXpNeeded();
	}

	private int calcNewXpNeeded() {
		// 1. Calculate xp needed for next level
		float lvlExp = this.level * this.xpLvlExp;
		int additionalXpNeeded = (int) (100 * Mathf.Pow (1.5f, lvlExp));
		int totalXpNeeded = this.xpNeeded + additionalXpNeeded;

		return totalXpNeeded;
	}

	static public float getXpLvlExp(XpType xpType) {
		switch (xpType) {
		case XpType.Easy:
			return 0.12f;

		case XpType.Medium:
			return 0.14f;

		case XpType.Hard:
			return 0.15f;

		case XpType.Normal:
		default:
			return 0.13f;

		}
	}
}
