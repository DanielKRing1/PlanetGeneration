using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
This script controls Entities that must be defeated (attacked) to be caught
*/

public class AtkCatchableEntity : CatchableEntity  {

	override protected bool isCatchConditionMet(CatchAttempt CatchAttempt) {
		// TODO Check if health == 0
		return ((LivingEntity) this.getEntityManager().getEntityComponent(EntityType.Living)).curHealth <= 0;
	}
}
