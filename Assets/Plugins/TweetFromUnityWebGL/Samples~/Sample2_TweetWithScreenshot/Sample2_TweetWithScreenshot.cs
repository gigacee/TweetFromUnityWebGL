using System;
using System.Collections;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Networking;
#if !UNITY_EDITOR && UNITY_WEBGL
using System.Runtime.InteropServices;
#endif

namespace TweetFromUnityWebGL.Samples
{
    public class Sample2_TweetWithScreenshot : MonoBehaviour
    {
        [SerializeField] private string _tweetMessage;
        [SerializeField] private string _imgurClientId;

#if !UNITY_EDITOR && UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern string TweetFromUnity(string rawMessage);

#endif

        public void TweetWithScreenshot()
        {
            StartCoroutine(TweetWithScreenshotCo());
        }

        private IEnumerator TweetWithScreenshotCo()
        {
            yield return new WaitForEndOfFrame();

            Texture2D tex = ScreenCapture.CaptureScreenshotAsTexture();

            var wwwForm = new WWWForm();
            wwwForm.AddField("image", Convert.ToBase64String(tex.EncodeToJPG()));
            wwwForm.AddField("type", "base64");

            // Upload to Imgur
            UnityWebRequest www = UnityWebRequest.Post("https://api.imgur.com/3/image.xml", wwwForm);
            www.SetRequestHeader("AUTHORIZATION", "Client-ID " + _imgurClientId);

            yield return www.SendWebRequest();

            var uri = "";

            if (!www.isNetworkError)
            {
                XDocument xDoc = XDocument.Parse(www.downloadHandler.text);
                uri = xDoc.Element("data")?.Element("link")?.Value;

                // Remove ext
                uri = uri?.Remove(uri.Length - 4, 4);
            }

#if !UNITY_EDITOR && UNITY_WEBGL
            TweetFromUnity($"{_tweetMessage}%0a{uri}");
            yield break;
#endif

            Application.OpenURL($"https://twitter.com/intent/tweet?text={_tweetMessage}%0a{uri}");
        }
    }
}
