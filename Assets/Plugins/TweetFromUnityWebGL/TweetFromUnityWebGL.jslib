mergeInto(LibraryManager.library, {
    TweetFromUnity: function (rawMessage) {
        var message = Pointer_stringify(rawMessage);
        var mobilePattern = /android|iphone|ipad|ipod/i;

        var ua = window.navigator.userAgent.toLowerCase();

        if (ua.search(mobilePattern) !== -1 || (ua.indexOf("macintosh") !== -1 && "ontouchend" in document)) {
            // Mobile
            location.href = "twitter://post?message=" + message;
        } else {
            // PC
            window.open("https://twitter.com/intent/tweet?text=" + message, "_blank");
        }
    },
});
