using System.Collections;
using System.Collections.Generic;
using Lean.Touch;
using Unity.VisualScripting;
using UnityEngine;

public class CameraDrag : LeanDragCamera
{
    public static CameraDrag inst;

    public int camX;
    public int camY;
    
    protected override void Awake()
    {
        inst = this;
    }

    protected override void LateUpdate()
    {
        //TODO: Jerry is On UGUI
        /*if (Stage.isTouchOnUI)
        {
            return;
        }
        */
        base.LateUpdate();

        var pos = GridUtils.Inst.GetCoordsByPos(transform.position);
        TileNumMgr.inst.ShowTileNum(pos.x, pos.y);
        
    }
}
