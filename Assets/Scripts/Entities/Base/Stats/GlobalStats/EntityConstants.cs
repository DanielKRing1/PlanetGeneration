using UnityEngine;

public class EntityConstants {
    private bool locked = false;

    public EntityConstants() {}


// AUTO PROPERTIES

    // STATS
    private ImmutableStats _stats;
    public ImmutableStats stats {
        get {
            if(this.locked) return this._stats;
            else throw this.createGetException();
        }
        private set {
            if(!this.locked) this._stats = value;
            else throw this.createSetException();
        }
    }
    public EntityConstants Stats(ImmutableStats stats) {
        this.stats = stats;

        return this;
    }

    // STAT EXPS
    // Exponents for leveling up stats
    private ImmutableStats _statExps;
    public ImmutableStats statExps {
        get {
            if(this.locked) return this._statExps;
            else throw this.createGetException();
        }
        private set {
            if(!this.locked) this._statExps = value;
            else throw this.createSetException();
        }
    }
    public EntityConstants StatExps(ImmutableStats statExps) {
        this.statExps = statExps;

        return this;
    }

    // XP LEVEL TYPE
    // How difficult it is for an entity to level up
    private LevelTracker.XpType _xpType;
    public LevelTracker.XpType xpType {
        get {
            if(this.locked) return this._xpType;
            else throw this.createGetException();
        }
        private set {
            if(!this.locked) this._xpType = value;
            else throw this.createSetException();
        }
    }
    public EntityConstants XpType(LevelTracker.XpType xpType) {
        this.xpType = xpType;

        return this;
    }

    // CATCH RATIO
    // A number between 0 and 1
    private float _catchRatio;
    public float catchRatio {
        get {
            if(this.locked) return this._catchRatio;
            else throw this.createGetException();
        }
        private set {
            if(!this.locked) this._catchRatio = value;
            else throw this.createSetException();
        }
    }
    public EntityConstants CatchRatio(float catchRatio) {
        this.catchRatio = catchRatio;

        return this;
    }
    
    // LUCK
    // A number between 0 and 1
    private float _luck;
    public float luck {
        get {
            if(this.locked) return this._luck;
            else throw this.createGetException();
        }
        private set {
            if(!this.locked) this._luck = value;
            else throw this.createSetException();
        }
    }
    public EntityConstants Luck(float luck) {
        this.luck = luck;

        return this;
    }

    // COLOR
    private Color _color;
    public Color color {
        get {
            if(this.locked) return this._color;
            else throw this.createGetException();
        }
        private set {
            if(!this.locked) this._color = value;
            else throw this.createSetException();
        }
    }
    public EntityConstants Color(Color color) {
        this.color = color;

        return this;
    }

    // SPAWN RATE
    // If not set, then this entity type has no chance to spawn
    private float _spawnRate = 0;
    public float spawnRate {
        get {
            if(this.locked) return this._spawnRate;
            else throw this.createGetException();
        }
        private set {
            if(!this.locked) this._spawnRate = value;
            else throw this.createSetException();
        }
    }
    public EntityConstants SpawnRate(float spawnRate) {
        this.spawnRate = spawnRate;

        return this;
    }

    public EntityConstants done() {
        this.locked = true;

        return this;
    }

    private UnityException createGetException() {
        return new UnityException ("Must call 'done' before accessing member variables");
    }

    private UnityException createSetException() {
        return new UnityException ("Cannot set member variables after calling 'done'");
    }
}