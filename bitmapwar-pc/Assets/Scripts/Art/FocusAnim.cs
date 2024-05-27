using System.Collections;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class FocusAnim : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    public void DoFocus()
    {
        gameObject.SetActive(true);
        transform.localScale = Vector3.one * 2f;
        transform.DOScale(0.124f, 0.5f).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
