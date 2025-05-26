using UnityEngine;

public enum ItemType
{
    CURRENCY = 0,     // 기타
    EXP = 1,      // 오퍼레이터 경험치 강화 아이템
    UPGRADE = 2, // 등급 ,스킬 강화 아이템
    WEAPONREINFORCE = 3,   // 장비 강화 아이템
    WEAPONUPGRADE = 4,  // 장비 승급 아이템
    CRAFTING = 5,       // 제작 아이템
}

[CreateAssetMenu(fileName ="Item", menuName = "Data/Item")]
public class ItemData : ScriptableObject
{
    public int key;
    public string name;
    public string desc;
    public ItemType type;
    public int exp;
    
    public string iconAddress;
    public Sprite icon;
}
