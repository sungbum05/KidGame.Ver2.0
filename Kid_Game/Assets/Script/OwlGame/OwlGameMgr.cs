using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

enum OwlState
{
    Idle, Like
}

[System.Serializable]
public class ObjsAttribute
{
    public GameObject Obj;
    public GameObject ShadowObjs;
    public bool SuccesObj = false;
}


public class OwlGameMgr : Mgr
{
    [Header("Owl_Mgr_attribute")]
    [SerializeField]
    GameObject FindCircle = null;
    [SerializeField]
    List<ObjsAttribute> Objs = null;
    [SerializeField]
    List<Transform> ObjSpawnPos = null;
    [SerializeField]
    GameObject Owl;
    [SerializeField]
    bool OnOption = false;

    [Header("Owl_Mgr_Mouse")]
    [Space(10)]
    [SerializeField]
    LayerMask layerMask;
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
        SoundMgr.In.ChangeBGM("Racing_The_Clock_-_The_Green_Orbs");

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
            MouseClick();
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
            StartCoroutine(AnimatorSet(OwlState.Idle));

            StartChk = true;
            FadePanel.DOFade(0, ShowTime / 1.2f);
            yield return new WaitForSeconds(ShowTime / 1.2f);
            FadePanel.gameObject.SetActive(false);

            ObjRandomSpawn();
        }
        #endregion

        #region 게임 종료
        if (ClearChk == true) //게임 끝남
        {

        }
        #endregion
    }

    #region  마우스 상호작용 함수들
    void MouseClick() // 마우스를 누르고 있는동안 ray실행
    {
        MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (ClearChk == true)
        {
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
            FindCircle.transform.position = MousePos;
        }
    }
    #endregion

    private void ObjRandomSpawn()
    {
        GetShuffleList<Transform>(ObjSpawnPos);

        for (int i = 0; i < Objs.Count; i++)
        {
            Objs[i].Obj.transform.localPosition = ObjSpawnPos[i].transform.localPosition;
        }
    }

    #region 오브젝트 이동 및 연출 효과
    public IEnumerator ShowObj(int ObjNum)
    {
        SoundMgr.In.PlaySound("Succes");

        yield return null;

        FindCircle.gameObject.transform.DOScale(Vector2.zero, ShowTime);
        yield return new WaitForSeconds(ShowTime);

        GameObject ChildObj = Objs[ObjNum].Obj.transform.GetChild(0).gameObject;
        ChildObj.GetComponent<SpriteMask>().enabled = true;
        ChildObj.gameObject.transform.DOScale(0.5f, ShowTime);
        yield return new WaitForSeconds(ShowTime);


        StartCoroutine(MoveToObj(ObjNum));
    }

    IEnumerator MoveToObj(int ObjNum)
    {
        StartCoroutine(AnimatorSet(OwlState.Like));

        yield return null;
        Debug.Log(ObjNum);

        float UserTime = 1;

        while (true)
        {
            yield return null;

            UserTime += (Time.deltaTime * 2);

            Objs[ObjNum].Obj.transform.position = Vector3.Slerp(Objs[ObjNum].Obj.transform.position,
                Objs[ObjNum].ShadowObjs.transform.position, 0.8f * Time.deltaTime * UserTime);

            if (Objs[ObjNum].Obj.transform.position == Objs[ObjNum].ShadowObjs.transform.position)
                break;
        }

        Debug.Log("End");
        Objs[ObjNum].Obj.transform.GetChild(0).gameObject.GetComponent<SpriteMask>().enabled = false;
        Objs[ObjNum].SuccesObj = true;

        FindCircle.gameObject.transform.DOScale(Vector2.one, ShowTime);
        yield return new WaitForSeconds(ShowTime);

        StartCoroutine(ClearShow());
    }

    IEnumerator AnimatorSet(OwlState State)
    {
        switch ((int)State)
        {
            case 0:

                Owl.gameObject.GetComponent<Animator>().SetBool("Idle", true);
                Owl.gameObject.GetComponent<Animator>().SetBool("False", false);
                Owl.gameObject.GetComponent<Animator>().SetBool("Succes", false);

                break;

            case 1:
                Owl.gameObject.GetComponent<Animator>().SetBool("Idle", false);
                Owl.gameObject.GetComponent<Animator>().SetBool("False", false);
                Owl.gameObject.GetComponent<Animator>().SetBool("Succes", true);
                yield return new WaitForSeconds(ShowTime);

                StartCoroutine(AnimatorSet(OwlState.Idle));
                break;
        }
    }

    protected override IEnumerator ClearShow()
    {
        yield return null;
        bool Chk = true;

        foreach (var obj in Objs)
        {
            if (obj.SuccesObj == false)
            {
                Chk = false;
            }
        }

        if (Chk)
        {
            Debug.Log("asd");
            FindCircle.gameObject.transform.DOScale(15, ShowTime * 5);
            FindCircle.GetComponent<FindCircle>().enabled = false;
            ClearChk = true;

            yield return new WaitForSeconds(ShowTime * 5);
            yield return base.ClearShow();
        }
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
