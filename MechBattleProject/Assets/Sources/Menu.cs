using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public sealed class Menu : MonoBehaviour {
    [SerializeField]
    private string[] levelNames = new string[] {
        "EasyLevel",
        "NormalLevel",
        "HardLevel"
    };
    [SerializeField]
    private Button exitButton;

    private void Start() {
        if (exitButton != null) {
            var needShowExitButton = Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.WindowsPlayer;
            exitButton.gameObject.SetActive(needShowExitButton);
        }
    }

    public void StartLevelClick(int id) {
        SceneManager.LoadScene(levelNames[id]);
    }

    public void ExitGameButtonClick() {
        Application.Quit();
    }
}
