using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class StatManager : MonoBehaviour {

	public LevelTracker levelTracker;
	public float luck { get; private set; }
	public Color color { get; private set; }

	private StatsDict statsDict;
	public ImmutableStats stats { get { return this.exposeImmutableStats (); } }

	private bool inited = false;
	public void Start() {
		if(!this.inited) throw new UnityException("Must call init on StatManager to set starting level");
	}

	// CALL THIS METHOD IMMEDIATELY AFTER INSTANTIATING AN ENTITY, ELSE WILL THROW AN ERROR
	public void init(int level = 0) {
		// 1. Mark as inited
		this.inited = true;

		// 2. Get static EntityConstants
		EntityConstants entityConstants = this.getEntityConstants();

		// print("hereeee");
		// print(entityConstants.stats.speed);

		// 3. Init stats
		this.statsDict = new StatsDict(this.getStartingStatsDict (entityConstants));

		// foreach(KeyValuePair<StatComponent.StatType, StatComponent> entry in this.statsDict.dict) {
		// 	print(entry.Key);
		// 	print(entry.Value.value);
		// }
		// print(this.statsDict.dict);

		// 4. Level up
		this.levelTracker = new LevelTracker(entityConstants.xpType, level);
		for (int i = 0; i < level; i++) {
			this.levelUp ();
		}
	}

// OVERRIDABLE
	abstract public StaticEntityConstants.EntityName getEntityName();

	public EntityConstants getEntityConstants() {
		return StaticEntityConstants.getEntityConstants(this.getEntityName());
	}


	// CAN OVERRIDE TO DEFINE ADDITIONAL STATCOMPONENTS FOR A CUSTOM IMMUTABLESTATS STRUCT
	virtual protected Dictionary<StatComponent.StatType, StatComponent> getStartingStatsDict (EntityConstants entityConstants) {
		// 1. Set luck
		this.luck = entityConstants.luck;

		// 2. Set color
		this.color = entityConstants.color;

		// 3. Set Dynamic stats
		Dictionary<StatComponent.StatType, StatComponent> startingStatsDict = new Dictionary<StatComponent.StatType, StatComponent>();

		startingStatsDict.Add(StatComponent.StatType.Health, new StatComponent(entityConstants.stats.health, entityConstants.statExps.health));
		startingStatsDict.Add(StatComponent.StatType.Defense, new StatComponent(entityConstants.stats.defense, entityConstants.statExps.defense));
		startingStatsDict.Add(StatComponent.StatType.Attack, new StatComponent(entityConstants.stats.attack, entityConstants.statExps.attack));
		startingStatsDict.Add(StatComponent.StatType.AtkSpeed, new StatComponent(entityConstants.stats.atkSpeed, entityConstants.statExps.atkSpeed));

		startingStatsDict.Add(StatComponent.StatType.Speed, new StatComponent(entityConstants.stats.speed, entityConstants.statExps.speed));
		startingStatsDict.Add(StatComponent.StatType.Vision, new StatComponent(entityConstants.stats.vision, entityConstants.statExps.vision));

		return startingStatsDict;
	}

	// CAN OVERRIDE TO DEFINE A WAY TO EXPOSE A CUSTOM IMMUTABLESTATS STRUCT
	virtual protected ImmutableStats exposeImmutableStats() {
		// 1. Get values from StatDict
		StatComponent health;
		this.statsDict.dict.TryGetValue (StatComponent.StatType.Health, out health);
		StatComponent defense;
		this.statsDict.dict.TryGetValue (StatComponent.StatType.Defense, out defense);
		StatComponent attack;
		this.statsDict.dict.TryGetValue (StatComponent.StatType.Attack, out attack);
		StatComponent atkSpeed;
		this.statsDict.dict.TryGetValue (StatComponent.StatType.AtkSpeed, out atkSpeed);

		StatComponent speed;
		this.statsDict.dict.TryGetValue (StatComponent.StatType.Speed, out speed);
		StatComponent vision;
		this.statsDict.dict.TryGetValue (StatComponent.StatType.Vision, out vision);

		// 2. Create ImmutableStats
		return new ImmutableStats(
			this.luck,
			health.value,
			defense.value,
			attack.value,
			atkSpeed.value,
			speed.value,
			vision.value
		);
	}

// LEVEL UP API

	public bool gainXp(int gainedXp) {
		bool leveledUp = this.levelTracker.gainXp (gainedXp);
		if (leveledUp)
			this.levelUp ();

		return leveledUp;
	}

	private void levelUp() {
		foreach (KeyValuePair<StatComponent.StatType, StatComponent> entry in this.statsDict.dict) {
			entry.Value.levelUp (this.luck);
		}
	}

}
