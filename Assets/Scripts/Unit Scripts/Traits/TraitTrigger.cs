using System;

public interface TraitTrigger
{
    public Action<Unit> trigger { get; set; }
    public void Trigger(Unit u);
}