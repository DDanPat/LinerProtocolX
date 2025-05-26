using System;
using UnityEngine.UI;

[Serializable]
public class OperatorInfoTable
{
    public int key;         // 오퍼레이터 키값
    public string name;     // 오퍼레이터 이름
    public string profile;  // 오퍼레이터 프로필 이미지
    public string illust;   // 오퍼레이터 전신 이미지
    public int role;        // 오퍼레이터 역할 0:없음 1:공격형 2:방어형 3:지원형
    public int attack;      // 오퍼레이터 공격력 스탯
    public int attackRate;  // 오퍼레이터 공격속도 스탯
    public int range;       // 오퍼레이터 공격범위 증가 스탯
    public int accuracy;    // 오퍼레이터 명중률 증가 스탯
    public int criticalRate; // 오퍼레이터 크리티컬 확률 증가 스탯
    public int criticalMultiplier;  // 오퍼레이터 크리티컬 데미지 계수 증가 스탯
    public int active;      // 오퍼레이터 액티브 스킬
    public int passvie1;
    public int passvie2;
    public int passvie3;
    public int dedicated;   // 오퍼레이터 전용장비 여부 0:없음
    public string lineIntro;    // 오퍼레이터 등장대사
    public string lineStart;    // 오퍼레이터 
    public string lineSkill;    // 오퍼레이터 스킬 사용대사
    public string lineWin;      // 오퍼레이터 승리 대사
    public string lineDefeat;   // 오퍼레이터 패배 대사
    public string lineLevelUp;  // 오퍼레이터 레벨업 대사
    public string operatorDesc; // 오퍼레이터 설명
}
