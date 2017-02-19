using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class PortraitClick : MonoBehaviour, IPointerClickHandler
{

    static Color[] colors =
    {
        Color.white,
        new Color(1f, 0.434f, 0.434f),
        new Color(0.455f, 1f, 0.706f)
    };

    Image image;
    int colorNum = 0;

    void Start()
    {
        image = GetComponent<Image>();
        UpdateColor();

        //for (int i = 0; i < colors.Length; i++)
        //{
        //    print(colors[i]);
        //}
    }


    public void ToggleColor()
    {
        colorNum++;
        UpdateColor();
    }

    void UpdateColor()
    {
        image.color = colors[colorNum % 3];
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Middle)
        {
            ToggleColor();
        }
    }
}