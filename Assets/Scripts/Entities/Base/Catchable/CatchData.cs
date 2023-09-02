using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CatchAttempt {

	public int catcherId;
	public Color color;
	public float startTime;

	public Object data;

	public CatchAttempt(int catcherId, Color color, float startTime = 0, Object data = null) {
		this.catcherId = catcherId;
		this.color = color;
		this.startTime = startTime;

		this.data = data;
	}
}
