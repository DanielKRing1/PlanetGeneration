using System;
using System.Linq;

public class Heap<T> {

	private T[] heapList;
	private int curSize;

	// Return >= 0 if arg1 should be higher in heap, else arg2 should be higher in heap
	private Func<T, T, float> comparator;

	public Heap(int capacity, Func<T, T, float> comparator) {
		this.heapList = new T[capacity];
		this.curSize = 0;

		this.comparator = comparator;
	}

	public void add(T item) {
		// 1. If list full, increase its size
		if (this.isFull())
			this.grow ();

		// 2. Init starting insertion position, at end of list
		int insertionIndex = this.curSize;
		// 3. Swap with parent until heap condition met
		while (insertionIndex > 0 && this.comparator(this.heapList[insertionIndex], this.parent(insertionIndex)) >= 0) {
			// 4. Swap
			int parentIndex = this.parentIndex(insertionIndex);
			this.swap(insertionIndex, parentIndex);

			// 5. Update current insertion index
			insertionIndex = parentIndex;
		}

		this.curSize++;
	}

	public T peek() {
		return this.heapList [0];
	}

	public T pop() {
		// 1. Store root value
		T poppedData = this.peek ();

		// 2. Replace root with last element
		this.heapList[0] = this.heapList[this.curSize];
		this.curSize--;

		return poppedData;
	}

	public void filter(Func<T, bool> condition) {
		this.heapList = this.heapList.Where(i => condition(i)).ToArray<T>();
	}

// PRIVATE

	private void heapify(int parentIndex) {
		// 1. Get left and right items
		int l = this.leftIndex (parentIndex);
		int r = this.rightIndex (parentIndex);
		int top = parentIndex;

		// 2. Check if left should be moved to top
		if (this.comparator (this.heapList[l], this.heapList[top]) > 0)
			top = l;

		// 3. Check if right should be moved to top
		if (this.comparator (this.heapList[r], this.heapList[top]) > 0)
			top = r;

		// 4. Continue heapifying down tree until parentIndex satisfies heap condition
		if (parentIndex != top) {
			this.swap (parentIndex, top);

			// 5. "Top" is now swapped with the parent index, who may still need to be heapified
			this.heapify (top);
		}
	}

	private T parent(int childIndex) {
		return this.heapList[this.parentIndex(childIndex)];
	}
	private int parentIndex(int childIndex) {
		return (childIndex - 1) / 2;
	}

	private T left(int parentIndex) {
		return this.heapList[this.leftIndex(parentIndex)];
	}
	private int leftIndex(int parentIndex) {
		return parentIndex * 2 + 1;
	}

	private T right(int parentIndex) {
		return this.heapList[this.rightIndex(parentIndex)];
	}
	private int rightIndex(int parentIndex) {
		return parentIndex * 2 + 2;
	}

	private void swap(int index1, int index2) {
		T temp = this.heapList [index1];

		this.heapList [index1] = this.heapList [index2];
		this.heapList [index2] = temp;
	}

	public bool isEmpty() {
		return this.getSize () <= 0;
	}

	private bool isFull() {
		return this.getSize() >= this.getCapacity();
	}

	public int getSize() {
		return this.curSize;
	}

	public int getCapacity() {
		return this.heapList.Length;
	}

	private void grow() {
		// 1. Create new list with double capacity
		int newCapacity = this.heapList.Length * 2;
		T[] newList = new T[newCapacity];

		// 2. Copy existing heap list to new list
		Array.Copy (this.heapList, 0, newList, 0, this.heapList.Length);
		// 3. Set new list as heap list
		this.heapList = newList;
	}

	public T[] toArray() {
		return (T[]) this.heapList.Clone ();
	}
}
