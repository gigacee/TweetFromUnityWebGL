using System;
using System.Collections;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace TweetFromUnityWebgl.Samples
{
    public class Sample2_TweetWithScreenshot : MonoBehaviour
    {
        [SerializeField] string tweetMessage;
        [SerializeField] string imgurClientId;

#if !UNITY_EDITOR && UNITY_WEBGL
        [System.Runtime.InteropServices.DllImport("__Internal")]
        static extern string TweetFromUnity(string rawMessage);
#endif

        public void TweetWithScreenshot()
        {
            StartCoroutine(TweetWithScreenshotCo());
        }

        IEnumerator TweetWithScreenshotCo()
        {
            yield return new WaitForEndOfFrame();

            var tex = ScreenCapture.CaptureScreenshotAsTexture();

            var wwwForm = new WWWForm();
            wwwForm.AddField("image", Convert.ToBase64String(tex.EncodeToJPG()));
            wwwForm.AddField("type", "base64");

            // Upload to Imgur
            var www = UnityWebRequest.Post("https://api.imgur.com/3/image.xml", wwwForm);
            www.SetRequestHeader("AUTHORIZATION", "Client-ID " + imgurClientId);

            yield return www.SendWebRequest();

            var uri = "";

            if (!www.isNetworkError)
            {
                var xDoc = XDocument.Parse(www.downloadHandler.text);
                uri = xDoc.Element("data")?.Element("link")?.Value;

                if (uri != null)
                {
                    // Remove ext
                    uri = uri.Remove(uri.Length - 4, 4);
                }
            }

#if !UNITY_EDITOR && UNITY_WEBGL
            TweetFromUnity($"{tweetMessage}%0a{uri}");
            yield break;
#endif

            Application.OpenURL($"https://twitter.com/intent/tweet?text={tweetMessage}%0a{uri}");
        }
    }
}
