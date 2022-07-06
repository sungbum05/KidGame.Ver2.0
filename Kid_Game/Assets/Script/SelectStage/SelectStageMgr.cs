using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SelectStageMgr : MonoBehaviour
{
    [SerializeField]
    Camera MainCamera;

    bool StartGamechk = false;
    [SerializeField]
    Image FadePanel;
    [SerializeField]
    float ShowTiem = 1.0f;

    [Space(10)]
    [SerializeField]
    Vector3 StartMousePos;
    [SerializeField]
    Vector3 CurMousePos;

    [Space(10)]
    [SerializeField]
    BoxCollider2D CameraBorder;

    [Space(10)]
    [SerializeField]
    private GameObject SelectHouse;
    [SerializeField]
    LayerMask layerMask;

    [Space(10)]
    [SerializeField]
    float SceneMoveSpeed = 0;
    [SerializeField]
    bool OnOption = false;

    [Header("Buttons")]
    [SerializeField]
    private Button OptionBtn;

    [Header("OtherPanel")]
    [SerializeField]
    private GameObject OptionPan;

    private void Start()
    {
        SoundMgr.In.ChangeBGM("Rainbow_Forest_-_Quincas_Moreira");
        StartCoroutine(FadeIn());
    }

    // Update is called once per frame
    void Update()
    {
        if (StartGamechk == true && OnOption == false)
        {
            MouseEvent();
            CameraMove();
        }
    }

    void MouseEvent()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartMousePos = MainCamera.ScreenToWorldPoint(Input.mousePosition);

        }

        if (Input.GetMouseButton(0))
        {
            Vector2 MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            RaycastHit2D hit = Physics2D.Raycast(MousePos, transform.forward, 10.0f, layerMask);
            if (hit)
            {
                Debug.Log(hit.collider.name);
                SelectHouse = hit.collider.gameObject;
                SelectHouse.GetComponent<SpriteRenderer>().sprite = SelectHouse.GetComponent<ObjectImg>().ChangeImg;
            }

            CurMousePos = MainCamera.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (SelectHouse != null)
            {
                StartCoroutine(SceneLoad(SelectHouse.GetComponent<ObjectImg>().LoadSceneName));
                SelectHouse.GetComponent<SpriteRenderer>().sprite = SelectHouse.GetComponent<ObjectImg>().OriginalImg;
                SelectHouse = null;
            }

            StartMousePos = Vector2.zero;
            CurMousePos = Vector2.zero;
        }
    }

    IEnumerator SceneLoad(string SceneName)
    {
        yield return null;

        StartGamechk = false;
        FadePanel.DOFade(1, ShowTiem / 1.2f);
        yield return new WaitForSeconds(ShowTiem / 1.2f);

        if (SceneName != "None")
        {
            SceneManager.LoadScene(SceneName);
        }
    }

    IEnumerator FadeIn()
    {
        yield return null;
        Debug.Log("asdas");

        FadePanel.DOFade(0, ShowTiem / 1.2f);
        yield return new WaitForSeconds(ShowTiem / 1.2f);
        StartGamechk = true;
    }

    void CameraMove()
    {
        MainCamera.transform.position = Vector2.Lerp(MainCamera.transform.position, MainCamera.transform.position + (StartMousePos - CurMousePos), SceneMoveSpeed * Time.deltaTime);

        float x = Mathf.Clamp(Camera.main.transform.position.x, -CameraBorder.bounds.extents.x / 1.8f, CameraBorder.bounds.extents.x / 1.8f);
        float y = Mathf.Clamp(Camera.main.transform.position.y, -CameraBorder.bounds.extents.y / 1.8f, CameraBorder.bounds.extents.y / 1.8f);
        float z = Mathf.Clamp(Camera.main.transform.position.z, -10, -10);

        MainCamera.transform.position = new Vector3(x, y, z);
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
    #endregion
}
