using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ElementType {

	Water,
	Fire,
	Grass

}

public enum ElementEffectiveness {
	Effective,
	Neutral,
	NonEffective
}

public static class ElementTypeUtils {
	
	public static string getName(ElementType type) {
		return System.Enum.GetName(typeof(ElementType), type);
	}

	/**
	 * Returns
	 * 1 if effective
	 * 0 if neutral
	 * -1 if noneffective
	 */
	public static ElementEffectiveness getEffectiveness(ElementType atkType, ElementType recipientType) {
		switch (atkType) {

		case ElementType.Fire:
			switch (recipientType) {
			case ElementType.Water:
				return ElementEffectiveness.NonEffective;
			case ElementType.Grass:
				return ElementEffectiveness.Effective;
			default:
				return ElementEffectiveness.Neutral;
			}

		case ElementType.Water:
			switch (recipientType) {
			case ElementType.Grass:
				return ElementEffectiveness.NonEffective;
			case ElementType.Fire:
				return ElementEffectiveness.Effective;
			default:
				return ElementEffectiveness.Neutral;
			}

		case ElementType.Grass:
			switch (recipientType) {
			case ElementType.Fire:
				return ElementEffectiveness.NonEffective;
			case ElementType.Water:
				return ElementEffectiveness.Effective;
			default:
				return ElementEffectiveness.Neutral;
			}
		}

		return ElementEffectiveness.Neutral;
	}

}