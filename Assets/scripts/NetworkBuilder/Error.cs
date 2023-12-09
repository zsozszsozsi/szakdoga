using System.Collections;
using TMPro;
using UnityEngine;

public class Error : MonoBehaviour
{
    private float time = 3f;

    private void Start()
    {
        StartCoroutine( FadeOut(time, gameObject.GetComponent<TextMeshProUGUI>()) );
    }

    private void Update()
    {
        
    }

    public IEnumerator FadeOut(float t, TextMeshProUGUI i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
        while (i.color.a > 0.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
            yield return null;
        }

        Destroy(gameObject);
    }
}
