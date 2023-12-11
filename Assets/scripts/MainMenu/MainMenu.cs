using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public static MainMenu Instance;

    public GameObject PopUp;

    private GameObject _popUp = null;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
    }

    void OnGUI()
    {
        if (Event.current.Equals(Event.KeyboardEvent("[esc]")))
        {

            if(SceneManager.GetActiveScene().name != "MainMenu")
            {
                if (!_popUp)
                {
                    var canvas = GameObject.FindGameObjectsWithTag("Canvas");
                    _popUp = Instantiate(PopUp, canvas[0].transform);
                }
                else
                {
                    Destroy(_popUp);
                    _popUp = null;
                }
            }
        }
    }
}
