using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * This class is a wrapper around the System.SortedList class.
 * It provides the feature of tracking the "largest value" (milestone) up to some "point in time".
 * 	The "largest value" can be defined via a Comparator.
 * 	The "point in time" might be a timestamp, but it can simply be some value that provides a reference-point for ordering to the values in the TimeseriesMilestoneList.
 * 
 * The original intended use-case for this TimeseriesMilestoneList is to track the largest received stat buff at the current point in time.
 * Stat buffs of varying "largeness" might be spammed on a player. They do not stack, so only the largest needs to be tracked for the player.
 * These buffs might also have varying durations. Once the current largest buff expires, the next largest spammed buff should be tracked at the next index of the list.
 * 
 * So even if countless buffs have been applied to a player:
 * 	[value: 50%, duration 1.5 sec]
 * 	[value: 40%, duration 2 sec]
 * 	[value: 30%, duration 3 sec]
 * 	[value: 20%, duration 3 sec]
 * 	...
 * 
 * Assuming that 50% is the largest received buff, and the above list snippet is ordered, then the MilestoneList would appear as follows:
 * 
 * TimeseriesMilestoneList[
 * 	[1.5: 50],
 * 	[2: 40],
 * 	[3: 30]
 * ]
 * 
 * , and the [3 sec, 20%] [key, value] pair would be discarded bcus the largest "milestone" at the 3 sec mark would have been 30%;
 * there would be no reason to know that a smaller 20% buff had been received bcus the larger 30% buff would be prioritized.
 * 
 * ***The fundamental property of this list is that every entry to the left is bigger or equal to every entry to the right,
 * 		and every entry to the left expires before every entry to the right
 * 
 */

public class TimeseriesMilestoneList<K, V>
	where K : System.IComparable<K>, System.IComparable<float>
	where V : System.IComparable {

	private SortedList<K, V> list;

	public TimeseriesMilestoneList() {
		this.list = new SortedList<K, V> ();
	}

	public void Add(K key, V newValue) {

		// 1. Add key, value
		try {
			// 1.1. Add
			this.list.Add(key, newValue);

		} catch(System.ArgumentException e) { // Key already exists
			V existingValue = this.list [key];

			// 1.2. Replace existing value if new value is greater
			if (existingValue.CompareTo(newValue) < 0)
				this.list [key] = newValue;
			else
				// Nothing added or changed; do nothing more
				return;

		}

		// 2. Get the index of the just added key, value pair
		int index = this.list.IndexOfValue(newValue);

		// 3. Check right of list: Remove newly added value if it is smaller than the entry directly to the right
		V rightValue = this.list.Values[index + 1];
		if (rightValue.CompareTo (newValue) > 0) {
			// 3.1. Remove, so then nothing was added or changed; do nothing more
			this.list.Remove (key);
			return;
		}

		// 4. Check left of list: Remove stale entries between the newly added index and the start of the list
		for(int i = index - 1; i >= 0; i--) {

			// 4.1. Smaller left entries are removed; the newly added entry will be peeked instead
			V leftValue = this.list.Values [i];
			if (leftValue.CompareTo(newValue) <= 0)
				this.list.Remove (list.Keys [i]);
			else
				// Do not keep checking to left if bigger value is found; left vales will only continue to get bigger
				break;
		}

	}

	public V peek() {
		this.cleanStale (Time.time);

		return this.list.Count > 0 ? this.list.Values [0] : default(V);
	}

	public int cleanStale(float milestone) {
		int rmCounter = 0;

		// 1. Remove stale key, value pairs
		while (this.list.Count > 0 && this.list.Keys [0].CompareTo(milestone) < 0) {
			this.list.RemoveAt (0);

			rmCounter++;
		}

		return rmCounter;
	}

	public int count() {
		return this.list.Count;
	}

	public bool isEmpty() {
		return this.list.Count <= 0;
	}
}
