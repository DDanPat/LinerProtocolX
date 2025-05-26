using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpTest : MonoBehaviour
{
    public void ExpEffect(float exp)
    {

        // 파티클을 위치로 이동
        //Vector3 targetPosition = new Vector3(-7f, 4f, 0f); // 원하는 월드 좌표로 조정
        Vector3 screenTopLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 10f));

        transform.DOMove(screenTopLeft, 1f)
            .SetEase(Ease.InOutSine) // 더 부드럽게
            .SetDelay(0.2f)
            .OnComplete(() =>
            {
                BattleManager.Instance.CurrentBattle.LevelUP(exp);
                Destroy(gameObject);
            });

        
    }
}
