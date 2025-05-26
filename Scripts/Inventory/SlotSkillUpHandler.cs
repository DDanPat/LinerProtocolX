using UnityEngine;

public class SlotSkillUpHandler : MonoBehaviour
{
    private SkillSlot skillSlot;
    private OperatorSkillUpPopup popup;
    [SerializeField] private int skillType; // 0 active, 123 passive

    private void Start()
    {
        skillSlot = gameObject.GetComponent<SkillSlot>();
        popup = gameObject.GetComponentInParent<OperatorSkillUpPopup>();
        var press = skillSlot.button.GetComponent<SlotPressHandler>();
        
        if (skillSlot?.button != null && popup != null)
        {
            press.onClick = () => popup.SetSkillUpgradeRequired(skillSlot.data, skillType);
            press.onLongPress = () =>
            {
                if (skillSlot.data != null)
                    popup.OpenSkillUpInfoPopup(skillType);
            };
            // skillSlot.button.onClick.AddListener(() => popup.SetSkillUpgradeRequired(skillSlot.data, skillType));
        }
    }
}
