using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsDict {

	public Dictionary<StatComponent.StatType, StatComponent> dict;

	public StatsDict(Dictionary<StatComponent.StatType, StatComponent> startingStats) {
		this.dict = startingStats;
	}
}
