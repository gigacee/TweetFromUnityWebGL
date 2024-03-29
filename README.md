# Tweet from Unity WebGL

Mobile-ready script for tweeting from Unity WebGL.

## 日本語による説明 / Explanation in Japanese

[モバイルにも対応した、Unity WebGL からツイートができるスクリプトを公開しました](https://blog.gigacreation.jp/entry/2020/10/04/223712)

## Demo

[Demo Page](https://www.gigacreation.jp/tweetfromunitywebgl/)

## Basic Usage

This script uses [.jslib plugin](https://docs.unity3d.com/Manual/webgl-interactingwithbrowserscripting.html). You can tweet from WebGL by calling `TweetFromUnity()` in `TweetFromUnityWebGL.jslib` as follows:

```cs
using UnityEngine;
#if !UNITY_EDITOR && UNITY_WEBGL
using System.Runtime.InteropServices;
#endif

public class Demo1 : MonoBehaviour
{
#if !UNITY_EDITOR && UNITY_WEBGL
    [DllImport("__Internal")]
    private static extern string TweetFromUnity(string rawMessage);
#endif

    public void Tweet()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        TweetFromUnity("Tweet Message");
#endif
    }
}
```

When you run your game on PC, twitter.com will be opened in your browser by this JavaScript code:

```js
window.open("https://twitter.com/intent/tweet?text=" + message, "_blank");
```

When mobile, the twitter app is launched by this:

```js
location.href = "twitter://post?message=" + message;
```

:warning: If the twitter app is not installed in your mobile, this script won't work.

`Sample1_Tweet` scene is an example of it.

## Tweet with Screenshot

You can also tweet with a screenshot of your game. Here's an example of using Imgur:

```cs
// Original code from https://github.com/ttyyamada/TweetWithScreenShotInWebGL
// Licensed under https://github.com/ttyyamada/TweetWithScreenShotInWebGL/blob/master/LICENSE

using System;
using System.Collections;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Networking;
#if !UNITY_EDITOR && UNITY_WEBGL
using System.Runtime.InteropServices;
#endif

public class Demo2 : MonoBehaviour
{
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

            // Remove Ext
            uri = uri?.Remove(uri.Length - 4, 4);
        }

#if !UNITY_EDITOR && UNITY_WEBGL
        TweetFromUnity($"Tweet Message%0a{uri}");
#endif
    }
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

`https://github.com/gigacee/TweetFromUnityWebGL.git?path=Assets/Plugins/TweetFromUnityWebGL`

### Manual

Copy `Assets/Plugins/TweetFromUnityWebGL/TweetFromUnityWebGL.jslib` to your project.

:warning: Be sure to put it in `Assets/Plugins/` .
