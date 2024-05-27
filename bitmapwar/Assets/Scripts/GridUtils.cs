using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;

public class GridUtils : Singleton<GridUtils>
{
    public int gridSizeX = 1000;
    public int gridSizeY = 824;
    public float cellSize = 0.1f;
    public float cellGap = 0.005f;

    public bool isTileInMap(int x, int y)
    {
        if (x * gridSizeX + y >= gridSizeX * gridSizeY)
        {
            return false;
        }

        return true;
    }

    public bool IsCoordValid(int x, int y)
    {
        if (x < 0 || x >= gridSizeX)
        {
            return false;
        }
        if (y < 0 || y >= gridSizeY)
        {
            return false;
        }

        return true;
    }

    public bool isTileIdValid(int tileId)
    {
        if (tileId < 0)
        {
            return false;
        }
        if (tileId > gridSizeX * gridSizeY)
        {
            return false;
        }

        return true;
    }
    
    public Vector2Int GetCoordsByPos(Vector3 pos)
    {
        var res = new Vector2Int();
        res.x = Mathf.RoundToInt(pos.x / (cellGap + cellSize));
        res.y = Mathf.RoundToInt(-pos.y / (cellGap + cellSize));

        return res;
    }

    public Vector3 GetTileScenePosByCoords(Vector2Int pos)
    {
        var offset = new Vector3((pos.x ) * (cellSize + cellGap), -(pos.y) * (cellSize + cellGap), 0);

        return offset;
    }

    public Vector3 GetTileScenePosByCoords(int x, int y)
    {
        var offset = new Vector3((x) * (cellSize + cellGap), -(y) * (cellSize + cellGap), 0);

        return offset;
    }

    public Vector2Int GetLogicCoords(Vector2Int pos)
    {
        var logiRes = pos + new Vector2Int(gridSizeX / 2, gridSizeY / 2);
        return logiRes;
    }

    public Vector2Int GetCenterPos(Vector2Int pos)
    {
        return new Vector2Int(pos.x - gridSizeX / 2, pos.y - gridSizeY / 2);
    }

    public int GetTileIDByPos(int x, int y)
    {
        return y * gridSizeX + x;
    }

    public string GetRealTimeStamp(int timeStamp)
    {
        DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(timeStamp);

        DateTime localTime = dateTimeOffset.LocalDateTime;

        return localTime.ToString();
    }

    public string GetRealTimeHoursAndMinuts(int timeStamp)
    {
        DateTime dateTime = DateTimeOffset.FromUnixTimeSeconds(timeStamp).LocalDateTime;

        string res = dateTime.ToString("HH:mm:ss");

        return res;
    }

    public string GetRealTimeDateAndTime(int timeStamp)
    {
        DateTime dateTime = DateTimeOffset.FromUnixTimeSeconds(timeStamp).LocalDateTime;
        string res = dateTime.ToString("MM/dd HH:mm:ss");

        return res;
    }

    public Vector2Int GetCoordByTileId(int tile)
    {
        var pos = new Vector2Int();
        pos.x = tile % gridSizeX;
        pos.y = tile / gridSizeX;

        return pos;
    }
}
