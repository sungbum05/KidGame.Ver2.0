using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TestUI : FadeInOut
{
    public SpriteRenderer spriteRenderer;
    public Image image;
    public RawImage rawImage;
    public Text text;

    // Start is called before the first frame update
    void Start()
    {
        StartFade(FadeType.FadeLoop, FadeObjType.Sprite, spriteRenderer);
    }

    // Update is called once per frame

}
