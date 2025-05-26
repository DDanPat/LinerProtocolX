using System;
using UnityEngine;

public class TutorialManager : Singleton<TutorialManager>
{
    
    [SerializeField] private RectTransform tutorialOrganizePrefab;
    public Tutorial_Organize tutorial_Organize;
    public bool organizeTutorial
    {
        get => PlayerPrefs.GetInt("TutorialOrganize", 0) == 1;
        private set => PlayerPrefs.SetInt("TutorialOrganize", value ? 1 : 0);
    }
    
    public void Intialize()
    {
        if (!organizeTutorial && tutorial_Organize == null)
        {
            RectTransform parent = GameObject.Find("LobbyUI")?.GetComponent<RectTransform>();
            tutorial_Organize = Instantiate(tutorialOrganizePrefab, parent, false).GetComponent<Tutorial_Organize>();
        }
    }

    public void StartOrganizeTutorial_1()
    {
        SlotDragHandler drag = UIManager.Instance.organizeUI.towerSlots[0].GetComponentInChildren<SlotDragHandler>();
        drag.dragAction += TutorialNext;
        
        var start = UIManager.Instance.organizeUI.towerSlots[0].GetComponent<RectTransform>();
        var end = UIManager.Instance.organizeUI.organizeSlotManger.GetTutorialSlotPosition(0);
        tutorial_Organize.SetTutorialTarget(start, end);
        tutorial_Organize.StartOrganizeTutorial();
    }

    public void StartOrganizeTutorial_2()
    {
        var slot = UIManager.Instance.organizeUI.organizeSlotManger.GetTutorialSlotPosition(0);
        OrganizeDragHandler drag = slot.gameObject.GetComponent<OrganizeDragHandler>();
        drag.action += tutorial_Organize.StopParticularTutorial;
        
        slot = UIManager.Instance.organizeUI.organizeSlotManger.GetTutorialSlotPosition(1);
        drag = slot.gameObject.GetComponent<OrganizeDragHandler>();
        drag.action += tutorial_Organize.StopParticularTutorial;
        
        tutorial_Organize.StartParticularTutorial();
    }

    private void TutorialNext()
    {
        SlotDragHandler drag = UIManager.Instance.organizeUI.towerSlots[0].GetComponentInChildren<SlotDragHandler>();
        drag.dragAction -= TutorialNext;
        
        tutorial_Organize.StopOrganizeTutorial();
        StartOrganizeTutorial_2();
    }

    public void EndOrganizeTutorial()
    {
        PlayerPrefs.SetInt("TutorialOrganize", 1);
        
        var slot = UIManager.Instance.organizeUI.organizeSlotManger.GetTutorialSlotPosition(0);
        OrganizeDragHandler drag = slot.gameObject.GetComponent<OrganizeDragHandler>();
        drag.action -= tutorial_Organize.StopParticularTutorial;
        
        slot = UIManager.Instance.organizeUI.organizeSlotManger.GetTutorialSlotPosition(1);
        drag = slot.gameObject.GetComponent<OrganizeDragHandler>();
        drag.action -= tutorial_Organize.StopParticularTutorial;
        
        
        organizeTutorial = true;
    }
}
