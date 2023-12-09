using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder : MonoBehaviour
{
    private Controller Controller;

    void Start()
    {
        Controller = Controller.Instance;
    }

    public void Build()
    {
        Controller.BuildNetwork();
    }
}
