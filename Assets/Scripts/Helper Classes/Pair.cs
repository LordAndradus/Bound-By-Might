using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Pair<F, S>
{
    [field: SerializeField] public F First { get; set; }
    [field: SerializeField] public S Second { get; set; }

    public Pair(Pair<F, S> copier)
    {
        copy(copier);
    }

    public Pair(F First, S Second)
    {
        this.First = First;
        this.Second = Second;
    }

    public Pair((F, S) TupplePair)
    {
        this.First = TupplePair.Item1;
        this.Second = TupplePair.Item2;
    }

    public void set(F First, S Second)
    {
        this.First = First;
        this.Second = Second;
    }

    public void copy(Pair<F, S> copier)
    {
        this.First = copier.First;
        this.Second = copier.Second;
    }

    public bool equals(Pair<F, S> compare)
    {
        return First.Equals(compare.First) && Second.Equals(compare.Second);
    }

    public override bool Equals(object obj)
    {
        if(obj is Pair<F,S> other)
        {
            return equals(other);
        }

        return base.Equals(obj);
    }

    public (F, S) ToTuple()
    {
        return (First, Second);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(First, Second);
    }

    public bool InList(List<Pair<F,S>> list)
    {
        foreach(var pair in list.ToList()) if(pair.equals(this)) return true;
        return false;
    }

    public string DetailedToString()
    {
        return "Pair<" + typeof(F) + ", " + typeof(S) + "> = (" + First + ", " + Second + ")";
    }

    public override string ToString()
    {
        return string.Format("({0}, {1})", First, Second);
    }
}