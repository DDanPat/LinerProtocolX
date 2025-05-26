using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BaseUnit : MonoBehaviour
{
    public int shape = 40;
    protected int key;
    public int operatorKey = -1; // 오퍼레이터 key;
    public int grade = 1; // 타워 레벨
    public float Range { get; private set; } = 2; // 사거리

    public bool isBuild;        // 미리보기 프리팹 만들지 bool로 할지?

    [HideInInspector]public List<Renderer> renderers;

    public virtual int GetTowerKey() => key;
    public virtual float GetRange() => Range;
    public Vector2Int start;
    public Vector2Int end;

    protected void Awake()
    {
        renderers = gameObject.GetComponentsInChildren<Renderer>().ToList();
    }
    // 등급 증가
    public void UpGrade()
    {
        if (grade < 5)
        {
            grade++;
        }
    }
    public bool CanUpgrade()
    {
        return grade <= 4;
    }
    public virtual void Remove()
    {
        // 타일 정보 삭제
        TileManager.Instance.ClearTile(start, end);
        // 오브젝트 삭제
        Destroy(gameObject);
    }

    public virtual void SetGradeColor()
    {
        
    }

}
