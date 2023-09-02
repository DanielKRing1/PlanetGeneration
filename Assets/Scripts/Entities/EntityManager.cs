using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EntityManager : MonoBehaviour {

	// For keeping track of this entity's existence
	protected GameManager gameManager;

	// For tracking this entitiy's Entity components
	private Dictionary<EntityType, EntityComponent> entityComponents;

	// Use this for initialization
	void Awake () {
		this.registerToGameManager ();
		this.registerEntityComponents ();
	}

// UPDATES

	// Update is called once per frame
	void Update () {

		Dictionary<EntityType, OnUpdateResult<Object>> onUpdateResults = this.updateComponents ();
		this.updateMovementComponent (onUpdateResults);

	}

	private Dictionary<EntityType, OnUpdateResult<Object>> updateComponents() {
		// 1. Init results dict
		Dictionary<EntityType, OnUpdateResult<Object>> onUpdateResults = new Dictionary<EntityType, OnUpdateResult<Object>> ();

		// 2. Get Updatable
		Dictionary<EntityType, IndependentlyUpdatable> independentlyUpdatableComponents = this.entityComponents.Where (kvp => this.isIndependentlyUpdatableComponent (kvp.Value)).ToDictionary(kvp => kvp.Key, kvp => (IndependentlyUpdatable) kvp.Value);

		foreach (KeyValuePair<EntityType, IndependentlyUpdatable> updatableEntity in independentlyUpdatableComponents) {
			// 1. Execute onUpdate
			OnUpdateResult<Object> additionalOnUpdateResult = updatableEntity.Value.onUpdate (this.GetComponent<StatManager> ());

			// 2. Get IndependentlyUpdatable component's entity type, and record the new data
			EntityType entityType = ((EntityComponent) updatableEntity.Value).getEntityType ();
			onUpdateResults.Add (entityType, additionalOnUpdateResult);
		}

		return onUpdateResults;
	}

	private void updateMovementComponent(Dictionary<EntityType, OnUpdateResult<Object>> onUpdateResults) {
		// 1. Get MovementFactors from OnUpdateResults
		List<MovementFactor> mvmntFactors = OnUpdateResult<Object>.collectMovementFactors(onUpdateResults);

		// 2. Get movement EntityComponent
		MovingEntity movingEntity = ((MovingEntity)this.getEntityComponent (EntityType.Moving));

		if (movingEntity != null) {
			// 3. If this is a moving entity, move
			// print(this.getEntityConstants().stats.speed);
			// print(this.GetComponent<StatManager>().stats.speed);
			MoveData moveData = new MoveData(this.GetComponent<StatManager> ().stats, mvmntFactors);
			movingEntity.onUpdate (moveData);
		}
	}

// INIT UTILS

	// GAME MANAGER UTILS

	private void findGameManager() {
		this.gameManager = FindObjectOfType<GameManager> ();
	}

	protected void registerToGameManager() {
		if (this.gameManager == null)
			this.findGameManager ();

		// print(this.gameManager);
		// print(this);
		this.gameManager.addEntity (this.GetInstanceID (), this);
	}

	protected void rmFromGameManager() {
		if (this.gameManager == null)
			this.findGameManager ();

		this.gameManager.rmEntity (this.GetInstanceID ());
	}

	// STATIC STATS UTILS
	
	public EntityConstants getEntityConstants() {
		print(this.GetComponent<StatManager>());
		return this.GetComponent<StatManager>().getEntityConstants();
	}

	// ENTITY COMPONENTS UTILS

	private void registerEntityComponents() {
		// 1. Initialize entity components dict
		this.entityComponents = new Dictionary<EntityType, EntityComponent> ();

		// 2. Get all Entity components
		EntityComponent[] components = this.GetComponents<EntityComponent> ();

		// 3. Add each Entity component to the map, by EntityType
		foreach (EntityComponent component in components)
			this.entityComponents.Add (component.getEntityType (), component);
	}

// DICTIONARY GETTERS/SETTERS

	public EntityComponent getEntityComponent(EntityType entityType) {
		EntityComponent entity;
		this.entityComponents.TryGetValue (entityType, out entity);

		if(entity == null) throw new UnityException("Exception thrown from EntityManager.getEntityComponent()" +
														"\nThis Entity does not have an EntityComponent of type '" + entityType.ToString() + "' attached" +
														"\nEntity name: " + this.GetComponent<StatManager>().getEntityName() +
														"\nInstance id: " + this.GetInstanceID());

		return entity;
	}

// OTHER DICTIONARY UTILS

	private bool isUpdatableComponent(EntityComponent component) {
		return component is Updatable<Object, Object>;
	}

	private bool isIndependentlyUpdatableComponent(EntityComponent component) {
		return component is IndependentlyUpdatable;
	}

	private bool isDependentlyUpdatableComponent(EntityComponent component) {
		return component is DependentlyUpdatable;
	}

// CLEAN UP

	private void freeCapturedEntities() {
		CatchingEntity component = (CatchingEntity) this.getEntityComponent (EntityType.Catching);

		if (component != null)
			component.free ();
	}
	
	private void freeFromCaptors() {
		CatchableEntity component = (CatchableEntity) this.getEntityComponent (EntityType.Catchable);

		if (component != null)
			component.free ();
	}



	protected void destroy() {
		// 1. Remove from GameManager
		this.rmFromGameManager ();

		// 2. Free captured entities
		this.freeCapturedEntities();

		// 3. Remove from captor entity
		if (this.getEntityComponent (EntityType.Catchable)) {

		}

		// 4. Destroy GameObject
		Destroy (this);
	}

// PUBLIC API ----------------------------------------------------------------------------------------------------------

// ATTACK API

	public LifeStatus startTakeDamage(float damage, ElementType elementType) {
		// 1. Get LivingEntity component
		LivingEntity entity = (LivingEntity) this.getEntityComponent (EntityType.Living);

		// 2. Apply damage to LivingEntity component
		LifeStatus lifeStatus = entity.takeDamage (this.GetComponent<StatManager>().stats, damage, elementType);

		// 3. If dead, Destroy
		if (lifeStatus == LifeStatus.Dead)
			this.destroy ();

		return lifeStatus;
	}

// CATCH API

	/**
	 * @return True if successfully captured, else False; This should only happen once, unless this CatchableEntity is subsequently freed
	*/
	public bool startAddCatcher(CatchAttempt catchAttempt) {
		// 1. Get CatchableEntity component
		CatchableEntity entity = (CatchableEntity) this.getEntityComponent (EntityType.Catchable);

		return entity.attemptCapture (catchAttempt);
	}

	public void startRmCatcher(int catcherId) {
		// 1. Get CatchableEntity component
		CatchableEntity entity = (CatchableEntity) this.getEntityComponent (EntityType.Catchable);

		entity.rmCatcher (catcherId);
	}
}
