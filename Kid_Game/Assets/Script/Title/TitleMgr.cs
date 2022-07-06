using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class TitleMgr : MonoBehaviour
{
    [SerializeField]
    private GameObject StartBtn;
    [SerializeField]
    private GameObject Title;
    [SerializeField]
    private GameObject FadePanel;

    [Space(10)]
    [SerializeField]
    Camera MainCamera;

    [Space(10)]
    [SerializeField]
    bool StartGamechk = false;
    [SerializeField]
    float ShowTiem = 1.0f;

    [Space(10)]
    [SerializeField]
    private Vector2 TitleOriginPos;
    [SerializeField]
    private float MoveDis = 30.0f;


    [Header("Buttons")]
    [SerializeField]
    private Button OptionBtn;
    [SerializeField]
    private Button ExitBtn;
    [SerializeField]
    private Button CloseBtn;
    [SerializeField]
    private Button CreditBtn;

    [Header("OtherPanel")]
    [SerializeField]
    private GameObject OptionPan;
    [SerializeField]
    private GameObject CreditPan;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(YetStartGame());

        StartBtn.GetComponent<Button>().onClick.AddListener(() =>
        {
            StartCoroutine(StartGame());
        });
    }

    private void Update()
    {

    }

    IEnumerator YetStartGame()
    {
        yield return null;
        TitleOriginPos = Title.GetComponent<RectTransform>().anchoredPosition;

        while(StartGamechk == false)
        {
            yield return null;

            Title.GetComponent<RectTransform>().DOAnchorPosY(TitleOriginPos.y + MoveDis, ShowTiem * 6);
            yield return new WaitForSeconds(ShowTiem * 1.5f);
            Title.GetComponent<RectTransform>().DOAnchorPosY(TitleOriginPos.y - MoveDis, ShowTiem * 6);
            yield return new WaitForSeconds(ShowTiem * 1.5f);
        }
    }

    IEnumerator StartGame()
    {
        StartGamechk = true;

        yield return null;
        FadePanel.SetActive(true);
        FadeOutObj();
        yield return new WaitForSeconds(ShowTiem/3);
        StartCoroutine(CameraFocusUp());
        yield return new WaitForSeconds(ShowTiem);
        FadeInObj();
        yield return new WaitForSeconds(ShowTiem);
        SceneManager.LoadScene("SelectStageScene");
    }

    void FadeOutObj()
    {
        Title.GetComponent<Image>().DOFade(0, ShowTiem/2);
        StartBtn.GetComponent<Image>().DOFade(0, ShowTiem/2);
        OptionBtn.transform.gameObject.GetComponent<Image>().DOFade(0, ShowTiem / 2);
        ExitBtn.transform.gameObject.GetComponent<Image>().DOFade(0, ShowTiem / 2);
        CreditBtn.transform.gameObject.GetComponent<Image>().DOFade(0, ShowTiem / 2);
    }

    void FadeInObj()
    {     
        FadePanel.GetComponent<Image>().DOFade(1, ShowTiem/1.2f);
    }

    IEnumerator CameraFocusUp()
    {
        yield return null;

        MainCamera.DOOrthoSize(2, ShowTiem / 1.3f);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;

#else
    Application.Quit();

#endif
    }

    public void OptionPanOnOff()
    {
        if (OptionPan.active)
            OptionPan.SetActive(false);

        else
            OptionPan.SetActive(true);
    }

    public void CreditPanOnOff()
    {
        if (CreditPan.active)
            CreditPan.SetActive(false);

        else
            CreditPan.SetActive(true);
    }
}
