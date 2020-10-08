# Tweet from Unity WebGL

Mobile-ready script for tweeting from Unity WebGL.

## 日本語による説明 / Explanation in Japanese

[モバイルにも対応した、Unity WebGL からツイートができるスクリプトを公開しました](https://blog.gigacreation.jp/entry/2020/10/04/223712)

## Basic Usage

This script uses [.jslib plugin](https://docs.unity3d.com/Manual/webgl-interactingwithbrowserscripting.html). You can tweet from WebGL by calling `TweetFromUnity()` in `TweetFromUnityWebgl.jslib` as follows:

```cs
#if !UNITY_EDITOR && UNITY_WEBGL
    [System.Runtime.InteropServices.DllImport("__Internal")]
    static extern string TweetFromUnity(string rawMessage);
#endif

    public void Tweet()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        TweetFromUnity("Tweet Message");
        return;
#endif
    }
```

When you run the game on PC, twitter.com will be opened in your browser by this JavaScript:

```js
window.open("https://twitter.com/intent/tweet?text=" + message, "_blank");
```

When mobile, the twitter app is launched by this:

```js
location.href = "twitter://post?message=" + message;
```

:warning: If the twitter app is not installed in your mobile, this script won't work.

`Sample1_Tweet` scene is an example of it.

## Tweet with screenshot

You can also tweet with screenshot of the game. Here's an example of using Imgur:

```cs
    [SerializeField] string imgurClientId;

#if !UNITY_EDITOR && UNITY_WEBGL
    [System.Runtime.InteropServices.DllImport("__Internal")]
    static extern string TweetFromUnity(string rawMessage);
#endif

    public IEnumerator TweetWithScreenshot()
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
        TweetFromUnity($"Tweet Message%0a{uri}");
        yield break;
#endif
    }
```

`Sample2_TweetWithScreenshot` scene is an example of it.

## Special Characters

- line-break : `%0a`
- hashtag (#) : `%23`

Example:

`TweetFromUnity("You can include line-breaks%0aand hashtags in your tweet message!%0a%0a%23TweetFromUnityWebGL");`

↓

```plaintext
You can include line-breaks
and hashtags in your tweet message!

#TweetFromUnityWebGL
```

## Installation

### Package Manager

`https://github.com/Gigacee/tweet-from-unity-webgl.git?path=Assets/Plugins/tweet-from-unity-webgl`

### Manual

Copy `Assets/Plugins/tweet-from-unity-webgl/TweetFromUnityWebgl.jslib` to your project.

:warning: Be sure to put it in `Assets/Plugins/` .
