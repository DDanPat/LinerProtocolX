using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class SkillButton : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler, IPointerDownHandler
{
    [SerializeField] private Image operatorImage;
    [SerializeField] private GameObject operatorMask;
    [SerializeField] private Image shodow;
    [SerializeField] private ParticleSystem _uIParticle;
    // 드래그 시작시 호출 함수
    public UnityAction<int, int> OnStartDragAction;
    // 드래그 종료시 호출 함수
    public UnityAction<int> OnEndDragAction;
    public int skillKey; // 스킬 키
    private int operatorKey;

    private int skillLevel;
    private float coolDown = 2.0f;
    private float lastUseTime = float.MinValue;
    private int skillTrigger = 0; // 즉시, 드래그
    private bool canUse = true; // 사용 가능 여부

    private bool useDrag; // 드래그 사용 여부
    public float longClickThreshold = 1.0f; // 롱클릭으로 인식할 시간(초)
    private bool isPointerDown = false; // 중복 실행 방지용
    private float pointerDownTimer = 0f;

    private ActiveSkillLevelTable skillData;
    private ActiveSkillTable skillTable;
    void Update()
    {
        // 쿨타임 시간 갱신
        if (Time.time - lastUseTime >= coolDown)
        {
            // 그림자 비활성화
            if (shodow != null)
            {
                shodow.enabled = false;
                shodow.fillAmount = 0f;
            }
            // 소환 활성화
            canUse = true;
            // 파티클 재생
            // _uIParticle.Play();
        }
        else
        {
            // 그림자 활성화, 비율로 채우기
            if (shodow != null)
            {
                shodow.enabled = true;
                shodow.fillAmount = 1f - (Time.time - lastUseTime) / coolDown;
            }
        }

        // 롱 클릭 타이머 진행,
        if (isPointerDown)
        {
            pointerDownTimer += Time.deltaTime;
        }
        // 자동으로 팝업
        if (pointerDownTimer >= longClickThreshold)
        {
            // 한번만 실행하도록
            isPointerDown = false;
            // 길게 누름 - 설명문 표기
            ShowDescript();
            pointerDownTimer = 0f;
        }

    }
    public void Setup(TowerOperList info)
    {
        int originSkill = info.operatorData.operatorInfoTable.active;
        operatorImage.sprite = info.operatorData.profile;
        operatorKey = info.operatorData.operatorInfoTable.key;
        // 오퍼레이터 레벨 데이터 연결
        skillLevel = info.operatorData.activeLv;
        skillLevel = Mathf.Max(skillLevel, 1);
        skillKey = originSkill * 10 + skillLevel;

        // 기본 정보 가져오기
        skillTable = SkillManager.Instance.skillDict[originSkill];
        skillTrigger = skillTable.trigger;

        // 스킬 레벨로 상세 정보 가져오기
        skillData = SkillManager.Instance.skillLevelDict[skillKey];
        coolDown = skillData.cooldown;

        operatorImage.gameObject.SetActive(true);
        operatorMask.SetActive(true);

        shodow.fillAmount = 0f;
        // 버튼 이벤트 등록
        GetComponent<Button>().onClick.AddListener(PointerUp);
        useDrag = true;
        // if (skillTrigger != 0)
        // {
        //     useDrag = true;
        // }
    }

    // 버튼 클릭
    public void PointerUp()
    {
        // 한번만 실행하도록
        isPointerDown = false;

        if (pointerDownTimer < longClickThreshold)
        {
            // 짧게 누름 - 스킬 사용
            //UseSkill();
        }
        else if (pointerDownTimer >= longClickThreshold)
        {
            // 길게 누름 - 설명문 표기
            ShowDescript();
        }
        pointerDownTimer = 0f;
    }

    // 설명문 표기
    public void ShowDescript()
    {
        useDrag = false;
        // 시간 정지
        Time.timeScale = 0;
        // 정보 가져오기, 팝업 표시
        string descript = SkillManager.Instance.GetName(skillKey) + SkillManager.Instance.GetDescript(skillKey);
        BattleManager.Instance.BattleUI.descriptPopup.Open(operatorImage.sprite, descript, OnCloseDescript);
    }
    public void OnCloseDescript()
    {
        useDrag = true;
        // 시간 원래대로
        Time.timeScale = BattleManager.Instance.BattleUI.SettingCurGameSpeed();
    }
    public void UseSkill()
    {
        // 버튼으로 즉시 사용 스킬 사용
        if (canUse)
        {
            canUse = false;
            // 스킬 사용
            SkillManager.Instance.UseSkill(skillKey, operatorKey);
            // 쿨타임 갱신
            lastUseTime = Time.time;
            // 스킬 정보 표시
            Sprite operatorSprite = BattleManager.Instance.GetTowerInfo(operatorKey).operatorData.profile;
            Sprite skillSprite = BattleManager.Instance.GetTowerInfo(operatorKey).operatorData.activeSkillData.icon;
            string skillName = SkillManager.Instance.skillDict[skillKey / 10].name;
            BattleManager.Instance.BattleUI.AddSkillQueue(operatorSprite, skillSprite, skillName);
        }
    }
    public int GetOperatorKey()
    {
        return operatorKey;
    }
    // 쿨타임 감소(초)
    public void ReduceTime(float time)
    {
        lastUseTime -= time;
    }
    // 쿨타임 감소(퍼센트)
    public void ReduceTimePercent(float percent)
    {
        float time = coolDown * percent / 100f;
        lastUseTime -= time;
    }
    // 쿨타임 다시 적용
    public void ApplyCoolTime()
    {
        canUse = false;
        lastUseTime = Time.time;
    }

    #region MouseAction
    // 드래그 시작
    public void OnBeginDrag(PointerEventData eventData)
    {
        // 드래그 시작
        if (useDrag && canUse)
        {
            // 드래그면 롱 클릭 취소
            isPointerDown = false;
            // 이벤트 실행
            OnStartDragAction?.Invoke(skillKey, operatorKey);
        }
    }
    // 드래그 동안 위치 이동
    public void OnDrag(PointerEventData eventData)
    {
        if (useDrag && canUse)
        {
            // 드래그시 위치 이동
            transform.position = eventData.position;
        }
    }

    // 드래그 종료 - 스킬 사용
    public void OnEndDrag(PointerEventData eventData)
    {
        if (useDrag && canUse)
        {
            // layout에서 원위치
            gameObject.SetActive(false);
            gameObject.SetActive(true);
            // 콜백 실행 - 위치 스킬 사용
            OnEndDragAction?.Invoke(skillKey);
        }
    }

    // 누름 시작,
    public void OnPointerDown(PointerEventData eventData)
    {
        isPointerDown = true;
        pointerDownTimer = 0f;
    }
    #endregion
}
