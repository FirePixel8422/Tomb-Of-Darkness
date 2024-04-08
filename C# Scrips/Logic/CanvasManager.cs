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
    }
}
