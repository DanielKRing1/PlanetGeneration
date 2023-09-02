using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatsManager : StatManager
{

    // DO NOT DYNAMICALLY INSTANTIATE PLAYER, SO MUST CALL INIT EXPLICITLY
    public void Awake() {
        this.init(0);
    }

	override public StaticEntityConstants.EntityName getEntityName() {
        return StaticEntityConstants.EntityName.GENERIC_ENEMY;
    }
}
