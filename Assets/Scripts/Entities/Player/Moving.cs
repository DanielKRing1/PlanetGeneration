using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player {
	
	public class Moving : MovingEntity {

// OVERRIDES

		/**
		 * Calculate the normal direction to move, given some movement bias
		 * 
		 * Player movement ignores the given bias
		 */
		override protected Vector3 calcMoveNormal(Vector3 moveBias) {
			int x, z;
			x = z = 0;

			// 1. Get x-axis direction
			if (Input.GetKey ("d"))
				x = 1;
			else if (Input.GetKey ("a"))
				x = -1;

			// 2. Get z-axis direction
			if (Input.GetKey ("w"))
				z = 1;
			else if (Input.GetKey ("s"))
				z = -1;

			// print ("Normalized getMovementDirections: " + new Vector3 (x, 0, z).normalized);

			return new Vector3 (x, 0, z).normalized;
		}
		/**
		 * Get a normal direction to move in when no movement is needed (MovementFactors are all Vector3.zero)
		 * 
		 * Player does not move when not using the controls
		 */
		override protected Vector3 getRoamNormal() {
			return Vector3.zero;
		}
		
		/**
		 * Add jump logic
		 */
		override protected void onMove(ImmutableStats stats) {
			if (Input.GetKey ("space") && this.isGrounded ())
				this.GetComponent<Rigidbody> ().AddForce (0, stats.speed / 5, 0, ForceMode.Impulse);
		}
	}
}
