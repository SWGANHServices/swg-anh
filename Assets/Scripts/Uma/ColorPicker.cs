using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ColorPicker : MonoBehaviour, IPointerClickHandler
{
    public Color pickedColor;

    [Serializable]
#pragma warning disable CA1034 // Nested types should not be visible
    public class ColorEvent : UnityEvent<Color> { }

#pragma warning restore CA1034 // Nested types should not be visible

    public ColorEvent OnColorPicked = new ColorEvent();

    public void OnPointerClick(PointerEventData eventData)
    {
        pickedColor = GetColor(GetPointerPosition());
        OnColorPicked.Invoke(pickedColor);
    }

    private Color GetColor(Vector2 pos)
    {
        Texture2D texture = GetComponent<Image>().sprite.texture;
        Color selectedColor = texture.GetPixelBilinear(pos.x, pos.y);
        selectedColor.a = 1; // force full alpha
        return selectedColor;
    }

    private Vector2 GetPointerPosition()
    {
        Vector3[] imageCorners = new Vector3[4];
        gameObject.GetComponent<RectTransform>().GetWorldCorners(imageCorners);
        float textureWidth = imageCorners[2].x - imageCorners[0].x;
        float textureHeight = imageCorners[2].y - imageCorners[0].y;
        float uvXPos = (Input.mousePosition.x - imageCorners[0].x) / textureWidth;
        float uvYPos = (Input.mousePosition.y - imageCorners[0].y) / textureHeight;
        return new Vector2(uvXPos, uvYPos);
    }
}