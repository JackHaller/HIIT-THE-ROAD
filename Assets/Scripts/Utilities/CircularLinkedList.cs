using System.Collections;
using System;

public class CircularLinkedList<T> {

	private LinkedListNode<T> head = null;
	private LinkedListNode<T> tail = null;
	
	public LinkedListNode<T> Head {
		get {
			return head;	
		}
	}
	
	public LinkedListNode<T> Tail {
		get {
			return tail;	
		}
	}
	
	public int Count { get; private set; }
	
	public CircularLinkedList() {
		Count = 0;	
	}
	
	public void AddLast(T item) {
		if (item == null) {
			throw new ArgumentNullException("item");	
		}
		if (head == null) {
			AddFirstItem(item);
		} else {
			LinkedListNode<T> newNode = new LinkedListNode<T>(item);
			tail.Next = newNode;
			newNode.Next = head;
			newNode.Previous = tail;
			tail = newNode;
			head.Previous = tail;
		}
		Count++;
	}
	
	public void AddFirst(T item) {
		if (item == null) {
			throw new ArgumentNullException("item");	
		}
		if (head == null) {
			AddFirstItem(item);	
		} else {
			LinkedListNode<T> newNode = new LinkedListNode<T>(item);
			head.Previous = newNode;
			tail.Next = newNode;
			newNode.Previous = tail;
			newNode.Next = head;
			head = newNode;
		}
		Count++;
	}
	
	//Handles the case that we are adding an item to an empty list
	private void AddFirstItem(T item) {
		head = new LinkedListNode<T>(item);
		tail = head;
		head.Next = tail;
		head.Previous = tail;
	}
	
	public LinkedListNode<T> getFirst(){
		return Head;
	}
	
	//Access the list by index
	public LinkedListNode<T> this[int index] {
		get {
			if (index >= Count || index < 0) {
				throw new ArgumentOutOfRangeException("index");	
			} else {
				LinkedListNode<T> node = head;
				for (int i = 0; i < index; i++) {
					node = node.Next;	
				}
				return node;
			}
		}
	}
	
	public void RemoveFirst() {
		if (Count == 0) {
			//List is empty, do nothing
			return;	
		} else if (Count == 1) {
			//only one item, remove it
			head = null;
			tail = null;
			Count--;
		} else {
			LinkedListNode<T> newHead = head.Next;
			head.Next = null;
			head.Previous = null;
			head = newHead;
			head.Previous = tail;
			tail.Next = newHead;
			Count--;
		}
	}
	
	public void RemoveLast() {
		if (Count == 0) {
			//List is empty, do nothing
			return;	
		} else if (Count == 1) {
			//only one item, remove it
			head = null;
			tail = null;
			Count--;
		} else {
			LinkedListNode<T> newTail = tail.Previous;
			tail.Next = null;
			tail.Previous = null;
			tail = newTail;
			head.Previous = newTail;
			tail.Next = head;
			Count--;
		}
	}
}
