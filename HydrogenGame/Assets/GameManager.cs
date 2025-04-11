using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    #region 変数
    // シングルトンインスタンス
    public static GameManager Instance { get; private set; }

    private ObjectManager _objectManager;

    // 選択中のID
    private string _SelectID = "";

    private List<Element> _Elements = new List<Element>();

    private AudioSource _audioSource;

    public AudioClip DestroySE;
    public AudioClip StartSE;
    public AudioClip FinishSE;

    // 元素のPrefabリスト
    public GameObject[] ElementPrefabs;

    public GameObject StandPanel;
    public GameObject ResultPanel;
    public GameObject OptionPanel;
    public GameObject APIPanel;

    public Text Count;

    public Text timer;

    public Text Score;
    public Text GameScore;
    public Text h2so4Score;
    public Text nh3Score;
    public Text h2oScore;
    public Text h2Score;
    public Text o2Score;
    public Text n2Score;

    //ラインレンダラー
    public LineRenderer LineRenderer;

    // 消すために必要な数
    public int ElementDestroy = 2;

    // 繋げられる範囲
    public float ElementRange = 1.5f;

    // 最大選択数
    public int MaxSelectionCount = 7;

    private float CountDown = 4;
    private float Limit = 10;

    private int CountTimer;
    private int LimitTimer = 60;

    private bool isStart;
    private bool isFinish;
    private bool isOption;

    private int score = 0;
    private int h2so4 = 0;
    private int nh3 = 0;
    private int h2o = 0;
    private int h2 = 0;
    private int n2 = 0;
    private int o2 = 0;
    #endregion

    private void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
        isStart = false;
        isFinish = true;
        isOption = false;
    ResultPanel.SetActive(false);
        APIPanel.SetActive(false);
        _objectManager = FindObjectOfType<ObjectManager>();
        _audioSource = GetComponent<AudioSource>();

    }

    private void Update()
    {

        if(!isOption)
        {
            CountDown -= Time.deltaTime;
        }

        if (isStart)
        {
            Limit -= Time.deltaTime;
            LimitTimer = ((int)Limit);
        }
        CountTimer = ((int)CountDown);
        timer.text = LimitTimer.ToString();
        Count.text = CountTimer.ToString();
        GameScore.text = score.ToString();
        if(CountTimer == 0)
        {
            isStart = true;
            isFinish = false;
            _audioSource.PlayOneShot(StartSE);
            StandPanel.SetActive(false);
        }

        if (LimitTimer <= 0)
        {
            isFinish = true;
            isStart = false;
            //_audioSource.PlayOneShot(FinishSE);
            ResultPanel.SetActive(true);
            StartCoroutine(UpdateScore());
            
        }


        LineRendererUpdate();
    }

    private IEnumerator UpdateScore()
    {
        h2Score.text = h2.ToString();
        yield return new WaitForSeconds(0.5f);
        o2Score.text = o2.ToString();
        yield return new WaitForSeconds(0.5f);
        n2Score.text = n2.ToString();
        yield return new WaitForSeconds(0.5f);
        h2oScore.text = h2o.ToString();
        yield return new WaitForSeconds(0.5f);
        nh3Score.text = nh3.ToString();
        yield return new WaitForSeconds(0.5f);
        h2so4Score.text = h2so4.ToString();
        yield return new WaitForSeconds(0.5f);
        Score.text = score.ToString();
    }

    /// <summary>
    /// 選択してる場合に線を引く関数
    /// </summary>
    private void LineRendererUpdate()
    {
        if(_Elements.Count >= 2)
        {
            LineRenderer.positionCount = _Elements.Count;
            LineRenderer.SetPositions(_Elements.Select(_Elements => _Elements.transform.position).ToArray());
            LineRenderer.gameObject.SetActive(true);
        }
        else LineRenderer.gameObject.SetActive(false);
    }

    /// <summary>
    /// ElementDownイベント
    /// </summary>
    /// <param name="element"></param>
    public void ElementDown(Element element)
    {
        if(!isFinish)
        {
            _Elements.Add(element);
            element.SetIsSelect(true);

            _SelectID = element.ID;
        }
    }

    /// <summary>
    /// ElementEnterイベント
    /// </summary>
    /// <param name="element"></param>
    public void ElementEnter(Element element)
    {
        if (element.IsSelect)
        {
            // 要素が選択されている場合の処理
            if (_Elements.Count >= 2 && _Elements[_Elements.Count - 2] == element)
            {
                var removeElement = _Elements[_Elements.Count - 1];
                removeElement.SetIsSelect(false);
                _Elements.RemoveAt(_Elements.Count - 1);
            }
        }
        else
        {
            // 要素が選択されていない場合の処理
            if (_Elements.Count > 0 && _Elements.Count < MaxSelectionCount)
            {
                var length = (_Elements[_Elements.Count - 1].transform.position - element.transform.position).magnitude;
                if (length < ElementRange)
                {
                    _Elements.Add(element);
                    element.SetIsSelect(true);
                }
            }
        }
    }

    /// <summary>
    /// ElementUpイベント
    /// </summary>
    public void ElementUp()
    {
        // H2SO4の組み合わせをチェックして削除
        if (IsH2SO4Selected(out List<Element> h2so4Elements))
        {
            _audioSource.PlayOneShot(DestroySE);
            DestroyElement(h2so4Elements);
            _Elements.RemoveAll(e => h2so4Elements.Contains(e));
            Debug.Log("H2SO4");
            score += 500;
            h2so4++;
        }

        // NH3の組み合わせをチェックして削除
        if (IsNH3Selected(out List<Element> nh3Elements))
        {
            _audioSource.PlayOneShot(DestroySE);
            DestroyElement(nh3Elements);
            _Elements.RemoveAll(e => nh3Elements.Contains(e));
            Debug.Log("NH3");
            score += 150;
            nh3++;
        }

        // 複数のH2Oの組み合わせをチェックして削除
        while (IsH2OSelected(out List<Element> h2oElements))
        {
            _audioSource.PlayOneShot(DestroySE);
            DestroyElement(h2oElements);
            _Elements.RemoveAll(e => h2oElements.Contains(e));
            Debug.Log("H2O");
            score += 50;
            h2o++;
        }

        // 複数のH2の組み合わせをチェックして削除
        while (IsH2Selected(out List<Element> h2Elements))
        {
            _audioSource.PlayOneShot(DestroySE);
            DestroyElement(h2Elements);
            _Elements.RemoveAll(e => h2Elements.Contains(e));
            Debug.Log("H2");
            score += 10;
            h2++;
        }

        // 複数のO2の組み合わせをチェックして削除
        while (IsO2Selected(out List<Element> o2Elements))
        {
            _audioSource.PlayOneShot(DestroySE);
            DestroyElement(o2Elements);
            _Elements.RemoveAll(e => o2Elements.Contains(e));
            Debug.Log("O2");
            score += 10;
            o2++;
        }

        // 複数のN2の組み合わせをチェックして削除
        while (IsN2Selected(out List<Element> n2Elements))
        {
            _audioSource.PlayOneShot(DestroySE);
            DestroyElement(n2Elements);
            _Elements.RemoveAll(e => n2Elements.Contains(e));
            Debug.Log("N2");
            score += 10;
            n2++;
        }
        foreach (var element in _Elements)
        {
            element.SetIsSelect(false);
        }

        
        _SelectID = "";
        _Elements.Clear();
    }
    /// <summary>
    /// H2SO4が選択されているかを確認する関数
    /// </summary>
    /// <param name="h2so4Elements">H2Oの要素リストを返すための出力パラメータ</param>
    private bool IsH2SO4Selected(out List<Element> h2so4Elements)
    {
        h2so4Elements = new List<Element>();
        if (_Elements.Count >= 7)
        {
            List<Element> hElements = new List<Element>();
            List<Element> oElements = new List<Element>();
            List<Element> sElements = new List<Element>();

            foreach (var element in _Elements)
            {
                if (element.ID == "H")
                {
                    hElements.Add(element);
                }
                else if (element.ID == "O")
                {
                    oElements.Add(element);
                }
                else if (element.ID == "S")
                {
                    sElements.Add(element);
                }

            }

            if (hElements.Count >= 2 && oElements.Count >= 4 && sElements.Count >= 1)
            {
                h2so4Elements.Add(hElements[0]);
                h2so4Elements.Add(hElements[1]);
                h2so4Elements.Add(oElements[0]);
                h2so4Elements.Add(oElements[1]);
                h2so4Elements.Add(oElements[2]);
                h2so4Elements.Add(oElements[3]);
                h2so4Elements.Add(sElements[0]);
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// H2Oが選択されているかを確認する関数
    /// </summary>
    /// <param name="h2oElements">H2Oの要素リストを返すための出力パラメータ</param>
    private bool IsH2OSelected(out List<Element> h2oElements)
    {
        h2oElements = new List<Element>();

        // 要素の数が3つ未満の場合、H2Oの形成は不可能
        if (_Elements.Count < 3) return false;

        for (int i = 0; i < _Elements.Count - 2; i++)
        {
            // 'H', 'H', 'O' もしくは 'O', 'H', 'H' という並びを探す
            if ((_Elements[i].ID == "H" && _Elements[i + 1].ID == "H" && _Elements[i + 2].ID == "O") ||
                (_Elements[i].ID == "O" && _Elements[i + 1].ID == "H" && _Elements[i + 2].ID == "H") ||
                (_Elements[i].ID == "H" && _Elements[i + 1].ID == "O" && _Elements[i + 2].ID == "H"))
            {
                h2oElements.Add(_Elements[i]);
                h2oElements.Add(_Elements[i + 1]);
                h2oElements.Add(_Elements[i + 2]);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// NH3が選択されているかを確認する関数
    /// </summary>
    /// <param name="nh3Elements"></param>
    /// <returns></returns>
    private bool IsNH3Selected(out List<Element> nh3Elements)
    {
        nh3Elements = new List<Element>();

        // 要素の数が4つ未満の場合、NH3の形成は不可能
        if (_Elements.Count < 4) return false;

        for (int i = 0; i < _Elements.Count - 3; i++)
        {
            // 並びを探す
            if ((_Elements[i].ID == "N" && _Elements[i + 1].ID == "H" && _Elements[i + 2].ID == "H" && _Elements[i + 3].ID == "H") ||
                (_Elements[i].ID == "H" && _Elements[i + 1].ID == "N" && _Elements[i + 2].ID == "H" && _Elements[i + 3].ID == "H") ||
                (_Elements[i].ID == "H" && _Elements[i + 1].ID == "H" && _Elements[i + 2].ID == "N" && _Elements[i + 3].ID == "H") ||
                (_Elements[i].ID == "H" && _Elements[i + 1].ID == "H" && _Elements[i + 2].ID == "H" && _Elements[i + 3].ID == "N"))
            {
                nh3Elements.Add(_Elements[i]);
                nh3Elements.Add(_Elements[i + 1]);
                nh3Elements.Add(_Elements[i + 2]);
                nh3Elements.Add(_Elements[i + 3]);
                return true;
            }
        }
        return false;
    }


    /// <summary>
    /// H2が選択されているかを確認する関数
    /// </summary>
    /// <param name="h2Elements"></param>
    /// <returns></returns>
    private bool IsH2Selected(out List<Element> h2Elements)
    {
        h2Elements = new List<Element>();
        if (_Elements.Count >= 2)
        {
            List<Element> hElements = new List<Element>();

            foreach (var element in _Elements)
            {
                if (element.ID == "H")
                {
                    hElements.Add(element);
                }
            }

            if (hElements.Count >= 2)
            {
                h2Elements.Add(hElements[0]);
                h2Elements.Add(hElements[1]);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// O2が選択されているかを確認する関数
    /// </summary>
    /// <param name="o2Elements"></param>
    /// <returns></returns>
    private bool IsO2Selected(out List<Element> o2Elements)
    {
        o2Elements = new List<Element>();
        if (_Elements.Count >= 2)
        {
            List<Element> oElements = new List<Element>();

            foreach (var element in _Elements)
            {
                if (element.ID == "O")
                {
                    oElements.Add(element);
                }
            }

            if (oElements.Count >= 2)
            {
                o2Elements.Add(oElements[0]);
                o2Elements.Add(oElements[1]);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// O2が選択されているかを確認する関数
    /// </summary>
    /// <param name="n2Elements"></param>
    /// <returns></returns>
    private bool IsN2Selected(out List<Element> n2Elements)
    {
        n2Elements = new List<Element>();
        if (_Elements.Count >= 2)
        {
            List<Element> nElements = new List<Element>();

            foreach (var element in _Elements)
            {
                if (element.ID == "N")
                {
                    nElements.Add(element);
                }
            }

            if (nElements.Count >= 2)
            {
                n2Elements.Add(nElements[0]);
                n2Elements.Add(nElements[1]);
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// 元素を消す関数
    /// </summary>
    /// <param name="elements"></param>
    private void DestroyElement(List<Element> elements)
    {
        foreach (var element in elements)
        {
            
            _objectManager.activeItems.Remove(element.gameObject);
            _objectManager.ReleaseItem(element.gameObject);
            element.SetIsSelect(false);
            // 新しいアイテムを生成してリストに追加
            Vector3 spanPos = new Vector3(Random.Range(-2.5f, 2.5f), 7, 0);
            GameObject newItem = _objectManager.GetPoolItem(spanPos);
            _objectManager.activeItems.Add(newItem);
        }
    }

    /// <summary>
    /// オプションパネルを開くボタン
    /// </summary>
    public void OptionButton()
    {
        if(!isOption)
        {
            OptionPanel.SetActive(true);
            isOption = true;
            isFinish = true;
            StartCoroutine(AnimTime(0));
        }
        else
        {
            OptionPanel.SetActive(false);
            isOption = false;
            isFinish = false;
            Time.timeScale = 1;
        }
    }

    /// <summary>
    /// アニメーションを動かすためのコルーチン
    /// </summary>
    /// <param name="timescale"></param>
    /// <returns></returns>
    IEnumerator AnimTime(float timescale)
    {
        yield return new WaitForSeconds(0.5f);
        Time.timeScale = timescale;
    }

    public void BackTitleButton()
    {
        SceneManager.LoadScene("Title");
    }
}
