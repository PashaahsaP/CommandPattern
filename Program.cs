using System;
using System.Collections.Generic;

namespace LimitedSizeStack;

public class ListModel<TItem>
{
    public List<TItem> Items { get; set; }
    public LimitedSizeStack<(int index,(bool op, TItem value))> History { get; set; }//(index,(append/remove,value))
    public int UndoLimit;
        
    public ListModel(int undoLimit) : this(new List<TItem>(), undoLimit)
    {
    }

    public ListModel(List<TItem> items, int undoLimit)
    {
        Items = items;
        UndoLimit = undoLimit;
        History = new LimitedSizeStack<(int, (bool, TItem))>(undoLimit);
    }

    public void AddItem(TItem item)
    {
        History.Push((Items.Count,(true,item)));
        Items.Add(item);
    }

    public void RemoveItem(int index)
    {
        History.Push((index, (false, Items[index])));
        Items.RemoveAt(index);
    }

    public bool CanUndo()
    {
        return History.Count > 0;
    }

    public void Undo()
    {
        if (CanUndo())
        {
            var temp = History.Pop();
            if (temp.Item2.op)
            {
                Items.RemoveAt(temp.Item1);
            }
            else
            {
                var newList = new List<TItem>();
                newList.AddRange(Items);
                newList.Add(default(TItem));
                newList.Insert(temp.index,temp.Item2.value);
                newList.RemoveAt(newList.Count-1);
                Items = newList;
            }
        }
    }
}