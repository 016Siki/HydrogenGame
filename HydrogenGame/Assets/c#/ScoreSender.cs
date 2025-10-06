    using UnityEngine;
    using UnityEngine.Networking;
    using System.Collections;
    using UnityEngine.UI;

    public class LaravelScoreSender : MonoBehaviour
    {
        public InputField nameInput;
        public Text score; // TextŒ^‚ğ•Û
        private bool rankingSet;
        [SerializeField] private GameObject setbutton;
        [SerializeField] private Text setText;

        public void SendScore()
        {
            if (rankingSet == false)
            {
                string playerName = string.IsNullOrEmpty(nameInput.text) ? "NoName" : nameInput.text;
                string scoreText = score.text; // Text‚Ì’†g‚ğstring‚Æ‚µ‚Äæ“¾
                StartCoroutine(PostScore(playerName, scoreText)); // score.text‚ğstring‚Æ‚µ‚Ä‘—M
                setText.text = "“o˜^’†";
                rankingSet = true;
            }
        }

        private IEnumerator PostScore(string name, string score) // score‚ÍstringŒ^
        {
            string url = "https://script.google.com/macros/s/AKfycbzKpGVsmQSSiLDdEIqvK9f1RokHO4qcadQZCB0ewxmOP00EO0DGbzCfUGI3S--IbLiS6g/exec";
            WWWForm form = new WWWForm();
            form.AddField("name", name);
            form.AddField("score", score); // score‚ğstring‚Æ‚µ‚Ä‘—M

            using (UnityWebRequest request = UnityWebRequest.Post(url, form))
            {
                yield return request.SendWebRequest();
                if (request.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log("Score sent successfully!");
                    setbutton.SetActive(false);
                }
                else
                {
                    Debug.Log("Error sending score: " + request.error);
                }
            }
        }
    }
