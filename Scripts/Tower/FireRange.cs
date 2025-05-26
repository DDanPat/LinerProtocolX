using UnityEngine;

public class FireRange : MonoBehaviour
{
    [SerializeField] private Tower tower;
    public float defaultRange = 2f;
    public GameObject fireEffect;

    private float currentRange;

    public float CurrentRange
    {
        get => currentRange;
        private set
        {
            if (Mathf.Approximately(currentRange, value)) return;
            currentRange = value;
            SetFireRange();
        }
    }

    private void Update()
    {
        if (tower == null) return;

        float newRange = tower.GetRange();
        if (!Mathf.Approximately(CurrentRange, newRange))
        {
            CurrentRange = newRange;
        }
    }

    private void SetFireRange()
    {
        if (fireEffect != null)
        {
            float scale = currentRange / defaultRange;
            fireEffect.transform.localScale = new Vector3(scale, scale, scale);
        }
    }
}