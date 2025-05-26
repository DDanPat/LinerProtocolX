using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialUIHandler : MonoBehaviour
{
    public TextMeshProUGUI tutorialName;
    public List<TutorialPanel>  tutorialPanels;
    [SerializeField] private GameObject parentPanel;
    
    private void Awake()
    {
        StartCoroutine(WaitForUIManagerAndRegister());
    }

    private IEnumerator WaitForUIManagerAndRegister()
    {
        yield return new WaitUntil(() => UIManager.Instance != null);
        UIManager.Instance.tutorialUIHandler = this;
        gameObject.SetActive(false);
    }

    public void OpenTutorialPanel(TUTORIALTYPE type)
    {
        gameObject.SetActive(true);


        foreach (TutorialPanel tutorialPanel in tutorialPanels)
        {
            if (tutorialPanel.type == type)
            {
                tutorialName.text = tutorialPanel.tutorialName;
                Instantiate(tutorialPanel.tutorialPanels, parentPanel.transform);
            }
        }
    }

    public void CloseTutorialPanel()
    {
        for (int i = parentPanel.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(parentPanel.transform.GetChild(i).gameObject);
        }
        gameObject.SetActive(false);
    }
    
}

[Serializable]
public class TutorialPanel
{
    public TUTORIALTYPE type;
    public string tutorialName;
    public RectTransform tutorialPanels;
}
