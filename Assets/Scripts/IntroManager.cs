using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroManager : MonoBehaviour
{
    public Image slideDisplay;
    public Button startButton;
    public Button backButton;
    public Button forwardButton;
    public Button exitButton;

    private List<Sprite> slides = new();
    private int currentSlideIndex = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Sprite[] loadedSprites = Resources.LoadAll<Sprite>("IntroSlides");

        if (loadedSprites != null && loadedSprites.Length > 0)
        {
            // Sort the sprites alphabetically by their name
            slides = loadedSprites.OrderBy(s => s.name).ToList();
        }
        else
        {
            Debug.LogError("No sprites found in Resources/IntroSlides");
            // Optionally, disable navigation buttons if no slides are found
            backButton.interactable = false;
            forwardButton.interactable = false;
        }

        startButton.onClick.AddListener(LoadMainScene);
        startButton.gameObject.SetActive(false);
        backButton.onClick.AddListener(ShowPreviousSlide);
        backButton.gameObject.SetActive(false);
        forwardButton.onClick.AddListener(ShowNextSlide);
        exitButton.onClick.AddListener(QuitApplication);

        UpdateSlideDisplay();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void LoadMainScene()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void ShowPreviousSlide()
    {
        if (currentSlideIndex > 0)
        {
            currentSlideIndex--;
            UpdateSlideDisplay();
        }
    }

    public void ShowNextSlide()
    {
        if (currentSlideIndex < slides.Count - 1)
        {
            currentSlideIndex++;
            UpdateSlideDisplay();
        }
    }

    public void QuitApplication()
    {
#if UNITY_EDITOR
        // If the game is running in the Unity Editor, stop play mode
        UnityEditor.EditorApplication.isPlaying = false;
        Debug.Log("Exiting Play Mode in Editor");
#else
        // If the game is a standalone build, quit the application
        Application.Quit();
        Debug.Log("Quitting Application");
#endif
    }

    private void UpdateSlideDisplay()
    {
        if (slides != null && slides.Count > 0 && slideDisplay != null)
        {
            slideDisplay.sprite = slides[currentSlideIndex];
        }

        // Update button interactability
        backButton.gameObject.SetActive(currentSlideIndex > 0);
        forwardButton.gameObject.SetActive(currentSlideIndex < slides.Count - 1);
        startButton.gameObject.SetActive(currentSlideIndex == slides.Count - 1);
    }
}
