using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class OpenSystemData
{
    public Sprite icon;
    public string Desc;
}

public class OpenSystemPopup : MonoBehaviour
{
    [SerializeField] private List<OpenSystemData> openSystemDatalist;

    [SerializeField] private GameObject popupObj;
    [SerializeField] private Image popupIcon;
    [SerializeField] private TextMeshProUGUI popupDesc;
    [SerializeField] private Button returnButton;

    private const string OpenSystemKeyPrefix = "OpenSystem_";

    private Queue<int> popupQueue = new Queue<int>();
    private HashSet<int> popupIndexSet = new HashSet<int>();

    private bool isPopupActive = false;


    private void Awake()
    {
        returnButton.onClick.AddListener(OnClickReturnButton);
        popupObj.SetActive(false);
    }

    public void SetOpenSystemPopup(int index)
    {
        // 이미 등록된 큐인지 확인하고 동일 큐가 있으면 return
        if (IsAlreadyOpened(index) || popupIndexSet.Contains(index))
            return;

        popupQueue.Enqueue(index);
        popupIndexSet.Add(index);
        TryShowNextPopup();
    }

    private void TryShowNextPopup()
    {
        if (isPopupActive || popupQueue.Count == 0)
            return;

        int index = popupQueue.Dequeue();
        popupIndexSet.Remove(index);

        if (index < 0 || index >= openSystemDatalist.Count)
        {
            Debug.LogWarning($"Invalid popup index: {index}");
            TryShowNextPopup(); // skip and try next
            return;
        }

        OpenSystemData data = openSystemDatalist[index];

        popupObj.SetActive(true);
        popupIcon.gameObject.SetActive(true);
        popupIcon.sprite = data.icon;
        popupDesc.text = data.Desc;

        isPopupActive = true; // 코루틴 호출 전에 먼저 상태 지정
        SaveOpened(index);

        returnButton.gameObject.SetActive(false);
        DOVirtual.DelayedCall(1f, () =>
        {
            returnButton.gameObject.SetActive(true);
        });
    }

    private void OnClickReturnButton()
    {
        popupObj.SetActive(false);
        isPopupActive = false;

        TryShowNextPopup();
    }


    private bool IsAlreadyOpened(int index)
    {
        string key = $"{OpenSystemKeyPrefix}{index}";
        return PlayerPrefs.GetInt(key, 0) == 1;
    }

    private void SaveOpened(int index)
    {
        string key = $"{OpenSystemKeyPrefix}{index}";
        PlayerPrefs.SetInt(key, 1);
        PlayerPrefs.Save();
    }

    public void ResetAllOpenSystemHistory()
    {
        for (int i = 0; i < openSystemDatalist.Count; i++)
        {
            string key = $"{OpenSystemKeyPrefix}{i}";
            PlayerPrefs.DeleteKey(key);
        }

        popupQueue.Clear();
        popupIndexSet.Clear();
        popupObj.SetActive(false);
        isPopupActive = false;
    }
    public void ShowPopupMessage(string message)
    { 
        popupObj.SetActive(true);
        popupIcon.gameObject.SetActive(false);
        popupDesc.text = message;
    }
}

