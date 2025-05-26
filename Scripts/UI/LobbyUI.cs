using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyUI : MonoBehaviour
{

    public void LoadOrganizeScene()
    {
        SceneManager.LoadScene("PlayerUITest");
    }
}
