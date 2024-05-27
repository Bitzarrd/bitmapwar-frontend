using System;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MoveAnimMgr : MonoBehaviour
{
    public static MoveAnimMgr inst;
    public SpriteRenderer animObj;
    private Queue<SpriteRenderer> moveAnimators = new();

    public SpriteRenderer atkObj;
    private Queue<SpriteRenderer> atkAnimators = new();

    public AnimatedSprites explosionObj;
    public Queue<AnimatedSprites> explosions = new();

    public AnimatedSprites lockSprite;
    public Queue<AnimatedSprites> lockSprites = new();

    public AnimatedSprites originFx;
    public Queue<AnimatedSprites> originFxs = new();

    public Dictionary<Vector2, AnimatedSprites> dicOriginFx = new();
    public Dictionary<Vector2, AnimatedSprites> dicLockFx = new();
    
    void Awake()
    {
        inst = this;
    }

    void Start()
    {
        for (int i = 0; i < 500; i++)
        {
            var a = Instantiate(animObj.gameObject);
            var sp = a.GetComponent<SpriteRenderer>();
            sp.color = new Color(1f, 1f, 1f, 0f);
            sp.transform.SetParent(transform);
            moveAnimators.Enqueue(sp);
        }

        if (atkObj != null)
        {
            for (int i = 0; i < 200; i++)
            {
                var a = Instantiate(atkObj.gameObject);
                var sp = a.GetComponent<SpriteRenderer>();
                sp.transform.SetParent(transform);
                atkAnimators.Enqueue(sp);
            }
        }

        if (explosionObj != null)
        {
            for (int i = 0; i < 50; i++)
            {
                var a = Instantiate(explosionObj.gameObject);
                var sp = a.GetComponent<AnimatedSprites>();
                sp.transform.SetParent(transform);
                explosions.Enqueue(sp);
            }
        }
        
        if (lockSprite != null)
        {
            for (int i = 0; i < 100; i++)
            {
                var a = Instantiate(lockSprite.gameObject);
                var sp = a.GetComponent<AnimatedSprites>();
                sp.transform.SetParent(transform);
                lockSprites.Enqueue(sp);
            }
        }
        
        if (originFx != null)
        {
            for (int i = 0; i < 100; i++)
            {
                var a = Instantiate(originFx.gameObject);
                var sp = a.GetComponent<AnimatedSprites>();
                sp.transform.SetParent(transform);
                originFxs.Enqueue(sp);
            }
        }
    }

    public void StartMove(Vector3 pos)
    {
        if (moveAnimators.Count == 0)
        {
            return;
        }
        var t = moveAnimators.Dequeue();
        if (t == null) return;
        t.transform.position = pos;
        t.color = new Color(1f, 1f, 1f, 1f);
        t.DOFade(0f, 2f).OnComplete(() =>
        {
            moveAnimators.Enqueue(t);
        });
    }

    public void PlayExplosion(int x, int y)
    {
        if (explosions.Count == 0)
        {
            return;
        }
        var tempVec = new Vector2Int(x, y);
        tempVec = GridUtils.Inst.GetCenterPos(tempVec);
        var pos = GridUtils.Inst.GetTileScenePosByCoords(tempVec.x, tempVec.y);
        
        var t = explosions.Dequeue();
        if (t == null) return;
        t.gameObject.SetActive(true);
        t.transform.position = pos;
        t.OnComplete = () =>
        {
            t.gameObject.SetActive(false);
            explosions.Enqueue(t);
        };
    }

    public void ClearLockFx()
    {
        foreach (var v in dicLockFx.Values)
        {
            DestroyImmediate(v.gameObject);
        }
        dicLockFx.Clear();
    }
    public void PlaceLockFx(Vector2Int _pos)
    {
        if (dicLockFx.ContainsKey(_pos))
        {
            return;
        }
        var tempVec = _pos;
        tempVec = GridUtils.Inst.GetCenterPos(tempVec);
        var pos = GridUtils.Inst.GetTileScenePosByCoords(tempVec.x, tempVec.y);

        AnimatedSprites t = null;
        
        if (lockSprites.Count == 0)
        {
            //t = Instantiate(lockSprite.gameObject).GetComponent<AnimatedSprites>();
            return;
        }
        else
        {
            t = lockSprites.Dequeue();
        }
        dicLockFx.Add(_pos, t);
        t.gameObject.SetActive(true);
        t.transform.position = pos;
    }
    public void PlaceOriginFx(Vector2Int _pos)
    {
        if (dicOriginFx.ContainsKey(_pos))
        {
            return;
        }

        var tempVec = _pos;
        tempVec = GridUtils.Inst.GetCenterPos(tempVec);
        var pos = GridUtils.Inst.GetTileScenePosByCoords(tempVec.x, tempVec.y);

        AnimatedSprites t = null;
        
        if (originFxs.Count == 0)
        {
            //t = Instantiate(originFx.gameObject).GetComponent<AnimatedSprites>();
            return;
        }
        else
        {
            t = originFxs.Dequeue();
        }
        dicOriginFx.Add(_pos, t);
        t.gameObject.SetActive(true);
        t.transform.position = pos;
    }

    public void PlayDefeat(int x, int y)
    {
        /*
        if (atkAnimators.Count == 0)
        {
            return;
        }
        var tempVec = new Vector2Int(x, y);
        tempVec = GridUtils.Inst.GetCenterPos(tempVec);
        var pos = GridUtils.Inst.GetTileScenePosByCoords(tempVec.x, tempVec.y);
        
        var t = atkAnimators.Dequeue();
        if (t == null) return;
        t.gameObject.SetActive(true);
        t.transform.position = pos;
        t.transform.localScale = Vector3.zero;
        t.transform.DOScale(2.5f, 1f).OnComplete(() =>
        {
            t.transform.localScale = Vector3.zero;
        }).SetLoops(-1);
        */
    }
}