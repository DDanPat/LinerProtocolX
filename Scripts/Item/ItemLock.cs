using UnityEngine;
using UnityEngine.UI;

public class ItemLock : MonoBehaviour
{
    [SerializeField] private Image lockImage;
    public void Lock(bool isOwned)
    {
        lockImage.gameObject.SetActive(!isOwned);
    }
}
