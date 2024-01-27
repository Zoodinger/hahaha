using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Hahaha
{
    public class MainMenu : MonoBehaviour
    {
        public void PlayGame()
        {
            //loads scene 1 - main level
            SceneManager.LoadSceneAsync(1);
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
