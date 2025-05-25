using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroManager : MonoBehaviour
{
    public GameObject introSlideShower;
    public Button startButton;
    public Button backButton;
    public Button forwardButton;
    public Button exitButton;

    private List<Sprite> _slides = new();
    private int _currentSlideIndex = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Sprite[] loadedSprites = Resources.LoadAll<Sprite>("IntroSlides");

        if (loadedSprites != null && loadedSprites.Length > 0)
        {
            // Sort the sprites alphabetically by their name
            _slides = loadedSprites.OrderBy(s => s.name).ToList();
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

        UpdateSlideDisplay(true);
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
        if (_currentSlideIndex > 0)
        {
            _currentSlideIndex--;
            UpdateSlideDisplay();
        }
    }

    public void ShowNextSlide()
    {
        if (_currentSlideIndex < _slides.Count - 1)
        {
            _currentSlideIndex++;
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

    private void UpdateSlideDisplay(bool firstCall = false)
    {
        SpriteRenderer painter = introSlideShower.GetComponent<SpriteRenderer>();

        if (_slides != null && _slides.Count > 0 && introSlideShower != null)
        {
            painter.sprite = _slides[_currentSlideIndex];

            if (firstCall)
            {
                DisplayFullScreenCentered centerer = introSlideShower.GetComponent<DisplayFullScreenCentered>();

                introSlideShower.transform.localScale = Vector3.one;

                centerer.PositionSpriteFullScreenCentered();
            }
        }

        // Update button interactability
        backButton.gameObject.SetActive(_currentSlideIndex > 0);
        forwardButton.gameObject.SetActive(_currentSlideIndex < _slides.Count - 1);
        startButton.gameObject.SetActive(_currentSlideIndex == _slides.Count - 1 || _slides.Count == 0);
    }
}
