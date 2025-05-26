using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GachaSlots : MonoBehaviour, IPoolable
{
    public Image itmeIcon;
    public Image back;
    public Image transPiece;
    public GameObject outIine;
    public GameObject rotateObj;

    public Button button;

    public int itemKey;

    public bool isRotated = false;
    public bool isOPOwned = false;

    bool isOper = false;

    CategoryEntryTable categoryEntryTable;
    private Action<GameObject> returnToPool;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(Rotate);
    }

    public void SetData(CategoryEntryTable data)
    {
        isOper = false;
        outIine.SetActive(false);
        categoryEntryTable = data;
        itemKey = data.item;
        SetItemIcon();
    }

    private void SetItemIcon()
    {
        var inventory = GameManager.Instance.Player.inventory;
        if (inventory.operators.ContainsKey(itemKey) &&
            inventory.operators[itemKey].data.profile != null)
        {
            itmeIcon.rectTransform.localScale = new Vector3(1, 1, 1);
            itmeIcon.sprite = inventory.operators[itemKey].data.profile;
            isOper = true;
            outIine.SetActive(true);
        }
        else if (inventory.items.ContainsKey(itemKey) &&
            inventory.items[itemKey].data.icon != null)
        {
            itmeIcon.rectTransform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
            itmeIcon.sprite = inventory.items[itemKey].data.icon;
        }
        else
        {
            itmeIcon.sprite = null; // 아이콘 없을 경우 초기화
        }
    }

    public void OperEffect()
    {
        if (isOper && !isOPOwned)
        {
            GachaEffectHandler.Instance
                .RequestGachaEffect(GameManager.Instance.DataManager.OperatorTableLoader.ItemsDict[itemKey]);
        }
    }

    public void Rotate()
    {
        OperEffect();
        outIine.SetActive(false);
        isRotated = true;
        button.enabled = false;
        var seq = DOTween.Sequence();
        seq.Append(this.transform.DORotate(this.transform.eulerAngles + new Vector3(0, 90, 0),
            0.4f)).SetEase(Ease.OutCubic);


        back.enabled = false;
        seq.Append(this.transform.DORotate(this.transform.eulerAngles + new Vector3(0, 360, 0),
            0.4f)).SetEase(Ease.OutCubic);
        seq.OnComplete(() =>
        {
            outIine.SetActive(isOper);
            if (isOPOwned) transPiece.gameObject.SetActive(true);
            CraftPanelUI.Instance.CheckGachaSlotsRotation(); // 회전 완료 후 상태 검사
        });
        
    }

    public void Initialize(Action<GameObject> returnAction)
    {
        returnToPool = returnAction;

    }

    public void OnSpawn()
    {
        if (button == null)
            button = GetComponent<Button>();

        isRotated = false;
        isOPOwned = false;

        transform.rotation = Quaternion.identity;

        if (button != null)
            button.enabled = true;

        if (back != null)
            back.enabled = true;

        if (transPiece != null)
            transPiece.gameObject.SetActive(false);

        if (itmeIcon != null)
            itmeIcon.sprite = null;

        itemKey = -1;
        categoryEntryTable = null;
    }

    public void OnDespawn()
    {
        returnToPool?.Invoke(gameObject);
    }
}
