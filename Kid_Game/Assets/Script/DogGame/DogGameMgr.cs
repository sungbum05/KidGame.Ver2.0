using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

enum DogState
{
    Idle, Angry, Like
}

[System.Serializable]
public class PieceObj
{
    public GameObject Obj;
    public GameObject AnswerObj;
    public Vector2 AnswerObjPos;
    public GameObject OriginalPos;
    public bool SuccesPiece;
}

public class DogGameMgr : Mgr
{
    [Header("Dog_Mgr_attribute")]
    [SerializeField]
    List<PieceObj> Objs = null;
    [SerializeField]
    List<Transform> ObjSpawnPos = null;
    [SerializeField]
    GameObject Dog;

    [Space]
    [SerializeField]
    GameObject SelectObject = null;
    [SerializeField]
    GameObject AnswerObject = null;
    [SerializeField]
    GameObject SelectAnwerPos = null;
    [SerializeField]
    bool OnOption = false;

    [Header("Dog_Mgr_ClearShow")]
    [Space(10)]
    [SerializeField]
    GameObject Pieces;
    [SerializeField]
    GameObject ShowObjs;
    [SerializeField]
    GameObject FrameObj;

    [Header("Dog_Mgr_Mouse")]
    [Space(10)]
    [SerializeField]
    LayerMask ObjlayerMask;
    [SerializeField]
    LayerMask AnswerObjlayerMask;
    Vector2 MousePos;

    [Header("Buttons")]
    [SerializeField]
    private Button OptionBtn;

    [Header("OtherPanel")]
    [SerializeField]
    private GameObject OptionPan;

    // Start is called before the first frame update
    void Start()
    {
        SoundMgr.In.ChangeBGM("Cute_Avalanche_-_RKVC");

        HomeBtn.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("SelectStageScene");
        });
        HomeBtn.gameObject.SetActive(false);

        RetryBtn.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        });

        StartCoroutine(StartGame());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0) && OnOption == false)
        {
            MouseClick();
        }

        if (Input.GetMouseButtonUp(0) && OnOption == false)
        {
            MouseUp();
        }
    }

    IEnumerator StartGame()
    {
        yield return null;

        #region 게임 중
        if (StartChk == true) // 게임 하는 중
        {

        }
        #endregion

        #region 게임 시작 전
        else if (StartChk == false) //게임 시작하기 전
        {
            StartCoroutine(AnimatorSet(DogState.Idle));
            StartChk = true;

            BasicSetting();

            FadePanel.DOFade(0, ShowTime / 1.2f);
            yield return new WaitForSeconds(ShowTime / 1.2f);
            FadePanel.gameObject.SetActive(false);
        }
        #endregion

        #region 게임 종료
        if (ClearChk == true) //게임 끝남
        {
            Debug.Log("eend");
            StartCoroutine(ClearShow());
        }
        #endregion
    }

    #region  마우스 상호작용 함수들
    void MouseClick() // 마우스를 누르고 있는동안 ray실행
    {
        if (ClearChk == true)
        {
            MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            RaycastHit2D hit = Physics2D.Raycast(MousePos, transform.forward, 10.0f, ClearLayer);
            if (hit)
            {
                SoundMgr.In.PlaySound("Balloon_Pop");

                Instantiate(balloonburst, new Vector2(hit.collider.gameObject.transform.position.x, hit.collider.gameObject.transform.position.y + hit.collider.gameObject.GetComponent<BoxCollider2D>().offset.y), Quaternion.identity);
                Destroy(hit.collider.gameObject);
            }
        }

        else
        {
            MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            RaycastHit2D Objhit = Physics2D.Raycast(MousePos, transform.forward, 10.0f, ObjlayerMask);
            if (Objhit)
            {
                Objhit.collider.gameObject.transform.position = MousePos;

                MatchingObject(Objhit.collider.gameObject);
            }

            RaycastHit2D AnswerObj = Physics2D.Raycast(MousePos, transform.forward, 10.0f, AnswerObjlayerMask);
            if (AnswerObj)
            {
                SelectAnwerPos = AnswerObj.collider.gameObject;
            }
        }
    }

    void MouseUp()
    {
        if (ClearChk == false)
        {
            if (AnswerObject == SelectAnwerPos)
            {
                SoundMgr.In.PlaySound("Succes");
                StartCoroutine(AnimatorSet(DogState.Like));

                StartCoroutine(SelectObject.GetComponent<PieceMove>().MoveToObj(SelectObject, AnswerObject));
                ResetObject(int.Parse(SelectObject.name.Split('_')[1]) - 1);
            }

            else
            {
                SoundMgr.In.PlaySound("Fail");

                StartCoroutine(AnimatorSet(DogState.Angry));
                SelectObject.transform.position = Objs[int.Parse(SelectObject.name.Split('_')[1]) - 1].OriginalPos.transform.position;
            }
        }
    }
    #endregion

    #region 시스템 내부 기능
    void BasicSetting()
    {
        GetShuffleList<Transform>(ObjSpawnPos);

        for (int i = 0; i < Objs.Count; i++)
        {
            Objs[i].Obj.transform.localPosition = ObjSpawnPos[i].transform.localPosition;
            Objs[i].OriginalPos = ObjSpawnPos[i].gameObject;

            Objs[i].AnswerObj.transform.position = Objs[i].AnswerObjPos;
        }

    }

    void MatchingObject(GameObject Obj)
    {
        SelectObject = Obj;
        AnswerObject = Objs[int.Parse(Obj.name.Split('_')[1]) - 1].AnswerObj;
    }

    void ResetObject(int ObjNum)
    {
        SelectObject.GetComponent<CircleCollider2D>().enabled = false;
        Objs[ObjNum].Obj.GetComponent<SpriteRenderer>().sortingOrder = 4;
        Objs[ObjNum].SuccesPiece = true;

        SelectObject = null;
        AnswerObject = null;
        SelectAnwerPos = null;

        int EndCheak = 0;

        foreach (var obj in Objs)
        {
            EndCheak += obj.SuccesPiece == true ? 1 : 0;
        }

        if (EndCheak >= Objs.Count)
        {
            ClearChk = true;
            StartCoroutine(StartGame());
        }
    }

    IEnumerator AnimatorSet(DogState State)
    {
        switch ((int)State)
        {
            case 0:

                Dog.gameObject.GetComponent<Animator>().SetBool("Idle", true);
                Dog.gameObject.GetComponent<Animator>().SetBool("False", false);
                Dog.gameObject.GetComponent<Animator>().SetBool("Succes", false);

                break;

            case 1:

                Dog.gameObject.GetComponent<Animator>().SetBool("Idle", false);
                Dog.gameObject.GetComponent<Animator>().SetBool("False", true);
                Dog.gameObject.GetComponent<Animator>().SetBool("Succes", false);
                yield return new WaitForSeconds(ShowTime / 1.5f);

                StartCoroutine(AnimatorSet(DogState.Idle));
                break;

            case 2:
                Dog.gameObject.GetComponent<Animator>().SetBool("Idle", false);
                Dog.gameObject.GetComponent<Animator>().SetBool("False", false);
                Dog.gameObject.GetComponent<Animator>().SetBool("Succes", true);
                yield return new WaitForSeconds(ShowTime / 1.5f);

                StartCoroutine(AnimatorSet(DogState.Idle));
                break;
        }
    }

    protected override IEnumerator ClearShow()
    {
        yield return null;
        yield return new WaitForSeconds(ShowTime / 3);

        ClearChk = true;

        Pieces.gameObject.SetActive(false);
        ShowObjs.gameObject.SetActive(true);

        FrameObj.GetComponent<SpriteRenderer>().DOFade(0, ShowTime * 2);

        yield return new WaitForSeconds(ShowTime * 2);
        yield return base.ClearShow();
    }
    #endregion

    #region 설정 창 관리
    public void OptionPanOnOff()
    {
        if (OptionPan.active)
        {
            OnOption = false;
            OptionPan.SetActive(false);
        }


        else
        {
            OnOption = true;
            OptionPan.SetActive(true);
        }
    }

    public void GotoLobby()
    {
        SceneManager.LoadScene("SelectStageScene");
    }
    #endregion
}
