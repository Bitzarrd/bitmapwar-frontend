using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class AnimatedSprites : MonoBehaviour
{
    public Action OnComplete;
    public SpriteRenderer sr;
    public List<Sprite> animList;
    public bool isLoop = false;

    public float timeElpase = 1f;

    public int currentFrame = 0;
    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = animList[currentFrame];
    }

    // Update is called once per frame
    void Update()
    {
        if (currentFrame >= animList.Count)
        {
            if (isLoop)
            {
                currentFrame %= animList.Count;
            }
            else
            {
                if (OnComplete != null)
                {
                    OnComplete.Invoke();
                }

                currentFrame = 0;
                gameObject.SetActive(false);
                return;
            }
        }
        
        if (timeElpase > 1.0f / 60f)
        {
            timeElpase = 0;
            sr.sprite = animList[currentFrame];
            currentFrame++;
        }

        timeElpase += Time.deltaTime;
    }
}
