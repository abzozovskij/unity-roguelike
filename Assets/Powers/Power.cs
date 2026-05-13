using UnityEngine;

public abstract class Power : ScriptableObject
{
    public string powerName;
    public string description;
    public Sprite icon;

    public abstract void Apply();
}
