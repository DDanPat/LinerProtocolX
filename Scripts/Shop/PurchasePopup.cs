using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PurchasePopup : MonoBehaviour
{
    public static PurchasePopup Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI messageText;    // 팝업 메시지
    [SerializeField] private Button confirmButton;           // 확인 버튼
    [SerializeField] private Button cancelButton;            // 취소 버튼

    // 콜백 델리게이트
    private Action onConfirm;
    private Action onCancel;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        gameObject.SetActive(false);
    }

    private void Start()
    {
        confirmButton.onClick.AddListener(HandleConfirm);
        cancelButton.onClick.AddListener(HandleCancel);
    }

    /// <summary>
    /// 구매 여부 확인 팝업을 띄웁니다.
    /// </summary>
    /// <param name="currency">“골드” 같은 재화 이름</param>
    /// <param name="product">“체력 물약” 같은 상품 이름</param>
    /// <param name="cost">필요 재화량</param>
    /// <param name="currentBalance">플레이어가 보유한 재화량</param>
    /// <param name="confirmCallback">확인 시 실행할 콜백</param>
    /// <param name="cancelCallback">취소(또는 확인) 시 실행할 콜백</param>
    public void Show(
        string currency,
        string product,
        int cost,
        int currentBalance,
        Action confirmCallback,
        Action cancelCallback = null
    )
    {
        bool isEnough = currentBalance >= cost;

        if (isEnough)
        {
            if (currency == "광고")
            {
                messageText.text = $"광고를 시청해서\n{product}을 구매하시겠습니까?";
            }
            else
            {
                string currencyStr = "";
                if (currency == "현금")
                {
                    currencyStr = "$" + cost.ToString();
                }
                else
                {
                    currencyStr = $"{cost}{currency}";
                }

                messageText.text = $"{currencyStr}을(를) 사용해서\n{product}을 구매하시겠습니까?";
            }
            // 충분할 때: “~를 구매하시겠습니까?”
            
            confirmButton.gameObject.SetActive(true);
            confirmButton.interactable = true;
            onConfirm = confirmCallback;
            onCancel = cancelCallback;
        }
        else
        {
            // 부족할 때: “재화가 부족합니다”
            messageText.text = $"{currency}이(가) 부족합니다.\n필요: {cost}  보유: {currentBalance}";
            confirmButton.gameObject.SetActive(false);
            onConfirm = null;
            onCancel = cancelCallback;
        }

        gameObject.SetActive(true);
    }

    private void HandleConfirm()
    {
        onConfirm?.Invoke();
        Close();
    }

    private void HandleCancel()
    {
        onCancel?.Invoke();
        Close();
    }

    private void Close()
    {
        onConfirm = null;
        onCancel = null;
        gameObject.SetActive(false);
    }
}
