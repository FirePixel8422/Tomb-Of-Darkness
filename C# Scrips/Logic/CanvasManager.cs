using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public static Canvas canvas;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
    }

    public static void AddUIToCanvas(Transform addedUI)
    {
        addedUI.SetParent(canvas.transform, true);
        addedUI.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        addedUI.transform.rotation = Quaternion.identity;
        foreach (Transform t in addedUI.GetComponentsInChildren<Transform>())
        {
            t.localScale = Vector3.one;
        }
    }
}
