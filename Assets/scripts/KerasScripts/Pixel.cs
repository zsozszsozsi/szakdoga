using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;



public class Pixel : MonoBehaviour, IPointerEnterHandler
{
    public RawImage Image;

    public void OnPointerEnter (PointerEventData eventData)
    {
        print("asdasd");
        
        
        if (Input.GetMouseButton(0))
        {
            Image.color = Color.white;

        }
        else if (Input.GetMouseButton(1))
        {
            Image.color = Color.black;
        }
        

        
    }
}
