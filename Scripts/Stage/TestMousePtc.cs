using System.Collections;
using DG.Tweening;
using UnityEngine;

public class TestMousePtc : MonoBehaviour
{
    [SerializeField] private ExpDropEffect expDropEffectPrefab;
    [SerializeField] private ParticleSystem expParticle;

    TrailRenderer trail;

    private void Start()
    {
        trail = GetComponent<TrailRenderer>();
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Ŭ���� ���� Ray �߻�
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            Vector3 spawnPos;

            // ������ �¾����� �ű��, �� �¾����� z = 10 ���� ī�޶� ������ ����
            if (Physics.Raycast(ray, out hit))
            {
                spawnPos = hit.point;
            }
            else
            {
                spawnPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
            }

            trail.emitting = false;

            DOVirtual.DelayedCall(1f, () =>
            {
                Destroy(gameObject);
            });

        }

}
}