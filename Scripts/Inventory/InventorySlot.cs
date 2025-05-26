using System;
using UnityEngine;
using UnityEngine.UI;

public interface IInventorySlot
{
    Sprite GetIcon();
    string GetName();
}

public enum SLOTTYPE
{
    TOWER,
    OPERATOR,
    ITEM,
    SKILL,
    WEAPON
    
}

public class InventorySlot : MonoBehaviour, IInventorySlot
{
    public SLOTTYPE slotType;
    public Image icon;
    public Button button;
    [SerializeField]private ScriptableObject _data;
    public ScriptableObject data { get => _data; set { _data = value; UpdateDataIcon(); } 
        // 자동으로 아이콘 업데이트
    }
    
    [SerializeField] private Sprite defaultIcon;

    public virtual void UpdateDataIcon()
    {
        if (slotType == SLOTTYPE.TOWER && data is TowerData towerData)
        {
            SetIcon(towerData?.profile);
        }
        else if (slotType == SLOTTYPE.OPERATOR && data is OperatorData operatorData)
        {
            SetIcon(operatorData?.profile);
        }
        else if(slotType == SLOTTYPE.ITEM && data is ItemData itemData)
        {
            SetIcon(itemData?.icon);
        }
        else if (slotType == SLOTTYPE.SKILL && data is ActiveSkillData activeSkillData)
        {
            SetIcon(activeSkillData?.icon);
        }
        else if (slotType == SLOTTYPE.SKILL && data is PassiveSkillData passiveSkillData)
        {
            SetIcon(passiveSkillData?.icon);
        }
        else if (data == null && defaultIcon != null)
        {
            SetIcon(defaultIcon);
        }
    }

    public virtual void UpdateUsed(bool isUsed)
    {
       
    }


    public Sprite GetIcon()
    {
        return icon.sprite;
    }

    public void SetIcon(Sprite sprite)
    {
        icon.sprite = sprite;
        icon.enabled = true;
        
        if (sprite != null)
        {
            Color c = icon.color;
            c.a = 1f;
            icon.color = c;
        }
        else
        {
            Color c = icon.color;
            c.a = 0f;
            icon.color = c;
        }
        
    }

    public string GetName()
    {
        if (data is TowerData towerData)
        {
            return towerData.towerInfoTable.name;
        }
        else if (data is OperatorData operatorData)
        {
            return operatorData.operatorInfoTable.name;
        }

        return null;
    }

    public virtual void ClearSlot()
    {
        data = null;
        // itemSlots.enabled = false;
    }

    public virtual void SwapSlot(InventorySlot slot)
    {
        (data, slot.data) = (slot.data, data); // 속성이므로 자동 업데이트 작동
        // (slot.itemSlots.sprite, itemSlots.sprite) = (itemSlots.sprite, slot.itemSlots.sprite);
    }
}
