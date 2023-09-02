using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnedAttackEntity : EntityComponent {

// ID
	override public EntityType getEntityType () {
		return EntityType.SpawnedAttack;
	}
}
