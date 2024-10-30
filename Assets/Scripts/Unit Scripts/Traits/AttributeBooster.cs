using System;

public interface AttributeBooster
{
    public Action<Unit> add { get; set; }
    public Action<Unit> remove { get; set; }

    public void Add(Unit u);
    public void Remove(Unit u);
}