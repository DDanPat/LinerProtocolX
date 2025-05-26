using UnityEngine;
using UnityEngine.UI;

public class InventoryOperatorSlot : InventorySlot
{
    public Outline outLine_default;
    public Image outLine;
    [SerializeField] private Material[] materials = new Material[5];


    public override void UpdateDataIcon()
    {
        base.UpdateDataIcon();
        var opData = data as OperatorData;
        int grade = GameManager.Instance.Player.inventory.operators[opData.operatorInfoTable.key].grade;

        outLine_default.enabled = !GameManager.Instance.Player.inventory.
                                        operators[opData.operatorInfoTable.key].isOwned;
        outLine.gameObject.SetActive(!outLine_default.enabled);
        outLine.material = materials[grade - 1];

        // urp 셰이더 빌드해서 나올때까지 임시
        // switch (grade)
        // {
        //     case 1:
        //         outLine_default.effectColor = new Color(127f / 255f, 127f / 255f, 127f / 255f, 1f);
        //         break;
        //     case 2:
        //         outLine_default.effectColor = new Color(31f / 255f, 184f / 255f, 31f / 255f, 1f);
        //         break;
        //     case 3:
        //         outLine_default.effectColor = new Color(0f / 255f, 127f / 255f, 255f / 255f, 1f);
        //         break;
        //     case 4:
        //         outLine_default.effectColor = new Color(191f / 255f, 63f / 255f, 191f / 255f, 1f);
        //         break;
        //     case 5:
        //         outLine_default.effectColor = new Color(255f / 255f, 191f / 255f, 0f / 255f, 1f);
        //         break;
        // }
    }// 
}
