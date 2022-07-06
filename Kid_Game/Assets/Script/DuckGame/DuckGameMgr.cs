using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

enum DuckState
{
    Idle, Suprise, Like, Ending
}

public class DuckGameMgr : Mgr
{
    [Header("DuckScene_Mgr_attribute")]
    #region ���� ����
    [SerializeField]
    private string SelectColor; // ���õ� �÷� �޾ƿ� ����
    [SerializeField]
    private List<string> g_Color; // �÷� ê�ڽ��� ������ ������ �÷�

    [SerializeField]
    private SpriteRenderer ChatBox; // �����鿡�� ������ ������ �÷�

    [SerializeField]
    private List<Sprite> ColorChatBox; // ���� ������ �θ� �÷� �̹�����

    [SerializeField]
    GameObject MotherDuck;
    public Dictionary<string, Sprite> ColorChatBoxkDic = new Dictionary<string, Sprite>();
    [SerializeField]
    private List<Button> BabyDuckBtns; // �ֱ� ������ ��ȣ �ۿ� ��ư

    [SerializeField]
    bool OnOption = false;

    [Header("Buttons")]
    [SerializeField]
    private Button OptionBtn;

    [Header("OtherPanel")]
    [SerializeField]
    private GameObject OptionPan;
    #endregion

    private void Awake()
    {
        DicSetting();
    }

    // Start is called before the first frame update
    void Start()
    {
        SoundMgr.In.ChangeBGM("Jigsaw_Puzzle_-_The_Green_Orbs");

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

        foreach (Button Btn in BabyDuckBtns)
        {
            Btn.onClick.AddListener(() =>
            {
                if (Btn.GetComponent<BabyDuckInfo>().BabyColor == SelectColor)
                {
                    StartCoroutine(AnimatorSet(DuckState.Like));
                    SoundMgr.In.PlaySound("Succes");

                    StartCoroutine(StartGame());
                    Debug.Log(Btn.name);
                }

                else
                {
                    StartCoroutine(AnimatorSet(DuckState.Suprise));
                    SoundMgr.In.PlaySound("Fail");
                }
            });
        }  
    }

    // Update is called once per frame
    void Update()
    {
        ProgressSetting();

        if(Input.GetMouseButtonDown(0) && OnOption == false)
        {
            Debug.Log("Down");
            MouseClick();
        }

        if(CurGameCount > MaxGameCount && StartChk == true)
        {
            StartChk = false;
            ClearChk = true;
            StartCoroutine(ClearShow());
            StartCoroutine(AnimatorSet(DuckState.Ending));
        }
    }

    void MouseClick() // ���콺�� ������ �ִµ��� ray����
    {
        if (ClearChk == true)
        {
            Vector2 MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            RaycastHit2D hit = Physics2D.Raycast(MousePos, transform.forward, 10.0f, ClearLayer);
            if (hit)
            {
                SoundMgr.In.PlaySound("Balloon_Pop");

                Instantiate(balloonburst, new Vector2 (hit.collider.gameObject.transform.position.x, hit.collider.gameObject.transform.position.y + hit.collider.gameObject.GetComponent<BoxCollider2D>().offset.y), Quaternion.identity);
                Destroy(hit.collider.gameObject);
            }
        }
    }

    protected override void ProgressSetting()
    {
        base.ProgressSetting();
    }

    IEnumerator StartGame()
    {
        yield return null;

        if(StartChk == true)
        {
            StartCoroutine(ExitDuck());
            yield return new WaitForSeconds(1.0f);
        }

        if (StartChk == false)
        {
            FadePanel.DOFade(0, ShowTime / 1.2f);

            StartChk = true;

            GetShuffleList<string>(g_Color);
            SettingBabyDuck();
            SettingColorChatBox();
            yield return new WaitForSeconds(ShowTime / 1.2f);
            FadePanel.gameObject.SetActive(false);
        }

        else
        {
            StartChk = true;

            GetShuffleList<string>(g_Color);
            SettingBabyDuck();
            SettingColorChatBox();
        }

        CurGameCount += 1;
    }

    IEnumerator ExitDuck()
    {
        yield return null;

        foreach (Button Btn in BabyDuckBtns)
        {
            Btn.gameObject.GetComponent<Image>().DOFade(0, 1);
        }

        yield return new WaitForSeconds(1.4f);

        StartCoroutine(EnterDuck());
    }

    IEnumerator EnterDuck()
    {
        yield return null;

        foreach (Button Btn in BabyDuckBtns)
        {
            Btn.gameObject.GetComponent<Image>().DOFade(1, 1);
        }
    }

    void DicSetting()
    {
        foreach (Sprite Img in ColorChatBox)
        {
            string[] NameSplit = Img.name.Split('_');
            ColorChatBoxkDic.Add(NameSplit[1], Img);
        }
    } // Dic����

    private void SettingBabyDuck()
    {
        int i = 0;

        foreach(Button Btn in BabyDuckBtns)
        {
            Btn.GetComponent<BabyDuckInfo>().BabyColor = g_Color[i];
            Btn.GetComponent<BabyDuckInfo>().ColorSetting();
            i++;
        }

        i = 0;
    } // �Ʊ� ������ ���� ����

    private void SettingColorChatBox()
    {
        int SelectColorIdx = Random.Range(0, 3);
        SelectColor = g_Color[SelectColorIdx];

        ChatBox.sprite = ColorChatBoxkDic[SelectColor];
    } // ê�ڽ� ���� ����

    IEnumerator AnimatorSet(DuckState State)
    {
        switch ((int)State)
        {
            case 0:

                MotherDuck.gameObject.GetComponent<Animator>().SetBool("Idle", true);
                MotherDuck.gameObject.GetComponent<Animator>().SetBool("False", false);
                MotherDuck.gameObject.GetComponent<Animator>().SetBool("Succes", false);

                break;

            case 1:

                MotherDuck.gameObject.GetComponent<Animator>().SetBool("Idle", false);
                MotherDuck.gameObject.GetComponent<Animator>().SetBool("False", true);
                MotherDuck.gameObject.GetComponent<Animator>().SetBool("Succes", false);
                yield return new WaitForSeconds(ShowTime / 1.5f);

                StartCoroutine(AnimatorSet(DuckState.Idle));
                break;

            case 2:
                MotherDuck.gameObject.GetComponent<Animator>().SetBool("Idle", false);
                MotherDuck.gameObject.GetComponent<Animator>().SetBool("False", false);
                MotherDuck.gameObject.GetComponent<Animator>().SetBool("Succes", true);
                yield return new WaitForSeconds(ShowTime / 1.5f);

                StartCoroutine(AnimatorSet(DuckState.Idle));
                break;

            case 3:
                MotherDuck.gameObject.GetComponent<Animator>().SetBool("Idle", false);
                MotherDuck.gameObject.GetComponent<Animator>().SetBool("False", false);
                MotherDuck.gameObject.GetComponent<Animator>().SetBool("Succes", true);
                break;
        }
    }

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
