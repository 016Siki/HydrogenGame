using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static System.Net.WebRequestMethods;

public class AccountLogin : MonoBehaviour
{
    [Header("UI入力欄")]
    [SerializeField] private InputField emailInput;
    [SerializeField] private InputField passwordInput;

    [Header("UI")]
    [SerializeField] private Text statusText;

    [SerializeField] private GameObject acountObject;
    [SerializeField] private GameObject gameStratObject;

    private string apiBaseUrl = "http://192.168.56.105:8000/api";

    [System.Serializable]
    public class TokenResponse { public string token; }

    private void Start()
    {
        StartCoroutine(AutoLogin());
    }

    public void PushLogOut()
    {
        StartCoroutine(Logout());
    }

    public void PushLogin()
    {
        string email = emailInput.text.Trim();
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            if (statusText) statusText.text = "メールとパスワードは\n必須です";
            return;
        }
        // 通信コルーチンを開始
        StartCoroutine(Login(email,password));
    }

    public void PushCreateAcount()
    {
            //string name = nameInput.text.Trim();
            string email = emailInput.text.Trim();
            string password = passwordInput.text;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                if (statusText) statusText.text = "メールとパスワードは\n必須です";
                return;
            }

            // 通信コルーチンを開始
            StartCoroutine(Register(email, password, name));
    }
 private IEnumerator Register(string email, string password, string name = null)
    {
        string url = apiBaseUrl.TrimEnd('/') + "/register";
        var form = new WWWForm();
        form.AddField("email", email);
        form.AddField("password", password);
        if (!string.IsNullOrEmpty(name)) form.AddField("name", name);

        using (var req = UnityWebRequest.Post(url, form))
        {
            req.SetRequestHeader("Accept", "application/json");
            yield return req.SendWebRequest();

            int code = (int)req.responseCode;
            string body = req.downloadHandler != null ? req.downloadHandler.text : "";

            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.Log($"[Register] HTTP {code} {req.error}\nBODY:\n{body}");
                if (statusText) statusText.text = $"エラー";
                yield break;
            }

            // 念のため本文の頭を確認
            if (string.IsNullOrWhiteSpace(body) || body[0] != '{')
            {
                Debug.Log($"[Register] JSONではないレスポンスを受信:\n{body}");
                if (statusText) statusText.text = "登録失敗(不正な応答)";
                yield break;
            }

            RegisterResponse res = null;
            try
            {
                res = JsonUtility.FromJson<RegisterResponse>(body);
            }
            catch (Exception e)
            {
                Debug.Log($"[Register] JSONパース失敗: {e}\nBODY:\n{body}");
                if (statusText) statusText.text = "登録失敗(JSONエラー)";
                yield break;
            }

            if (res == null || string.IsNullOrEmpty(res.token))
            {
                Debug.Log($"[Register] tokenが取得できませんでした。\nBODY:\n{body}");
                if (statusText) statusText.text = "登録失敗(tokenなし)";
                yield break;
            }

            PlayerPrefs.SetString("token", TokenCrypto.EncryptAES(res.token));
            Debug.Log("[Register] 登録成功");
            Debug.Log(res.token);
            if (statusText) statusText.text = "登録完了！";
            acountObject.SetActive(false);
            gameStratObject.SetActive(true);
        }
    }
    private IEnumerator Login(string email, string password)
    {
        string url = apiBaseUrl.TrimEnd('/') + "/login";
        var form = new WWWForm();
        form.AddField("email", email.Trim().ToLowerInvariant());
        form.AddField("password", password.Trim());

        using (var req = UnityWebRequest.Post(url, form))
        {
            req.SetRequestHeader("Accept", "application/json");
            yield return req.SendWebRequest();

            int code = (int)req.responseCode;
            string body = req.downloadHandler?.text ?? "";

            if (req.result != UnityWebRequest.Result.Success)
            {
                if (code == 401)
                {
                    if (statusText) statusText.text = "メールまたはパスワード\nが違います";
                }
                else
                {
                    if (statusText) statusText.text = $"ログイン失敗";
                }
                Debug.Log($"[Login] HTTP {code} {req.error}\nBODY:\n{body}");
                yield break;
            }

            var token = JsonUtility.FromJson<TokenResponse>(body)?.token;
            if (string.IsNullOrEmpty(token))
            {
                if (statusText) statusText.text = "ログイン失敗(tokenなし)";
                Debug.Log("[Login] token フィールドが見つかりません。\nBODY:\n" + body);
                yield break;
            }

            PlayerPrefs.SetString("token", TokenCrypto.EncryptAES(token));
            if (statusText) statusText.text = "ログイン成功！";
            Debug.Log("ログイン成功");
            Debug.Log(token);

            acountObject.SetActive(false);
            gameStratObject.SetActive(true);
        }
    }

    IEnumerator AutoLogin()
    {
        Debug.Log("オートlog in");
        if (PlayerPrefs.HasKey("token"))
        {
            var enc = PlayerPrefs.GetString("token");
            var token = TokenCrypto.DecryptAES(enc);

            using var req = UnityWebRequest.Get(apiBaseUrl + "/user");
            req.SetRequestHeader("Authorization", "Bearer " + token);
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("自動ログイン成功");
                acountObject.SetActive(false);
                gameStratObject.SetActive(true);
                yield break;
            }


            Debug.Log($"自動ログイン失敗: {req.responseCode} {req.error}");
        }
        acountObject.SetActive(true);
        gameStratObject.SetActive(false);
    }

    IEnumerator Logout()
    {
        if (!PlayerPrefs.HasKey("token")) yield break;

        string token = TokenCrypto.DecryptAES(PlayerPrefs.GetString("token"));
        string url = apiBaseUrl + "/logout";

        // 空JSONを送信
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes("{}");

        using (UnityWebRequest req = new UnityWebRequest(url, "POST"))
        {
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");
            req.SetRequestHeader("Authorization", "Bearer " + token);

            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("ログアウト成功");
                acountObject.SetActive(true);
                gameStratObject.SetActive(false);
                statusText.text = "アカウント\n登録";
                emailInput.text = "";
                passwordInput.text = "";
            }
            else
            {
                Debug.Log($"ログアウト失敗: {req.responseCode} {req.error}\nBody: {req.downloadHandler.text}");
            }
        }

        PlayerPrefs.DeleteKey("token");
    }

}
