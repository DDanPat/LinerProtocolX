using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UITowerInfo : MonoBehaviour
{
    [SerializeField] Image towerImage; // 타워 이미지
    [SerializeField] Image operatorImage; // 오퍼레이터 이미지
    [SerializeField] Button closeBtn; // 닫기 버튼
    [SerializeField] Button activeBtn; // 스킬 버튼

    [SerializeField] TextMeshProUGUI towerName; // 이름
    [SerializeField] TextMeshProUGUI towerDes; // 설명문
    [SerializeField] TextMeshProUGUI towerAttack; // 공격력
    [SerializeField] TextMeshProUGUI towerRange; // 사거리
    [SerializeField] TextMeshProUGUI towerType; // 공격 타입
    [SerializeField] TowerData towerData;
    [SerializeField] OperatorData OperatorData;

    void Awake()
    {
        activeBtn.onClick.AddListener(ActiveSkill);
        closeBtn.onClick.AddListener(Close);
    }
    // Start is called before the first frame update
    void Start()
    {
        // 테스트용
        //SetUp(towerData, OperatorData);
        Close();
    }
    public void SetUp(TowerTable towerData, OperatorData operatorData)
    {
        gameObject.SetActive(true);
        //towerImage.sprite = towerData.itemSlots;
        towerName.text = towerData.name;
        towerDes.text = towerData.desc;
        towerAttack.text = towerData.attack.ToString();
        towerRange.text = towerData.range.ToString();
        towerType.text = towerData.attackType.ToString();

        if (operatorData != null)
        {
            activeBtn.gameObject.SetActive(true);
            operatorImage.sprite = operatorData.profile;
        }
        else
        {
            activeBtn.gameObject.SetActive(false);
        }
    }

    public void ActiveSkill()
    {

    }
    public void Close()
    {
        gameObject.SetActive(false);
    }
}
