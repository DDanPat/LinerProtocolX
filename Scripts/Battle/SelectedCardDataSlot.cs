using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Image;

public class SelectedCardDataSlot : MonoBehaviour
{
    public Image towerIcon;
    public Image operIcon;
    public TextMeshProUGUI cardDesc;
    public TextMeshProUGUI amount;

    public GameObject operIconObj;

    public void SetSelectedCardDataSlot(SelectedCardDataValue data)
    {
        towerIcon.sprite = data.towerOperData.towerData.profile;

        if (data.towerOperData.operatorData != null)
        {
            operIconObj.SetActive(true);
            operIcon.sprite = data.towerOperData.operatorData.profile;
        }
        else
        {
            operIconObj.SetActive(false);
            operIcon.enabled = false;
        }
        //string desc = data.cardData.desc.Replace("\\n", "");

        string desc = data.cardData.desc;
        int index = desc.IndexOf("n");

        if (index >= 0)
        {
            desc = data.cardData.desc.Substring(index + 1);
        }

        string towerName = data.towerOperData.towerData.towerInfoTable.name;


        cardDesc.text = $"[{towerName}]\n{desc}";

        amount.text = data.amount.ToString();
    }
}
