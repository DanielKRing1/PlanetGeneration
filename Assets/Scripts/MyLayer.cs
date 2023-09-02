using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyLayer {

	static public MyLayer LivingEntities = new MyLayer("LivingEntities");

	private string name;

	private MyLayer(string name) {
		this.name = name;
	}

	public int toLayerMask() {
		return LayerMask.NameToLayer (this.name);
	}
}
