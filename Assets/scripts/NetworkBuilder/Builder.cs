using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder : MonoBehaviour
{
    private Controller Controller;

    // Start is called before the first frame update
    void Start()
    {
        Controller = Controller.Instance;
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0)) Controller.BuildNetwork();
    }
}
