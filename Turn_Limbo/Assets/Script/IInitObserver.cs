using UnityEngine;

public interface IInitObserver
{
    public void Init();
    public int Priority { get; }
    public GameObject gameObject { get; }
}