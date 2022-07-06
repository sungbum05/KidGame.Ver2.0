using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mgr : MonoBehaviour //각 씬 매니져들 상속
{
    [Header("Mgr_Basic_Setting_start")]
    [SerializeField]
    protected int MaxGameCount = 5;
    [SerializeField]
    protected int CurGameCount = 0;
    [SerializeField]
    protected bool StartChk = false;
    [SerializeField]
    protected bool ClearChk = false;
    [SerializeField]
    protected float ShowTime = 1.0f;
    [SerializeField]
    protected Image FadePanel;
    [Space(10)]
    [SerializeField]
    List<GameObject> ProgressPoint;

    [Header("Mgr_Basic_Setting_Ending")]
    #region 게임 끝 연출
    [Space(10)]
    [SerializeField]
    protected Vector3 BallonSpawnPoint;
    [SerializeField]
    protected List<GameObject> SideClearBallon;
    [SerializeField]
    protected GameObject MainClearBallon;
    [SerializeField]
    protected GameObject balloonburst;
    [SerializeField]
    protected LayerMask ClearLayer;
    [SerializeField]
    protected Button HomeBtn;
    [SerializeField]
    protected Button RetryBtn;
    #endregion

    protected virtual void ProgressSetting()
    {
        if (CurGameCount > 0)
        {
            int i = 0;

            //if (CurGameCount > MaxGameCount)
            //    ClearChk = true;

            foreach (GameObject obj in ProgressPoint)
            {
                if (i <= CurGameCount - 1)
                {
                    obj.GetComponent<Image>().color = Color.white;
                }

                i++;
            }

            i = 0;
        }
    } // 진행상황 바 업데이트

    protected virtual IEnumerator ClearShow()
    {
        yield return null;
        FadePanel.gameObject.SetActive(true);

        for (int i = 0; i < 30; i++)
        {
            yield return new WaitForSeconds(Random.Range(0.1f, 0.3f));

            BallonSpawnPoint = new Vector3(Random.Range(-8.0f, 8.0f), -13.0f, 0f);
            Instantiate(SideClearBallon[Random.Range(0, SideClearBallon.Count)], BallonSpawnPoint, Quaternion.identity);
        }

        yield return new WaitForSeconds(0.5f);
        Instantiate(MainClearBallon, new Vector3(0, -13.0f, 0), Quaternion.identity);
        yield return new WaitForSeconds(1.0f);
        HomeBtn.gameObject.SetActive(true);
        RetryBtn.gameObject.SetActive(true);
    } //게임 끝 연출

    protected List<T> GetShuffleList<T>(List<T> _list) // 제네릭 리스트를 이용한 리스트 랜덤 셔플 함수
    {
        for (int i = _list.Count - 1; i > 0; i--)
        {
            int rnd = Random.Range(0, i + 1); // 1 추가 해서 첫번 째 오브젝트도 첫번째로 나올 수 있도록 수정함

            T temp = _list[i];
            _list[i] = _list[rnd];
            _list[rnd] = temp;
        }

        return _list;
    }//(출처 : https://drehzr.tistory.com/802) // 수정: sblim05
}
