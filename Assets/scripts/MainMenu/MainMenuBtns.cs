using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuBtns : MonoBehaviour
{
    public void LoadBuilder()
    {
        SceneManager.LoadScene("NetworkBuilder");
    }

    public void LoadPCA()
    {
        SceneManager.LoadScene("LoadDataScene");
    }

    public void LoadTester()
    {
        SceneManager.LoadScene("MNIST_MLP");
    }

    public void Exit()
    {
        Application.Quit();
    }
}
