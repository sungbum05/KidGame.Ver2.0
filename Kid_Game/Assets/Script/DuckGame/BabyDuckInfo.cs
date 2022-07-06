using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BabyDuckInfo : MonoBehaviour
{
    public string BabyColor;

    [SerializeField]
    private List<Sprite> ColorBabyDuck; // 애기 오리 컬러 별 이미지

    public Dictionary<string, Sprite> ColorBabyDuckDic = new Dictionary<string, Sprite>();

    private void Awake()
    {
        DicSetting();
    }

    void DicSetting()
    {
        foreach (Sprite Img in ColorBabyDuck)
        {
            string[] NameSplit = Img.name.Split('_');
            //Debug.Log(NameSplit[1]);
            ColorBabyDuckDic.Add(NameSplit[1], Img);
        }
    }

    public void ColorSetting()
    {
        Debug.Log(ColorBabyDuckDic[BabyColor].name);
        this.gameObject.GetComponent<Image>().sprite = ColorBabyDuckDic[BabyColor];
    }
}
