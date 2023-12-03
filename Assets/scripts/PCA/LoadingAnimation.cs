using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingAnimation : MonoBehaviour
{
    public RectTransform RectTransform;
    public float Speed = 600f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RectTransform.Rotate(0, 0, -Speed * Time.deltaTime);
    }
}
