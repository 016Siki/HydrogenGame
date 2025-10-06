using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;

[Serializable]
public class LaravelRankingData
{
    public int rank;
    public string name;
    public int score;
}

[Serializable]
public class LaravelRankingList
{
    public LaravelRankingData[] ranking;
    public int my_rank;
    public string updated_at; // コントローラが返す場合に備える（無くてもOK）
}

public class LaravelRankingDisplay : MonoBehaviour
{
    [Header("UI")]
    public GameObject rankingWindow;
    public Transform rankingPanel;               // ← GameObject ではなく Transform にすると楽
    public GameObject rankingParamPrefab;        // 子に Text が3つ（順位/名前/スコア）ある想定
    public GameObject loadingIndicator;
    public Text statusText;                      // エラーを出す欄

    [Header("設定")]
    public string rankingType = "total";         // "daily" | "monthly" | "total"
    public int? modeId = 1;                   // モード未指定なら null

    // ★本番は必ず https
    [SerializeField] private string apiUrl = "http://192.168.56.105:8000/api/ranking";

    private void Start()
    {
        if (rankingWindow != null) rankingWindow.SetActive(false);
    }

    public void PushRankingButton()
    {
        if (rankingWindow != null) rankingWindow.SetActive(true);
        FetchRanking();
    }

    public void PushRankingButtonExit()
    {
        if (rankingWindow != null) rankingWindow.SetActive(false);
    }

    public void FetchRanking()
    {
        StartCoroutine(GetRanking(rankingType, modeId));
    }

    // 外部UI（タブ/ドロップダウン）から呼べる切替
    public void SetRankType(string type)
    {
        rankingType = type;      // "daily" | "monthly" | "total"
        FetchRanking();
    }
    public void SetModeId(int id)
    {
        modeId = id;
        FetchRanking();
    }
    public void ClearModeId()
    {
        modeId = null;
        FetchRanking();
    }

    private IEnumerator GetRanking(string rankType, int? modeIdParam)
    {
        if (loadingIndicator) loadingIndicator.SetActive(true);
        ClearPanel();

        // 値を正規化（空や不正は total にフォールバック）
        rankType = string.IsNullOrWhiteSpace(rankType) ? "total" : rankType.Trim().ToLowerInvariant();
        if (rankType != "daily" && rankType != "monthly" && rankType != "total") rankType = "total";

        // URLを安全に組み立て（// や ? の事故防止）
        string baseUrl = apiUrl.TrimEnd('/');
        string url = $"{baseUrl}?rank_type={UnityWebRequest.EscapeURL(rankType)}";
        if (modeIdParam.HasValue) url += $"&modeid={modeIdParam.Value}";

        Debug.Log($"[Ranking] GET {url}");

        using (var req = UnityWebRequest.Get(url))
        {
            // 認証
            string enc = PlayerPrefs.GetString("token", "");
            if (!string.IsNullOrEmpty(enc))
            {
                string token = TokenCrypto.DecryptAES(enc);
                req.SetRequestHeader("Authorization", "Bearer " + token);
            }
            req.SetRequestHeader("Accept", "application/json");

            yield return req.SendWebRequest();

            if (loadingIndicator) loadingIndicator.SetActive(false);

            var code = (int)req.responseCode;
            var body = req.downloadHandler?.text ?? "";

            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.Log($"ランキング取得失敗: {code} {req.error}\nBody: {body}");
                if (code == 422 && body.Contains("rank type") && !string.IsNullOrEmpty(rankType))
                    Debug.Log("[Ranking] rank_type が届いていません。URLとサーバ側バリデーションを確認してください（下のチェックリスト参照）。");
                yield break;
            }

            string json = req.downloadHandler.text;
            if (string.IsNullOrWhiteSpace(json) || json[0] != '{')
            {
                Debug.Log("[Ranking] JSONではない応答:\n" + json);
                if (statusText) statusText.text = "取得失敗(不正な応答)";
                yield break;
            }

            LaravelRankingList list = null;
            try
            {
                list = JsonUtility.FromJson<LaravelRankingList>(json);
            }
            catch (Exception e)
            {
                Debug.Log("[Ranking] JSONパース失敗: " + e + "\nBODY:\n" + json);
                if (statusText) statusText.text = "取得失敗(JSONエラー)";
                yield break;
            }

            // UIへ反映
            if (list != null && list.ranking != null)
            {
                foreach (var data in list.ranking)
                {
                    var go = Instantiate(rankingParamPrefab, rankingPanel);
                    var texts = go.GetComponentsInChildren<Text>(true); // rank/name/score の順で3つ想定
                    if (texts.Length >= 3)
                    {
                        texts[0].text = data.rank.ToString();
                        texts[1].text = string.IsNullOrEmpty(data.name) ? "user" : data.name;
                        texts[2].text = data.score.ToString();
                    }
                }
            }

            if (statusText)
            {
                string updated = !string.IsNullOrEmpty(list?.updated_at) ? list.updated_at : DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                string myRankText = (list != null && list.my_rank > 0) ? $" / My: {list.my_rank}" : "";
                statusText.text = $"Type: {rankType}{(modeIdParam.HasValue ? $" (mode {modeIdParam.Value})" : "")} / Updated: {updated}{myRankText}";
            }
        }
    }
    private void ClearPanel()
    {
        if (rankingPanel == null) return;
        for (int i = rankingPanel.childCount - 1; i >= 0; i--)
        {
            Destroy(rankingPanel.GetChild(i).gameObject);
        }
    }
}