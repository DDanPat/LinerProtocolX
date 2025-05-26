using System;
using UnityEngine;

public class VFX : MonoBehaviour, IPoolable
{
    private Action<GameObject> returnToPool;
    
    public void Initialize(System.Action<GameObject> returnAction)
    {
        returnToPool = returnAction;
    }

    public void OnSpawn()
    {
        
    }

    public void OnDespawn()
    {
        returnToPool?.Invoke(gameObject);
    }

    private void OnParticleSystemStopped()
    {
        OnDespawn();
    }
}
