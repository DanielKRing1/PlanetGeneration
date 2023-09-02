using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveData {

	public ImmutableStats stats;
	public List<MovementFactor> mFactors;

	private MoveData() {
		// must instantiate with values
	}

	public MoveData(ImmutableStats stats) : this(stats, new List<MovementFactor>()) {
	}

	public MoveData(ImmutableStats stats, List<MovementFactor> mFactors) {
		this.stats = stats;
		this.mFactors = mFactors;
	}
}
