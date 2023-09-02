using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MovementFactor {

	public Vector3 normal;
	public Object data;
	public EntityType entityType;

	public MovementFactor(Vector3 normal, Object data, EntityType entityType) {
		this.normal = normal;
		this.data = data;
		this.entityType = entityType;	}

	public void fill(Vector3 normal, Object data, EntityType entityType) {
		this.normal = normal;
		this.data = data;
		this.entityType = entityType;
	}

	public bool isNull() {
		return this.normal == null && data == null && entityType == null;
	}
}
