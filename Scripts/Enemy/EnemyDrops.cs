using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class EnemyDrops : Singleton<EnemyDrops>
{
    ObjectPoolManager objectPoolManager;

    public GameObject[] drops;
    
    private void Start()
    {
        objectPoolManager = ObjectPoolManager.Instance;
    }

    public void Drops(ObjectType type, Transform pos, float randomExp)
    {        
        GameObject dropsParticle = objectPoolManager.GetObject(type, pos.position, Quaternion.identity);
        dropsParticle.GetComponent<ExpDropEffect>().PlayEffect(randomExp);
    }
}
