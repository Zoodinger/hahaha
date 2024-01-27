using UnityEngine;
using UnityEngine.SceneManagement;

namespace Hahaha
{
    public class pauseMenu : MonoBehaviour
    {
        public static bool GameIsPaused = false;
        public GameObject PauseMenuUI;
        
        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (GameIsPaused)
                {
                    Resume();
                }
                else
                {
                    Pause();
                }
            }
        }

        void Pause()
        {
            PauseMenuUI.SetActive(true);
            Time.timeScale = 0f;
            GameIsPaused = true;
        }
        
        public void Resume()
        {
            PauseMenuUI.SetActive(false);
            Time.timeScale = 1f;
            GameIsPaused = false;
        }

        public void QuitGame()
        {
            Application.Quit();
        }
        
        public void RestartGame()
        {
            Resume();
            SceneManager.LoadScene(2);
        }
    }
}
