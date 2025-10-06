using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;

public class ScoreSender : MonoBehaviour
{
    public InputField nameInput;
    public Text scoreText;
    [SerializeField] private GameObject setButton;
    [SerializeField] private Text setStatusText;
    private bool rankingSet;

    [SerializeField] private string apiUrl = "http://192.168.56.105:8000/api/scores";
    [SerializeField] private int modeId = 1; // 取得側と必ず一致させる

    public void SendScore()
    {
        if (rankingSet) return;

        if (string.IsNullOrEmpty(PlayerPrefs.GetString("token", "")))
        {
            if (setStatusText) setStatusText.text = "ログインが必要です";
            return;
        }

        if (!int.TryParse(scoreText.text, out int scoreValue))
        {
            if (setStatusText) setStatusText.text = "スコアが数値ではありません";
            return;
        }

        string playerName = string.IsNullOrWhiteSpace(nameInput?.text) ? "NoName" : nameInput.text;

        StartCoroutine(PostScore(playerName, scoreValue));
        if (setStatusText) setStatusText.text = "登録中...";
        rankingSet = true;
        if (setButton) setButton.SetActive(false);
    }

    private IEnumerator PostScore(string name, int score)
    {
        var form = new WWWForm();
        // ← サーバが name をバリデーションしていないなら送らなくてもOK
        form.AddField("name", name);
        form.AddField("score", score);
        form.AddField("modeid", modeId);

        using (UnityWebRequest request = UnityWebRequest.Post(apiUrl.TrimEnd('/'), form))
        {
            string encToken = PlayerPrefs.GetString("token", "");
            string token = TokenCrypto.DecryptAES(encToken);
            request.SetRequestHeader("Authorization", "Bearer " + token);
            request.SetRequestHeader("Accept", "application/json");
            request.timeout = 15;

            Debug.Log($"[Score] POST {apiUrl} score={score} modeid={modeId}");
            yield return request.SendWebRequest();

            var code = (int)request.responseCode;
            var body = request.downloadHandler?.text ?? "";

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"スコア送信成功: {code} {body}");
                if (setStatusText) setStatusText.text = "登録完了";
            }
            else
            {
                Debug.Log($"送信失敗: {code} {request.error}\nBody: {body}");
                if (setStatusText) setStatusText.text = (code == 401) ? "認証エラー(再ログイン)" :
                                                          (code == 422) ? "入力エラー(ログ確認)" :
                                                          $"送信失敗({code})";
                rankingSet = false;              // リトライ可能に戻す
                if (setButton) setButton.SetActive(true);
            }
        }
    }
}
