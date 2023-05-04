using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class InputManager : MonoBehaviour
{
    private Controller Controller;

    void Start()
    {
        Controller = Controller.Instance;    
    }

    private void OnMouseOver()
    {
        if(Input.GetMouseButtonDown(0)) Controller.AddInput();
        if (Input.GetMouseButtonDown(1)) Controller.RemoveInput();
    }
}
