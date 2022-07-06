using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;

enum AntState
{
    Idle, Surprise, Like
}

public class AntGameScene : Mgr
{
    [Header("AntScene_Mgr_attribute")]
    [SerializeField]
    int SelectNum;
    [SerializeField]
    int PickNum;

    [Space(10)]
    [SerializeField]
    private Vector2 EnterPos; // 개미 생성 지점
    [SerializeField]
    private Vector2 StayPos; // 개미 머무는 지점
    [SerializeField]
    private Vector2 ExitPos; // 개미 나가는 지점

    [Space(10)]
    [SerializeField]
    private GameObject Breads;
    [SerializeField]
    private Vector2 BreadStayPos;
    [SerializeField]
    private Vector2 BreadExitPos;
    [SerializeField]
    private List<GameObject> Bread;
    [SerializeField]
    private List<GameObject> BreadPos;

    [Space(10)]
    [SerializeField]
    LayerMask layerMask;
    Vector2 MousePos;

    [Space(10)]
    [SerializeField]
    private List<GameObject> AntGroup;
    [SerializeField]
    GameObject Ants;
    [SerializeField]
    BoxCollider2D AntZone;
    [SerializeField]
    Sprite AntChangeImg;

    [SerializeField]
    bool OnOption = false;

    [Header("Buttons")]
    [SerializeField]
    private Button OptionBtn;

    [Header("OtherPanel")]
    [SerializeField]
    private GameObject OptionPan;

    // Start is called before the first frame update
    void Start()
    {
        SoundMgr.In.ChangeBGM("Happy_Mistake_-_RKVC");

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
        ProgressSetting();

        if (Input.GetMouseButton(0) && OnOption == false)
        {
            MouseClick();
        }

        if(Input.GetMouseButtonUp(0) && OnOption == false)
        {
            MouseUp();
        }

        if (CurGameCount > MaxGameCount && ClearChk == false)
        {
            ClearChk = true;

            Debug.Log("end");
            StartCoroutine(ClearShow());
        }
    }

    protected override void ProgressSetting()
    {
        base.ProgressSetting();
    }

    IEnumerator StartGame()
    {
        yield return null;
        if (StartChk == false)
        {
            yield return null;
            FadePanel.DOFade(0, ShowTime / 1.2f);
            yield return new WaitForSeconds(ShowTime / 1.2f);
            FadePanel.gameObject.SetActive(false);
        }

        Destroy(Ants);
        Ants = null;

        StartCoroutine(EnterAnt());
        StartCoroutine(BreadPosChange());

        CurGameCount++;
    }

    #region  마우스 상호작용 함수들
    void MouseClick() // 마우스를 누르고 있는동안 ray실행
    {
        if (ClearChk == true)
        {
            Vector2 MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

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

            RaycastHit2D hit = Physics2D.Raycast(MousePos, transform.forward, 10.0f, layerMask);
            if (hit)
            {
                string[] SpiltName = hit.collider.name.Split('_');
                PickNum = int.Parse(SpiltName[1]);
                hit.collider.gameObject.transform.position = MousePos;
            }
        }
    }

    void MouseUp()
    {
        if (ClearChk == false)
        {
            if ((Mathf.Abs(MousePos.x) < Mathf.Abs(AntZone.bounds.extents.x) && Mathf.Abs(MousePos.y) < Mathf.Abs(AntZone.bounds.extents.y)) && SelectNum == PickNum)
            {
                SoundMgr.In.PlaySound("Succes");

                Debug.Log("Yes");
                foreach (Transform Child in Ants.transform)
                {
                    Child.gameObject.GetComponent<SpriteRenderer>().sprite = AntChangeImg;
                    StartCoroutine(AnimatorSet(AntState.Like));

                    Bread[PickNum - 1].SetActive(false);
                }

                StartCoroutine(ExitAnt());
            }

            else
            {
                SoundMgr.In.PlaySound("Fail");

                Bread[PickNum - 1].transform.DOMove(BreadPos[PickNum - 1].transform.position, ShowTime / 2);
                StartCoroutine(AnimatorSet(AntState.Surprise));

                PickNum = 0;
            }
        }
    }
    #endregion

    #region 개미 연출
    IEnumerator EnterAnt()
    {
        yield return null;
        GetShuffleList<GameObject>(AntGroup);
        string[] SplitName = AntGroup[0].name.Split('_');
        SelectNum = int.Parse(SplitName[1]);

        Ants = Instantiate(AntGroup[0], EnterPos, Quaternion.identity);
        StartCoroutine(AnimatorSet(AntState.Idle));

        Ants.transform.DOMove(StayPos, ShowTime * 1.5f);

        yield return new WaitForSeconds(ShowTime * 2.0f);
    }

    IEnumerator ExitAnt()
    {
        yield return new WaitForSeconds(ShowTime);

        Ants.transform.DOMove(ExitPos, ShowTime * 2.0f);

        yield return new WaitForSeconds(ShowTime * 2);

        StartCoroutine(StartGame());
    }
    #endregion

    IEnumerator BreadPosChange() // 빵의 위치를 바꿔준다(연출 포함)
    {
        yield return null;

        if (StartChk == true)
        {
            Bread[PickNum - 1].transform.position = BreadPos[PickNum - 1].transform.position;
            Breads.transform.DOMove(BreadExitPos, ShowTime);

            yield return new WaitForSeconds(ShowTime);
        }

        int i = 0;

        GetShuffleList<GameObject>(BreadPos);

        foreach(Transform Child in Breads.transform)
        {
            Child.transform.position = new Vector2(BreadPos[i].transform.position.x, Child.transform.position.y);
            string[] SplitName = BreadPos[i].name.Split('_');
            Child.gameObject.GetComponent<Bread>().Pos = int.Parse(SplitName[1]);
            i++;
        }

        if (StartChk == true)
        {
            Debug.Log("asdad");
            Bread[PickNum - 1].SetActive(true);
            PickNum = 0;

            yield return new WaitForSeconds(ShowTime / 2);       
        }

        Breads.transform.DOMove(BreadStayPos, ShowTime);

        StartChk = true;
        i = 0;
    }

    IEnumerator AnimatorSet(AntState State)
    {
        switch((int)State)
        {
            case 0:
                foreach(Transform Ant in Ants.transform)
                {
                    Ant.gameObject.GetComponent<Animator>().SetBool("Idle", true);
                    Ant.gameObject.GetComponent<Animator>().SetBool("False", false);
                    Ant.gameObject.GetComponent<Animator>().SetBool("Succes", false);
                }
                break;

            case 1:
                foreach (Transform Ant in Ants.transform)
                {
                    Ant.gameObject.GetComponent<Animator>().SetBool("Idle", false);
                    Ant.gameObject.GetComponent<Animator>().SetBool("False", true);
                    Ant.gameObject.GetComponent<Animator>().SetBool("Succes", false);
                }
                yield return new WaitForSeconds(ShowTime / 1.5f);

                StartCoroutine(AnimatorSet(AntState.Idle));
                break;

            case 2:
                foreach (Transform Ant in Ants.transform)
                {
                    Ant.gameObject.GetComponent<Animator>().SetBool("Idle", false);
                    Ant.gameObject.GetComponent<Animator>().SetBool("False", false);
                    Ant.gameObject.GetComponent<Animator>().SetBool("Succes", true);
                }
                break;
        }
    }

    protected override IEnumerator ClearShow()
    {
        yield return base.ClearShow();
    }

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
