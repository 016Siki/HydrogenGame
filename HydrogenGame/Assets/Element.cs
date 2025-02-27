using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Element : MonoBehaviour
{
    //識別用ID
    public string ID;

    //選択状態の画像
    public GameObject _SelectSprite;

    //選択状態
    public bool IsSelect { get; private set; }


    /// <summary>
    /// MouseDownイベント
    /// </summary>
    private void OnMouseDown()
    {
        GameManager.Instance.ElementDown(this);
    }

    /// <summary>
    /// MouseEnterイベント
    /// </summary>
    private void OnMouseEnter()
    {
        GameManager.Instance.ElementEnter(this);
    }

    /// <summary>
    /// MouseUpイベント
    /// </summary>
    private void OnMouseUp()
    {
        GameManager.Instance.ElementUp();
    }

    /// <summary>
    /// 選択状態の設定
    /// </summary>
    /// <param name="isSelect"></param>
    public void SetIsSelect(bool isSelect)
    {
        IsSelect = isSelect;
        _SelectSprite.SetActive(isSelect);
    }
}
