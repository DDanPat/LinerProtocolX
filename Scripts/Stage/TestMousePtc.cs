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
            // 클릭한 곳에 Ray 발사
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            Vector3 spawnPos;

            // 뭔가에 맞았으면 거기로, 안 맞았으면 z = 10 기준 카메라 앞으로 생성
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