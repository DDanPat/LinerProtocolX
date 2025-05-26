using UnityEngine;
using UnityEngine.UI;
public class EnemyDamage : MonoBehaviour
{
    [SerializeField] private Canvas worldCanvas;
    [SerializeField] private Image hpImage;
    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
    }
    void LateUpdate()
    {
        worldCanvas.transform.LookAt(transform.position + mainCam.transform.rotation * Vector3.forward,
                         mainCam.transform.rotation * Vector3.up);
    }
    public void Init()
    {
        worldCanvas.gameObject.SetActive(false);
        hpImage.fillAmount = 1f;
        worldCanvas.gameObject.transform.LookAt(Camera.main.transform);
    }
    public void UpdateHP(float nowHp, float maxHp)
    {
        worldCanvas.gameObject.SetActive(true);
        hpImage.fillAmount = nowHp / maxHp;
        if (nowHp < 1)
            worldCanvas.gameObject.SetActive(false);
    }
}
