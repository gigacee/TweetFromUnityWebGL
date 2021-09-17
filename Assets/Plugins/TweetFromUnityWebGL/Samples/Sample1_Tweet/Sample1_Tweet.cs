using UnityEngine;
#if !UNITY_EDITOR && UNITY_WEBGL
using System.Runtime.InteropServices;
#endif

namespace TweetFromUnityWebGL.Samples
{
    public class Sample1_Tweet : MonoBehaviour
    {
        [SerializeField] private string _tweetMessage;

#if !UNITY_EDITOR && UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern string TweetFromUnity(string rawMessage);
#endif

        public void Tweet()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            TweetFromUnity(_tweetMessage);
            return;
#endif

            Application.OpenURL($"https://twitter.com/intent/tweet?text={_tweetMessage}");
        }
    }
}
