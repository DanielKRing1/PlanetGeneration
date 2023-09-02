using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
This script controls Entities that must be followed for some amount of time to be caught
*/

public class PassiveCatchableEntity : CatchableEntity {

	private float catchDuration;

	override protected bool isCatchConditionMet(CatchAttempt CatchAttempt) {
		CatchAttempt dataInDict = this.getCatcher (CatchAttempt.catcherId);
		
		// TODO Add catch duration to EntityConstants
		return Time.time - dataInDict.startTime >= catchDuration;
	}
}
