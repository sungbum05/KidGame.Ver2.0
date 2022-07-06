using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

[System.Serializable]
public class ResultObjClass
{
    public Sprite ResultImg;
    public Sprite ShadowImg;
    public int PickNum;
}

[System.Serializable]
public class ShowObj
{
    public GameObject Obj;
    public Sprite ObjImg;
    public Vector3 OriginalPos;
    public int PickNum;
}

enum SqurrielState
{
    Idle, Suprise, Like, Ending
}

public enum StageResult
{
    Fail = 0,
    Succes = 1,
}

public enum ShowType
{
    Hide = 0,
    Spawn = 1,
}

public class SquirrelGameMgr : Mgr
{
    [Header("SquirrelScene_Mgr_attribute")]
    [SerializeField]
    int SelectNum = 0;
    [SerializeField]
    int ResultNum = 0;
    [SerializeField]
    GameObject Squirrel;
    [SerializeField]
    GameObject Result = null;
    [SerializeField]
    GameObject Shadow = null;
    [SerializeField]
    GameObject SelectObj = null;

    [Space(10)]
    [SerializeField]
    List<ResultObjClass> ResultObjImgs = null;
    [SerializeField]
    List<ShowObj> Objs = null;
    [SerializeField]
    bool OnOption = false;

    [Header("SquirrelScene_Mgr_Mouse")]
    [Space(10)]
    [SerializeField]
    LayerMask layerMask;
    [SerializeField]
    Vector2 MaxPos;
    [SerializeField]
    Vector2 MinPos;
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
        SoundMgr.In.ChangeBGM("Calimba - E's Jammy Jams");

        HomeBtn.onClick.AddListener(() =>
        {
            SoundMgr.In.PlaySound("ButtonClick");
            SceneManager.LoadScene("SelectStageScene");
        });
        HomeBtn.gameObject.SetActive(false);

        RetryBtn.onClick.AddListener(() =>
        {
            SoundMgr.In.PlaySound("ButtonClick");
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

        if (Input.GetMouseButtonUp(0) && OnOption == false)
        {
            MouseUp();
        }
    }

    IEnumerator StartGame()
    {
        yield return null;

        #region ���� ��
        if (StartChk == true) // ���� �ϴ� ��
        {
            StartCoroutine(ResultObjsProduce(Shadow, ShowType.Hide));
            StartCoroutine(ResultObjsProduce(Result, ShowType.Hide));

            for (int i = 0; i < Objs.Count; i++)
            {
                StartCoroutine(ObjListProduce(Objs[i].Obj, ShowType.Hide));
                yield return new WaitForSeconds(ShowTime / 2);
            }

            InfoReset(StageResult.Succes);
            yield return new WaitForSeconds(ShowTime);

            CurGameCount++;

            ShuffleObj();
            StartCoroutine(ResultObjsProduce(Shadow, ShowType.Spawn));

            for (int i = 0; i < Objs.Count; i++)
            {
                StartCoroutine(ObjListProduce(Objs[i].Obj, ShowType.Spawn));
                yield return new WaitForSeconds(ShowTime / 2);
            }
        }
        #endregion

        #region ���� ���� ��
        else if (StartChk == false) //���� �����ϱ� ��
        {
            StartChk = true;
            CurGameCount++;

            FadePanel.DOFade(0, ShowTime / 1.2f);
            yield return new WaitForSeconds(ShowTime / 1.2f);
            FadePanel.gameObject.SetActive(false);

            GetShuffleList<ResultObjClass>(ResultObjImgs);

            ShuffleObj();

            StartCoroutine(ResultObjsProduce(Shadow, ShowType.Spawn));

            for (int i = 0; i < Objs.Count; i++)
            {
                StartCoroutine(ObjListProduce(Objs[i].Obj, ShowType.Spawn));
                yield return new WaitForSeconds(ShowTime / 2);
            }
        }
        #endregion

        #region ���� ����
        if (CurGameCount > MaxGameCount) //���� ����
        {
            ClearChk = true;
            StartCoroutine(AnimatorSet(SqurrielState.Ending));
            StartCoroutine(ClearShow());
        }
        #endregion
    }

    #region ���� �ý���
    protected override void ProgressSetting()
    {
        base.ProgressSetting();
    }

    void ShuffleObj() //shadow ���� �� Ư�� 3�� ������Ʈ �̹��� ����
    {
        int Ran = Random.Range(0, Objs.Count);
        Shadow.GetComponent<SpriteRenderer>().sprite = ResultObjImgs[Ran].ShadowImg;
        Result.GetComponent<SpriteRenderer>().sprite = ResultObjImgs[Ran].ResultImg;

        ResultNum = ResultObjImgs[Ran].PickNum;

        for (int i = 0; i < Objs.Count; i++)
        {
            Objs[i].ObjImg = ResultObjImgs[i].ResultImg;
            Objs[i].PickNum = ResultObjImgs[i].PickNum;
            Objs[i].Obj.GetComponent<SpriteRenderer>().sprite = Objs[i].ObjImg;
        }
    }

    void InfoReset(StageResult type)
    {
        if (type == StageResult.Fail)
        {
            SelectObj = null;
            SelectNum = 0;
        }

        else if (type == StageResult.Succes)
        {
            Debug.Log("Asdasd");
            SelectObj.transform.localPosition = Objs[int.Parse(SelectObj.name.Split('_')[1]) - 1].OriginalPos;
            //SelectObj.SetActive(true);

            SelectObj = null;
            SelectNum = 0;

            GetShuffleList<ResultObjClass>(ResultObjImgs);
        }
    }

    IEnumerator AnimatorSet(SqurrielState State)
    {
        switch ((int)State)
        {
            case 0:

                Squirrel.gameObject.GetComponent<Animator>().SetBool("Idle", true);
                Squirrel.gameObject.GetComponent<Animator>().SetBool("False", false);
                Squirrel.gameObject.GetComponent<Animator>().SetBool("Succes", false);

                break;

            case 1:

                Squirrel.gameObject.GetComponent<Animator>().SetBool("Idle", false);
                Squirrel.gameObject.GetComponent<Animator>().SetBool("False", true);
                Squirrel.gameObject.GetComponent<Animator>().SetBool("Succes", false);
                yield return new WaitForSeconds(ShowTime / 1.5f);

                StartCoroutine(AnimatorSet(SqurrielState.Idle));
                break;

            case 2:
                Squirrel.gameObject.GetComponent<Animator>().SetBool("Idle", false);
                Squirrel.gameObject.GetComponent<Animator>().SetBool("False", false);
                Squirrel.gameObject.GetComponent<Animator>().SetBool("Succes", true);
                yield return new WaitForSeconds(ShowTime / 1.5f);

                StartCoroutine(AnimatorSet(SqurrielState.Idle));
                break;

            case 3:
                Squirrel.gameObject.GetComponent<Animator>().SetBool("Idle", false);
                Squirrel.gameObject.GetComponent<Animator>().SetBool("False", false);
                Squirrel.gameObject.GetComponent<Animator>().SetBool("Succes", true);
                break;
        }
    }

    protected override IEnumerator ClearShow()
    {
        yield return base.ClearShow();
    }
    #endregion

    #region  ���콺 ��ȣ�ۿ� �Լ���
    void MouseClick() // ���콺�� ������ �ִµ��� ray����
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

            RaycastHit2D hit = Physics2D.Raycast(MousePos, transform.forward, 10.0f, layerMask);

            if (hit)
            {
                SelectNum = Objs[int.Parse(hit.transform.gameObject.name.Split('_')[1]) - 1].PickNum;
                SelectObj = hit.transform.gameObject;

                hit.collider.gameObject.transform.position = MousePos;
                hit.collider.gameObject.transform.localScale = Vector3.one * 2;
            }
        }
    }

    void MouseUp()
    {
        if (ClearChk == false)
        {
            if (MousePos.x < MaxPos.x && MousePos.x > MinPos.x && MousePos.y < MaxPos.y && MousePos.y > MinPos.y && SelectNum == ResultNum)
            {
                StartCoroutine(AnimatorSet(SqurrielState.Like));
                SoundMgr.In.PlaySound("Succes");

                Debug.Log("Yes");
                StartCoroutine(SuccesThisStage(SelectObj, StageResult.Succes));
            }

            else
            {
                StartCoroutine(AnimatorSet(SqurrielState.Suprise));
                SoundMgr.In.PlaySound("Fail");

                Debug.Log("No");
                StartCoroutine(SuccesThisStage(SelectObj, StageResult.Fail));
            }
        }
    }
    #endregion

    #region ������Ʈ����(����Ʈ ����/����, �׸��� �������)
    IEnumerator SuccesThisStage(GameObject Obj, StageResult Type) // ������Ʈ�� �°ų� Ʋ�� �� ����(�Ű�����: ���� �� ������Ʈ, ���� ȿ�� Ÿ��), (ȿ��: ������ ����, ��ġ �̵�, ���İ� ����)
    {
        yield return null;

        if ((int)Type == 0)
        {
            Obj.transform.DOScale(0, ShowTime / 2);

            yield return new WaitForSeconds(ShowTime / 2);

            Obj.transform.localPosition = Objs[int.Parse(Obj.name.Split('_')[1]) - 1].OriginalPos;
            Obj.transform.DOScale(1, ShowTime / 2);
            InfoReset(StageResult.Fail);
        }

        else
        {
            SelectObj.SetActive(false);
            Result.GetComponent<SpriteRenderer>().DOFade(1, ShowTime);

            yield return new WaitForSeconds(ShowTime);

            StartCoroutine(StartGame()); //���� �����̶�� �ٽ� �������� ����
        }

    }

    IEnumerator ResultObjsProduce(GameObject obj, ShowType type) // �׸��� �⿬ ����(�Ű�����: ���� �� ������Ʈ, ���� ȿ�� Ÿ��), (ȿ��: Ȯ��, �� ����)
    {
        yield return null;

        obj.GetComponent<SpriteRenderer>().DOFade((int)type, ShowTime * 1.5f);
    }

    IEnumerator ObjListProduce(GameObject Obj, ShowType type) // ����Ʈ �� �峭�� �⿬ ����(�Ű�����: ���� �� ������Ʈ, ���� ȿ��), (ȿ��: Ȯ��, ȸ��)
    {
        yield return null;

        Obj.transform.DOScale((int)type, ShowTime * 1f);

        if(type == ShowType.Spawn)
            Obj.SetActive(true);

        while (true)
        {
            yield return null;
            Obj.transform.Rotate(0, 0, (360 * (type == ShowType.Spawn ? -1 : 1)) * Time.deltaTime);

            if (Obj.transform.localScale.x == (int)type)
            {
                break;
            }
        }

        Obj.transform.rotation = Quaternion.Euler(0, 0, 0);
        StopCoroutine(ObjListProduce(Obj, type));
    }
    #endregion

    #region ���� â ����
    public void OptionPanOnOff()
    {
        SoundMgr.In.PlaySound("ButtonClick");
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
        SoundMgr.In.PlaySound("ButtonClick");
        SceneManager.LoadScene("SelectStageScene");
    }
    #endregion
}
