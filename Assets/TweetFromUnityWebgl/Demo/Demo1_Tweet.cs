using UnityEngine;

namespace TweetFromUnityWebgl.Demo
{
    public class Demo1_Tweet : MonoBehaviour
    {
        [SerializeField] string tweetMessage;

#if !UNITY_EDITOR && UNITY_WEBGL
        [System.Runtime.InteropServices.DllImport("__Internal")]
        static extern string TweetFromUnity(string rawMessage);
#endif

        public void Tweet()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            TweetFromUnity(tweetMessage);
            return;
#endif

            Application.OpenURL($"https://twitter.com/intent/tweet?text={tweetMessage}");
        }
    }
}
