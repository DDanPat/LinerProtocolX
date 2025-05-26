using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class DescriptPopup : MonoBehaviour
{
    [SerializeField] private Button closeBtn;
    [SerializeField] Image skillImage;
    [SerializeField] TextMeshProUGUI descriptText;
    [SerializeField] GameObject content;
    UnityAction closeAction;
    void Start()
    {
        closeBtn.onClick.AddListener(Close);
        Close();
    }
    public void Open(Sprite sprite, string descript, UnityAction closeCallback = null)
    {
        content.SetActive(true);
        skillImage.sprite = sprite;
        descriptText.text = descript;
        closeAction = closeCallback;
    }
    void Close()
    {
        content.SetActive(false);
        closeAction?.Invoke();
    }
}
