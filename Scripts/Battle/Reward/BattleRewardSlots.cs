using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleRewardSlots : RewardSlotsBase
{
    private void Start()
    {
        itemButton.onClick.AddListener(OnClickItemButton);
    }

    private void OnClickItemButton()
    {
        // TODO 아이템 정보 패널 표시
        BattleManager.Instance.BattleUI?.ItemInfoPopup.OnPupup();
        BattleManager.Instance.BattleUI?.ItemInfoPopup.SetItemPopupInfo(rewardItem, rewardItem.ItemData.icon);
    }
}
