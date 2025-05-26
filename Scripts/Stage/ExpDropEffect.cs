using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
public enum EffectType
{
    Exp,
    Gold
}

public class ExpDropEffect : MonoBehaviour, IPoolable
{
    [SerializeField] private EffectType effectType;
    [SerializeField] private ExpDropEffect expDropEffectPrefab;

    private Action<GameObject> returnToPool;

    public void PlayEffect(float exp)
    {
        DieEffect(exp);
    }
    public void DieEffect(float exp)
    {
        Vector3 movePoint = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 10f));

        // 파티클을 위치로 이동
        switch (effectType)
        {
            case EffectType.Exp:
                movePoint = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 25f));
                break;
            case EffectType.Gold:
                movePoint = TileManager.Instance.headquarterTransform.position;
                break;
        }
        
        

        transform.DOMove(movePoint, 1f)
            .SetDelay(0.2f)
            .OnComplete(() =>
            {
                switch (effectType)
                {
                    case EffectType.Exp:
                        BattleManager.Instance.CurrentBattle.LevelUP(exp);
                        break;
                    case EffectType.Gold:
                        int gold = Mathf.RoundToInt(exp);
                        BattleManager.Instance.CurrentBattle.TakeGold(gold);
                        break;
                }
                OnDespawn();
            });
    }

    public void Initialize(Action<GameObject> returnAction)
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
}