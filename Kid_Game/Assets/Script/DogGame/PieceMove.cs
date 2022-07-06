using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceMove : MonoBehaviour
{
    public IEnumerator MoveToObj(GameObject SelectObj, GameObject AnswerObj)
    {
        yield return null;
        SelectObj.transform.position = AnswerObj.transform.position;

        StopCoroutine(MoveToObj(SelectObj, AnswerObj));
    }
}
