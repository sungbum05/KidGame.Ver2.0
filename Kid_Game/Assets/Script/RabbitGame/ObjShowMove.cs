using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[System.Serializable]
enum ObjType
{
    Rabbit, Milk, Towel
}

public class ObjShowMove : MonoBehaviour
{
    [SerializeField]
    ObjType ObjType;

    public void StartObjShow()
    {
        switch((int)ObjType)
        {
            case 0:
                //���� ��ȹ�Ȱ� �����ϴ�.
                break;

            case 1:
                this.gameObject.GetComponent<SpriteRenderer>().DOFade(0, 2);
                break;

            case 2:
                this.gameObject.GetComponent<SpriteRenderer>().DOFade(0, 2);
                break;

            default:
                Debug.Log("sad");
                break;
        }
    }
}
