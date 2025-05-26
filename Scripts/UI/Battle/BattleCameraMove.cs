using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleCameraMove : MonoBehaviour
{
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private Camera targetCamera;

    public float moveSpeed = 5f;
    private bool moving;
    private bool isRight;
    private float minX; // 최대 왼쪽
    private float maxX; // 최대 오른쪽

    void Start()
    {
        leftButton.GetComponent<CameraMoveButton>().RegistController(this);
        rightButton.GetComponent<CameraMoveButton>().RegistController(this);
    }
    void Update()
    {
        if (moving)
        {
            if (isRight)
            {
                // 범위까지만 이동
                float moveLength = moveSpeed * Time.deltaTime;
                if (maxX > targetCamera.transform.position.x + moveLength)
                {
                    Vector3 move = Vector3.right * moveLength;
                    targetCamera.transform.position += move;
                }

            }
            else
            {
                // 범위까지만 이동
                float moveLength = moveSpeed * Time.deltaTime;
                if (minX < targetCamera.transform.position.x - moveLength)
                {
                    Vector3 move = Vector3.left * moveLength;
                    targetCamera.transform.position += move;
                }
            }
        }
    }
    public void SetStage(StageInfoTable stage)
    {
        int margin = stage.width - 22;
        if (margin <= 0)
        {
            gameObject.SetActive(false);
            minX = 0f;
            maxX = 0f;
            return;
        }
        float stepSize = 0.5f;
        float size = margin * stepSize * 0.5f;
        minX = -size;
        maxX = size;

        // 시작시 카메라가 기지를 중심으로 잡도록
        int hq = stage.playerBase[0];
        float pos = (hq - stage.width / 2) * 0.5f;
        if (pos > 0)
        {
            Vector3 origin = targetCamera.transform.position;
            targetCamera.transform.position = new Vector3(maxX, origin.y, origin.z);
        }
        else
        {
            Vector3 origin = targetCamera.transform.position;
            targetCamera.transform.position = new Vector3(minX, origin.y, origin.z);
        }
    }
    public void TryMove(bool isRight)
    {
        this.moving = true;
        this.isRight = isRight;
    }
    public void Stop()
    {
        moving = false;
    }
}
