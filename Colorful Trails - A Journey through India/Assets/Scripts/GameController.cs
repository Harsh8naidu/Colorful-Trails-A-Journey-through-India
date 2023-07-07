using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void BackToPreviousScene()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;

        SceneManager.LoadScene(currentScene - 1);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void SwitchToCityLevels(string cityName)
    {
        // Load the scene for the selected city's levels
        string sceneName = cityName;
        SceneManager.LoadScene(sceneName);
    }


    public void SwitchtoLevel(string levelNumber)
    {
        string sceneName = levelNumber;
        SceneManager.LoadScene(sceneName);
    }
}
