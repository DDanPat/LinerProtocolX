using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScene : MonoBehaviour
{
    public AudioSource audio;
    [SerializeField] private Image loadingBar;
    private bool isLoading = false;
    void Start()
    {
        float bgm = PlayerPrefs.GetFloat("BGMVolume", 0.8f);
        audio.volume = bgm;
    }


    public void LoadMainScene()
    {
        if (!isLoading)
        {
            isLoading = true;
            StartCoroutine(LoadScene());
        }
    }
    
    private IEnumerator LoadScene()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync("MainScenes");
        op.allowSceneActivation = false;                 // 로딩 완료 후 자동 전환 막기
        
        float timer = 0f;        // 로딩 완료 시 Lerp 보간용 시간 변수

        while (!op.isDone)
        {
            yield return null;

            // 1) 로딩바 채우기
            if (op.progress < 0.9f)
            {
                loadingBar.fillAmount = op.progress; // 0~0.9까지 실제 진행도 반영
            }
            else
            {
                timer += Time.unscaledDeltaTime;
                loadingBar.fillAmount = Mathf.Lerp(0.9f, 1f, timer); // 마지막 10% 부드럽게 채우기

                if (loadingBar.fillAmount >= 1f)
                {
                    op.allowSceneActivation = true; // 씬 전환 허용
                    yield break;
                }
            }
        }
    }
}
