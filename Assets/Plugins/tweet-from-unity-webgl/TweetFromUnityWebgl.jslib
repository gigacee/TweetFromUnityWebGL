mergeInto(LibraryManager.library, {
    TweetFromUnity: function (rawMessage) {
        var message = Pointer_stringify(rawMessage);
        var mobilePattern = /Android|iPhone|iPad|iPod/i;

        if (window.navigator.userAgent.search(mobilePattern) !== -1) {
            // Mobile
            location.href = "twitter://post?message=" + message;
        } else {
            // PC
            window.open("https://twitter.com/intent/tweet?text=" + message, "_blank");
        }
    },
});
