using System;

[Serializable]
public class EnemyRoot
{
    public float speedScale = 1f; // 이동 속도 배율
    public float healthScale = 1f; // 체력 배율
    public float shieldScale = 1f; // 실드 배율
    public float defenseScale = 1f; // 방어 배율
    public float expScale = 1f; // 드랍 경험치 배율
    public float goldScale = 1f; // 드랍 골드 배율
    public int goldChance = 0; // 골드 드랍 추가 확률
    public int itemChance = 0; // 아이템 드랍 추가 확률
}
