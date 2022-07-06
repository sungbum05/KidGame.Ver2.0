using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum FadeType // ��� �� ���̵� Ÿ��
{
    FadeIn = 0,
    FadeOut = 1,
    FadeLoop = 2,
}

public enum FadeObjType // ���̵��� ������Ʈ Ÿ��
{
    Sprite = 0,
    Image = 1,
    RawImage = 2,
    Text = 3,
}

public class FadeInOut : MonoBehaviour
{
    protected void StartFade(FadeType FadeType, FadeObjType ObjType, UnityEngine.Object Obj) // ���̵� Ÿ��, ���̵� �� ������Ʈ ����, ���̵� �� ������Ʈ
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
            #region Ÿ�� �з�
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
