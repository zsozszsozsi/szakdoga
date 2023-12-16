using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Pixel : MonoBehaviour, IPointerEnterHandler
{
    public RawImage Image;

    private int row;
    private int col;
    private List<(int, int)> neighbours;
    private Color color;

    private void Start()
    {
        row = int.Parse( gameObject.name.Split(";")[0] );
        col = int.Parse( gameObject.name.Split(";")[1] );
        neighbours = Drawing.Instance.GetNeihgbours(row, col);

    }


    public void OnPointerEnter (PointerEventData eventData)
    {        
        
        if (Input.GetMouseButton(0))
        {
            Image.color = Color.white;
            foreach(var neighbour in neighbours)
            {
                Drawing.Instance.pixels[Drawing.CoordToIndex(neighbour)].color = Color.white;
            }

        }
        else if (Input.GetMouseButton(1))
        {
            Image.color = Color.black;
            foreach (var neighbour in neighbours)
            {
                Drawing.Instance.pixels[Drawing.CoordToIndex(neighbour)].color = Color.black;
            }
        }
        

        
    }
}
