using System.Collections;

public sealed class LinkedListNode<T> {

	public T Value { get; private set; }
	
	//Next node in the list
	public LinkedListNode<T> Next { get; internal set; }
	
	//previous node in the list
	public LinkedListNode<T> Previous {get; internal set; }
	
	internal LinkedListNode(T item)
	{
		this.Value = item;	
	}
}
