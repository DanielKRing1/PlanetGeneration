using System.Collections.Generic;
using UnityEngine;
using System.Linq;

static public class StaticEntityConstants {
    public enum EntityName {
        Player,
        GENERIC_ENEMY
    };


// STATIC VARIABLES
    
    private static readonly Dictionary<EntityName, EntityConstants> entityConstants = new Dictionary<EntityName, EntityConstants>();

// DEFINE ENTITY CONSTANTS

    private static readonly EntityConstants PLAYER_CONSTANTS = new EntityConstants()
                                                                    .Stats(new ImmutableStats(0.5f, 25f, 2f, 4f, 1f, 0.1f, 5f))
                                                                    .StatExps(new ImmutableStats(0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f))
                                                                    .XpType(LevelTracker.XpType.Normal)
                                                                    .Luck(0.5f)
                                                                    .CatchRatio(0.5f)
                                                                    .Color(Color.blue)
                                                                    .done();

    private static readonly EntityConstants GENERIC_ENEMY_CONSTANTS = new EntityConstants()
                                                                    .Stats(new ImmutableStats(0.5f, 10f, 2f, 2f, 1f, 0.1f, 5f))
                                                                    .StatExps(new ImmutableStats(0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f))
                                                                    .XpType(LevelTracker.XpType.Normal)
                                                                    .Luck(0.5f)
                                                                    .CatchRatio(0.5f)
                                                                    .Color(Color.blue)
                                                                    .SpawnRate(0.5f)
                                                                    .done();

    static StaticEntityConstants() {
        StaticEntityConstants.entityConstants.Add(StaticEntityConstants.EntityName.Player, PLAYER_CONSTANTS);
        StaticEntityConstants.entityConstants.Add(StaticEntityConstants.EntityName.GENERIC_ENEMY, GENERIC_ENEMY_CONSTANTS);
    }
    
// PUBLIC GETTER

    public static EntityConstants getEntityConstants(StaticEntityConstants.EntityName entityName) {
        EntityConstants entityConstants;

        StaticEntityConstants.entityConstants.TryGetValue(entityName, out entityConstants);

        // EntityConstants not defined for the given entityName
        if(entityConstants == null) throw new UnityException("EntityConstants were not defined for the given entityName: " + entityName);

        return entityConstants;
    }

    /**
    * Filters out EntityNames that have a spawnRate of <= 0
    *
    * Returns a Dict of <EntityName, spawnRate>
    */
    public static Dictionary<EntityName, float> getSpawnRates() {
        return StaticEntityConstants.entityConstants
                                        .Where(e => e.Value.spawnRate > 0)
                                        .ToDictionary(e => e.Key, e => e.Value.spawnRate);
    }
    
}