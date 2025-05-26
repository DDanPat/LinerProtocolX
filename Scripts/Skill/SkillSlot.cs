using UnityEngine;
using UnityEngine.UI;

public class SkillSlot : InventorySlot
{
    public Image backGround;
    public Sprite defaultSprite;
    public Sprite fillSprite;
    
    public int GetKey()
    {
        if (data is ActiveSkillData activeSkillData)
        {
            return activeSkillData.skillTable.key;
        }
        else if (data is PassiveSkillData passiveSkillData)
        {
            return passiveSkillData.skillTable.key;
        }

        return 0;
    }

    public void SetDefaultBackground()
    {
        backGround.sprite = defaultSprite;
    }

    public void SetFillSprite()
    {
        backGround.sprite = fillSprite;
    }
}
