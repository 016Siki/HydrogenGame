using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class RankingDisplay : MonoBehaviour
{
    public GameObject rankingPanel;
    public GameObject rankingParamPrefab;
    public string url = "https://script.google.com/macros/s/AKfycbzucqxgMPkUzj-nlUn4P5WdFNG9J8XFl4gYcB619tCj0xgifHQC-kPweaqxOZs3H3YW7Q/exec";

    public void FetchRanking()
    {
        StartCoroutine(GetRanking());
    }

    private IEnumerator GetRanking()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                // レスポンスをそのまま確認
                string jsonResponse = request.downloadHandler.text;
                Debug.Log("Response: " + jsonResponse);

                // 必要ならばランキングリスト形式に変換
                if (!jsonResponse.StartsWith("{\"rankings\":"))
                {
                    jsonResponse = "{\"rankings\":" + jsonResponse + "}";
                }

                try
                {
                    RankingList rankingList = JsonUtility.FromJson<RankingList>(jsonResponse);

                    // RankingPanelをクリア
                    foreach (Transform child in rankingPanel.transform)
                    {
                        Destroy(child.gameObject);
                    }

                    // ランキングデータを表示
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
                    Debug.LogError("Error parsing JSON: " + e.Message);
                }
            }
            else
            {
                Debug.LogError("Error fetching ranking: " + request.error);
            }
        }
    }
}