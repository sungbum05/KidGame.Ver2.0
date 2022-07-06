using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum FadeType // 사용 될 페이드 타입
{
    FadeIn = 0,
    FadeOut = 1,
    FadeLoop = 2,
}

public enum FadeObjType // 페이드할 오브젝트 타입
{
    Sprite = 0,
    Image = 1,
    RawImage = 2,
    Text = 3,
}

public class FadeInOut : MonoBehaviour
{
    protected void StartFade(FadeType FadeType, FadeObjType ObjType, UnityEngine.Object Obj) // 페이드 타입, 페이드 할 오브젝트 종류, 페이드 할 오브젝트
    {
        switch (FadeType)
        {
            case FadeType.FadeIn:
                Debug.Log(FadeType.FadeIn);
                StartCoroutine(FadeIn(ObjType, Obj));
                break;

            case FadeType.FadeOut:
                Debug.Log(FadeType.FadeOut);
                StartCoroutine(FadeOut(ObjType, Obj));
                break;

            case FadeType.FadeLoop:
                Debug.Log(FadeType.FadeLoop);
                StartCoroutine(FadeLoop(ObjType, Obj));
                break;
        }
    }

    IEnumerator FadeIn(FadeObjType objType, Object Obj)
    {
        yield return null;

        //Debug.Log(Obj.ToString());    
        Debug.Log(Obj.ToString());

    }

    IEnumerator FadeOut(FadeObjType objType, Object Obj)
    {
        yield return null;
        Debug.Log(Obj.GetType());
        //Debug.Log(Obj.ToString());
    }

    IEnumerator FadeLoop(FadeObjType objType, Object Obj)
    {
        yield return null;
        switch (objType)
        {
            #region 타입 분류
            case FadeObjType.Sprite:
                SpriteRenderer spriteRenderer = (SpriteRenderer)Obj;
                break;

            case FadeObjType.Image:
                Image image = (Image)Obj;
                break;

            case FadeObjType.RawImage:
                RawImage rawImage = (RawImage)Obj;
                break;

            case FadeObjType.Text:
                Text text = (Text)Obj;
                break;
                #endregion
        }
    }
}
