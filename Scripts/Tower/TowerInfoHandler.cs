using UnityEngine;
using UnityEngine.EventSystems;
[RequireComponent(typeof(BoxCollider))]
public class TowerInfoHandler : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Tower tower;
    public void OnPointerClick(PointerEventData eventData)
    {
        // 타워 클릭 시 정보 팝업
        ShowInfo();
    }
    public void ShowInfo()
    {
        // 정보 팝업 표시
        if (tower != null && tower.isBuild)
        {
            BattleManager.Instance.BattleUI.battleTowerInfo.Set(tower);
        }
    }
}
