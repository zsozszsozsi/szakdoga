using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ErrorManager : MonoBehaviour
{
    public static ErrorManager Instance;

    public GameObject ErrorText;


    public void Awake()
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

    public void AddError(string error)
    {
        ErrorText.GetComponent<TextMeshProUGUI>().text = error;
        Instantiate(ErrorText, transform); 
    }

}
