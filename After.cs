using NUnit.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace LimitedSizeStack;

public class ListModel<TItem>
{
	
	public Invoker<TItem> Invoker { get; set; }
    public List<TItem> Items { get; set; }
    public int UndoLimit;

    #region ctor
    public ListModel(int undoLimit) : this(new List<TItem>(), undoLimit)
	{
	}

	public ListModel(List<TItem> items, int undoLimit)
	{
		UndoLimit = undoLimit;
        Items = items;
        Invoker = new Invoker<TItem>(UndoLimit);
    }

    #endregion    

	public void AddItem(TItem item)
	{
        Invoker.SetCommand(new AddCommand<TItem>(item));
        Invoker.Run(Items);
	}

	public void RemoveItem(int index)
	{
        Invoker.SetCommand(new RemoveAtCommand<TItem>(index));
        Invoker.Run(Items);
	}

	public bool CanUndo()
	{
		return Invoker.History.Count> 0;
	}

	public void Undo()
	{
        if (CanUndo())
        {
            var command = Invoker.History.Pop();
            Invoker.SetCommand(command);
            Invoker.Cancel(Items);
        }
	}
}

public class Invoker<T>  
{
    public LimitedSizeStack<Icommand<T>> History { get; set; }
    public Icommand<T> command { get; set; }
    public Invoker(int size)
    {
        History = new LimitedSizeStack<Icommand<T>>(size);
    }

    public void SetCommand(Icommand<T> c) => command = c;
    public void Run(List<T> Items)
    {
        command.Do(Items);
        History.Push(command);
    }
    public void Cancel(List<T> Items)
    {
        command.Undo(Items);

    }
}
public class AddCommand<T>:Icommand<T>
{
    private T item;

    public AddCommand(T item) =>this.item = item;


    public void Do(List<T> Items)
    {
        Items.Add(item);
    }
    public void Undo(List<T> Items)
    {
        Items.Remove(item);
    }

}
public class RemoveAtCommand<T>:Icommand<T>
{
    public T Item { get; set; }
    private int index;
    public RemoveAtCommand( int index) => this.index = index;
    public void Do(List<T> Items)
    {
        Item = Items[index];
        Items.RemoveAt(index);
    }
    public void Undo(List<T> Items)
    {
        Items.Add(Item);
        Items.Insert(index, Item);
        Items.RemoveAt(Items.Count-1);
    }
}
public interface Icommand<Q>
{
    public void Do(List<Q> Items);
    public void Undo(List<Q> Items);
}