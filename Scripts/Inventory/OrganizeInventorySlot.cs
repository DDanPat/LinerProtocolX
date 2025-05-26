using System;
using UnityEngine;
using UnityEngine.UI;

public class OrganizeInventorySlot : InventorySlot
{
    public GameObject usedIcon;
    [SerializeField] private bool _used;
    public bool used { get => _used; 
        set { _used = value; 
            usedIcon?.SetActive(_used); } }
    
    [SerializeField] private Outline outLine_default;
    [SerializeField] private Image outLine;
    [SerializeField] private Material[] materials= new Material[5];
    
    public override void UpdateDataIcon()
    {
        base.UpdateDataIcon();
        used = false;

        if (slotType == SLOTTYPE.OPERATOR && data != null)
        {
            var opData = data as OperatorData;
            int grade = GameManager.Instance.Player.inventory.operators[opData.operatorInfoTable.key].grade;

            outLine_default.enabled = !GameManager.Instance.Player.inventory.
                                            operators[opData.operatorInfoTable.key].isOwned;
            outLine.gameObject.SetActive(!outLine_default.enabled);
            outLine.material = materials[grade - 1];
            
        }
    }

    public override void UpdateUsed(bool isUsed)
    {
        used = isUsed;
    }

    public override void ClearSlot()
    {
        base.ClearSlot();
        used = false;
    }

    public void OpenInfoPopup()
    {
        UIManager.Instance.organizeUI.OpenInfoPopup(data);
    }
}
