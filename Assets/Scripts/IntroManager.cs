using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroManager : MonoBehaviour
{
    public Button startButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startButton.onClick.AddListener(LoadMainScene);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void LoadMainScene()
    {
        SceneManager.LoadScene("MainScene");
    }
}
