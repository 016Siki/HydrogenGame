using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class RankingDisplay : MonoBehaviour
{
    public GameObject rankingPanel;
    public GameObject rankingParamPrefab;
    public string url = "https://script.google.com/macros/s/AKfycbzKpGVsmQSSiLDdEIqvK9f1RokHO4qcadQZCB0ewxmOP00EO0DGbzCfUGI3S--IbLiS6g/exec";
    public GameObject loadingIndicator; //ローディング
    public GameObject RankingPanal;
    public RankingDisplay rankingDisplay;



    void Start()
    {
        // ここでは特に初期化処理は行っていませんが、必要に応じて追加できます
        RankingPanal.SetActive(false);
        // rankingDisplay が未設定なら探す
        if (rankingDisplay == null)
        {
            rankingDisplay = FindObjectOfType<RankingDisplay>();
            if (rankingDisplay == null)
            {
                Debug.Log("RankingDisplay が見つかりません！");
            }
        }
    }
    public void FetchRanking()
    {
        StartCoroutine(GetRanking());
    }
    public void PushRankingButton()
    {
        RankingPanal.SetActive(true);
        FetchRanking();
    }
    public void PushRankingButtonExit()
    {
        RankingPanal.SetActive(false);
    }
    private IEnumerator GetRanking()
    {
        // 1. RankingPanelの子供オブジェクトをすべて削除
        foreach (Transform child in rankingPanel.transform)
        {
            Destroy(child.gameObject);
        }

        // 2. ローディング表示をON
        if (loadingIndicator != null) loadingIndicator.SetActive(true);

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            // 3. ローディング表示をOFF
            if (loadingIndicator != null) loadingIndicator.SetActive(false);

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                Debug.Log("Response: " + jsonResponse);

                if (!jsonResponse.StartsWith("{\"rankings\":"))
                {
                    jsonResponse = "{\"rankings\":" + jsonResponse + "}";
                }

                try
                {
                    RankingList rankingList = JsonUtility.FromJson<RankingList>(jsonResponse);

                    // ★ここでの削除処理は不要になった
                    // foreach (Transform child in rankingPanel.transform) Destroy(child.gameObject);

                    foreach (RankingData data in rankingList.rankings)
                    {
                        GameObject param = Instantiate(rankingParamPrefab, rankingPanel.transform);
                        Text[] texts = param.GetComponentsInChildren<Text>();

                        if (texts.Length >= 3)
                        {
                            texts[0].text = data.rank.ToString();
                            texts[1].text = data.name;
                            texts[2].text = data.score.ToString();
                        }
                        else
                        {
                            Debug.LogWarning("RankingParamPrefabのTextコンポーネントが不足しています。");
                        }
                    }
                }
                catch (System.Exception e)
                {
                    Debug.Log("Error parsing JSON: " + e.Message);
                }
            }
            else
            {
                Debug.Log("Error fetching ranking: " + request.error);
            }
        }

        // 念のため最後にもOFF
        if (loadingIndicator != null) loadingIndicator.SetActive(false);
    }


}