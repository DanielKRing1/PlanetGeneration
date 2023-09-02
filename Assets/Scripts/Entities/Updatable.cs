using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Updatable<T, R> {

	R onUpdate (T dataWrapper);
}

public interface IndependentlyUpdatable : Updatable<StatManager, OnUpdateResult<Object>>{}

public interface DependentlyUpdatable : Updatable<MoveData, Object>{}
