using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Lean.Touch;
using UnityEngine;

public class ZoomAnim : Singleton<ZoomAnim>
{
    public LeanCameraZoom zoomer;

    public void LerpZoom()
    {
        if (zoomer == null)
        {
            zoomer = Camera.main.GetComponent<LeanCameraZoom>();
        }
        DOTween.To(() => zoomer.Zoom, value => zoomer.Zoom = value, 1.0f, 1);
    }
}
