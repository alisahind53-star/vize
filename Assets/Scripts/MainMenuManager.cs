using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Sahne Ayarları")]
    [Tooltip("Build Settings'e eklediğiniz ana oyun sahnesinin tam adı")]
    [SerializeField] private string gameSceneName = "GameScene";

    private void Start()
    {
        // Ana menüde zamanın aktığından ve imlecin serbest olduğundan emin ol
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // "Başla" butonunun OnClick() olayına bağlanacak
    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    // "Çık" butonunun OnClick() olayına bağlanacak
    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}