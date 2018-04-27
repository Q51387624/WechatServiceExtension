chrome.extension.onRequest.addListener(
    function (request, sender, sendResponse) {
        var action = request.action;
        var data = request.data;
        var callback = sendResponse;
        if (action === "script") {
            chrome.tabs.executeScript(sender.tab.id, { code: data.code }, function () {
                if (callback) callback();
            });
            return;
        }
        server[action](data, callback, sender);
    });
