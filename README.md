# Tweet from Unity WebGL

Mobile-ready script for tweeting from Unity WebGL.

## Basic Usage

This script uses [.jslib plugin](https://docs.unity3d.com/Manual/webgl-interactingwithbrowserscripting.html) in `Assets/Plugins/WebGL/` . You can tweet from WebGL by calling `TweetFromUnity()` in .jslib as follows:

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

`Demo1_Tweet` scene is an example of it.

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

`Demo2_TweetWithScreenshot` scene is an example of it.

## Special Characters

- line-break : `%0a`
- hashtag (#) : `%23`

Example:

`TweetFromUnity("You can include line-breaks%0aand hashtags in your tweet message!%0a%0a%23TweetFromUnityWebGL");`

↓

```
You can include line-breaks
and hashtags in your tweet message!

#TweetFromUnityWebGL
```

## Installation

All you have to do is copy `/Assets/Plugins/WebGL/plugins.jslib` to your project.

:warning: Be sure to place it in the same directory hierarchy.
