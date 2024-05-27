using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GIFPlayer : MonoBehaviour
{

    public SpriteRenderer spRender;
    public TextAsset gifdata;

    float mTime = 0f;

    public static GIFPlayer inst;
    public List<Sprite> Frames = new List<Sprite>();
    private List<float> mFrameDelay = new List<float>();
    private int mCurFrame = 0;

    private void Awake()
    {
        inst = this;
    }

    public void Play()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        byte[] data = gifdata.bytes;

        using (var decoder = new MG.GIF.Decoder(data))
        {
            var img = decoder.NextImage();

            while (img != null)
            {
                Texture2D tex = img.CreateTexture();
                Frames.Add(Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), 0.5f * Vector2.one));
                int delay = img.Delay;
                mFrameDelay.Add(img.Delay / 1000.0f);

                img = decoder.NextImage();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Frames == null)
        {
            return;
        }

        mTime += Time.deltaTime;

        if (mTime >= mFrameDelay[mCurFrame])
        {
            mCurFrame = (mCurFrame + 1) % Frames.Count;
            mTime = 0.0f;

            spRender.sprite = Frames[mCurFrame];
        }

    }
}
