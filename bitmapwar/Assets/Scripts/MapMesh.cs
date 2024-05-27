using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class MapMesh : MonoBehaviour
{
    public int gridSizeX;
    public int gridSizeY;

    public float cellSize;
    public float cellGap;

    private MeshFilter _mf;

    private Vector2[] _uvs;

    public bool isGenerated = false;

    public static MapMesh inst;
    public FocusAnim focusIndicator;

    private void Awake()
    {
        inst = this;
    }

    public List<Vector2> uvDic = new List<Vector2>()
    {
        new Vector2(0, 0),
        new Vector2(0, 1),
        new Vector2(0.2f, 1),
        new Vector2(0.2f, 0),

        new Vector2(0.2f, 0),
        new Vector2(0.2f, 1),
        new Vector2(0.4f, 1),
        new Vector2(0.4f, 0),

        new Vector2(0.4f, 0),
        new Vector2(0.4f, 1),
        new Vector2(0.6f, 1),
        new Vector2(0.6f, 0),

        new Vector2(0.6f, 0),
        new Vector2(0.6f, 1),
        new Vector2(0.8f, 1),
        new Vector2(0.8f, 0),

        new Vector2(0.8f, 0),
        new Vector2(0.8f, 1),
        new Vector2(1f, 1),
        new Vector2(1f, 0),
    };

    public GameObject cursor;

    private Mesh _gridMesh;
    
    public Vector3[] GetVertex(Vector3 offset)
    {
        Vector3[] vertices = new Vector3[4]
        {
            new Vector3(-0.5f * cellSize, -0.5f * cellSize, 0) + offset,
            new Vector3(0.5f * cellSize, -0.5f * cellSize, 0) + offset,
            new Vector3(-0.5f * cellSize, cellSize * 0.5f, 0) + offset,
            new Vector3(cellSize * 0.5f, cellSize * 0.5f, 0) + offset
        };

        return vertices;
    }

    public void UpdateUV(int x, int y, int uvIndex)
    {
//        Debug.Log($"Updating UV:{x}, {y}");
        var mesh = _mf.mesh;
        var uvs = mesh.uv;
        var baseIndex = (y * gridSizeX + x) * 4;
        var baseUvIndex = uvIndex * 4;
        uvs[baseIndex] = uvDic[baseUvIndex];
        uvs[baseIndex + 1] = uvDic[baseUvIndex + 1];
        uvs[baseIndex + 2] = uvDic[baseUvIndex + 2];
        uvs[baseIndex + 3] = uvDic[baseUvIndex + 3];

        mesh.uv = uvs;
    }

    public void ClearAll()
    {
        //var uvs = Lock();
        for (int y = 0; y < gridSizeY; y++)
        {
            for (int x = 0; x < gridSizeX; x++)
            {
                SetUV(_uvs, x, y, 0);
            }
        }

        Unlock(_uvs);
    }

    public void Unlock(Vector2[] uvs)
    {
        var mesh = _mf.mesh;

        mesh.uv = uvs;
    }

    public Vector2[] Lock()
    {
        //var mesh = _mf.mesh;
        //return mesh.uv;
        return null;
    }

    public void SetUV(Vector2[] uvs, int x, int y, int uvIndex)
    {
        if (y * gridSizeX + x >= gridSizeX * gridSizeY)
        {
            //越界
            return;
        }

        var baseIndex = (y * gridSizeX + x) * 4;
        var baseUvIndex = uvIndex * 4;
        if (baseUvIndex > uvDic.Count)
        {
            Debug.Log("Uv Index is :" + uvIndex);
        }
        uvs[baseIndex] = uvDic[baseUvIndex];
        uvs[baseIndex + 1] = uvDic[baseUvIndex + 1];
        uvs[baseIndex + 2] = uvDic[baseUvIndex + 2];
        uvs[baseIndex + 3] = uvDic[baseUvIndex + 3];
    }

    private Vector2Int tempVec = new Vector2Int();


    private void Update()
    {
        if (_mf == null)
        {
            return;
        }
        
        if(Game.Inst.updateCell.Count > 900000)
        {
            Game.Inst.updateCell.Clear();
            WebSocketClient.inst.Close();
            return;
        }

        int max = 5000;
        if (Game.Inst.updateCell.Count > 0)
        {
            int count = 0;
            if(Game.Inst.updateCell.Count > max)
            {
                Debug.Log("Err: update Cell is Larger now:" + Game.Inst.updateCell.Count);
                count = max;
            }
            else
            {
                count = Game.Inst.updateCell.Count;
            }
            //var uvs = Lock();
            for (int i = 0; i < count; i++)
            {
                var c = Game.Inst.updateCell[i];
                var index = Consts.colorDic[c.color];
                if (GridUtils.Inst.IsCoordValid(c.x, c.y))
                {
                    SetUV(_uvs, c.x, c.y, index);
                    tempVec.x = c.x;
                    tempVec.y = c.y;

                    tempVec = GridUtils.Inst.GetCenterPos(tempVec);
                    var pos = GridUtils.Inst.GetTileScenePosByCoords(tempVec.x, tempVec.y);
                    MoveAnimMgr.inst.StartMove(pos);
                }
                else
                {
                    Debug.Log("Paint Out of Map x:" + c.x + " Y: " + c.y);
                }
            }
            //Game.Inst.updateCell.Clear();
            Unlock(_uvs);
            Game.Inst.updateCell.RemoveRange(0, count);
        }
    }

    public void SetCursor(Vector2Int pos)
    {
        //Debug.Log("Set Cursor" + pos);
        cursor.SetActive(true);
        var offset = new Vector3((pos.x) * (cellSize + cellGap), -(pos.y) * (cellSize + cellGap), 0);
        cursor.transform.position = offset;

        focusIndicator.transform.position = offset;
    }

    public void SetFocus()
    {
        focusIndicator.DoFocus();
    }

    public int[] GetTriangles(int offset)
    {
        int[] tris = new int[6]
        {
            // lower left triangle
            0 + offset, 2 + offset, 1 + offset,
            // upper right triangle
            2 + offset, 3 + offset, 1 + offset
        };

        return tris;
    }

    public void Generate()
    {
        if (isGenerated)
        {
            return;
        }
        // Create a new Mesh object
        if (_gridMesh != null)
        {
            DestroyImmediate(_gridMesh);
        }
        Mesh gridMesh = new Mesh();
        gridMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        _gridMesh = gridMesh;

        // Calculate the total number of vertices and triangles
        int numVertices = gridSizeX * gridSizeY * 4;
        int numTriangles = gridSizeX * gridSizeY * 2 * 3;

        // Create arrays to hold the vertices, UVs, and triangles
        List<Vector3> vertices = new();
        Vector2[] uv = new Vector2[numVertices];
        List<int> triangles = new();


        Vector3 offset = new Vector3();
        // Generate the vertices and UVs
        
        for (int y = 0, i = 0; y < gridSizeY; y++)
        {
            for (int x = 0; x < gridSizeX; x++, i+=4)
            {
                offset = new Vector3((x - gridSizeX / 2) * (cellSize + cellGap), (gridSizeY / 2 - y) * (cellSize + cellGap), 0);
                var verts = GetVertex(offset);
                var trians = GetTriangles(i);
                
                vertices.AddRange(verts);
                triangles.AddRange(trians);
                    
                uv[i] = new Vector2(0, 0);
                uv[i + 1] = new Vector2(0, 1);
                uv[i + 2] = new Vector2(0.2f, 1);
                uv[i + 3]  = new Vector2(0.2f, 0);
                
            }
        }

        // Assign the arrays to the Mesh object
        gridMesh.vertices = vertices.ToArray();
        gridMesh.uv = uv;
        gridMesh.triangles = triangles.ToArray();

        // Attach the Mesh object to the GameObject
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = gridMesh;
        _mf = meshFilter;
        isGenerated = true;
        _uvs = gridMesh.uv;
    }

    private void Start()
    {
        //Application.runInBackground = true;
        gridSizeX = GridUtils.Inst.gridSizeX;
        gridSizeY = GridUtils.Inst.gridSizeY;

        cellSize = GridUtils.Inst.cellSize;
        cellGap = GridUtils.Inst.cellGap;
        
        //Generate();
    }
}
