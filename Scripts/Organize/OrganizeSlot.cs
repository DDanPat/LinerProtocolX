using UnityEngine;
using UnityEngine.UI;

public class OrganizeSlot : MonoBehaviour
{
    public SLOTTYPE slotType;
    public Image icon;
    [SerializeField] private Sprite clearSprite;
    private ScriptableObject _data;
    public ScriptableObject data
    {
        get => _data;
        private set
        {
            _data = value;

            switch (slotType)
            {
                case SLOTTYPE.TOWER when _data is TowerData towerData:
                    SetIcon(towerData.profile);
                    break;
                case SLOTTYPE.OPERATOR when _data is OperatorData operatorData:
                    SetIcon(operatorData.profile);
                    break;
                default:
                    ClearIcon();
                    break;
            }
        }
    }

    public void SetData(ScriptableObject itemData)
    {
        data = itemData;
    }
    
    public ScriptableObject GetData()
    {
        return data;
    }
    
    public bool CheckTypeData(ScriptableObject data)
    {
        if (slotType == SLOTTYPE.TOWER && data is TowerData)
            return true;
        if (slotType == SLOTTYPE.OPERATOR && data is OperatorData)
            return true;

        return false;
    }
    
    public void SetIcon(Sprite sprite)
    {
        icon.sprite = sprite;
        
        Color c = icon.color;
        c.a = 1f;
        icon.color = c;
    }

    public bool CheckData()
    {
        if(data != null)
            return true;
        return false;
    }
    
    public void ClearIcon()
    {
        icon.sprite = clearSprite;
        
        Color c = icon.color;
        c.a = 50f / 255f;
        icon.color = c;
    }

    public void ClearSlot()
    {
        data = null;
    }
    
    public void SwapSlot(OrganizeSlot slot)
    {
        if (CheckTypeData(slot.data))
        {
            ScriptableObject temp = data;
            data = slot.GetData();
            slot.SetData(temp);
        }
    }
}
