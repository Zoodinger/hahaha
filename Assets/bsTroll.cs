using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Hahaha
{
    public class bsTroll : MonoBehaviour
    {
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SceneManager.LoadScene(2);
            }
        }
    }
}
