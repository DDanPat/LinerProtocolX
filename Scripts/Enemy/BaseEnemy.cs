using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class BaseEnemy : MonoBehaviour
{
    [SerializeField] private Transform target;
    private Vector3 _lastTargetPos;
    NavMeshAgent _agent;
    // Start is called before the first frame update
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null || _agent == null) return;

        if (_lastTargetPos != target.position)
        {
            _agent.SetDestination(target.position);
            _lastTargetPos = target.position;
        }
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }
}
