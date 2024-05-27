using System.Collections;
using System.Collections.Generic;
using Lean.Touch;
using UnityEngine;

public class TapMgr : MonoBehaviour
{
    public MapMesh mapMesh;
    
    void OnEnable()
    {
        LeanTouch.OnFingerTap += HandleFingerTap;
    }

    void OnDisable()
    {
        LeanTouch.OnFingerTap -= HandleFingerTap;
    }

    void HandleFingerTap(LeanFinger finger)
    {
        if (finger.IsOverGui)
        {
            return;
        }

        var scenePos = Camera.main.ScreenToWorldPoint(finger.ScreenPosition);
        scenePos.z = 0;

        var clickPos = GridUtils.Inst.GetCoordsByPos(scenePos);
        
        var logicPos = GridUtils.Inst.GetLogicCoords(clickPos);
        //Debug.Log("LogiPos: " + logicPos);
        if (logicPos.x < 0 || logicPos.y < 0)
        {
            return;
        }

        if (logicPos.x >= Game.Inst.mapWidth)
        {
            return;
        }

        if (logicPos.y >= Game.Inst.mapHeight)
        {
            return;
        }
        //mapMesh.UpdateUV(logicPos.x, logicPos.y, 2);

        Game.Inst.currentSelect = logicPos;
        mapMesh.SetCursor(clickPos);
        
        MainPage.inst.RefreshTileInfo();

        /*
        var mapId = logicPos.y * GridUtils.Inst.gridSizeX + logicPos.x;
        ProtoUtils.SendPlaceSoldier(mapId, 1, Consts.colors[0]);
        */
    }
}
