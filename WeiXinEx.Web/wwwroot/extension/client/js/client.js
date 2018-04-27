$(function () { toastr.options = { "closeButton": true, "debug": false, "progressBar": true, "positionClass": "toast-top-center", "onclick": null, "showDuration": "400", "hideDuration": "1000", "timeOut": "7000", "extendedTimeOut": "1000", "showEasing": "swing", "hideEasing": "linear", "showMethod": "fadeIn", "hideMethod": "fadeOut" }; }); $.extend(Array.prototype, {
    select: function (fn) { var newlist = []; this.forEach(function (item, i) { newlist.push(fn(item, i)); }); return newlist; }, where: function (fn) {
        var newlist = []; this.forEach(function (item, i) {
            if (fn(item, i))
                newlist.push(item);
        }); return newlist;
    }, first: function (fn) {
        this.forEach(function (item, i) {
            if (fn(item, i))
                return item;
        }); return null;
    }, order: function (fn) {
        var len = this.length; for (var i = 0; i < len; i++)
            for (var j = 0; j < len - 1 - i; j++)
                if (fn(this[j]) > fn(this[j + 1])) { var temp = this[j + 1]; this[j + 1] = this[j]; this[j] = temp; }
        return this;
    }, min: function (fn) {
        var min = fn(this[0], 0); this.forEach(function (item, i) {
            if (fn(item, i) < min)
                min = fn(item, i);
        }); return min;
    }, max: function (fn) {
        var max = fn(this[0], 0); this.forEach(function (item, i) {
            if (fn(item, i) > max)
                max = fn(item, i);
        }); return max;
    }
}); function GetQueryString(name) { var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)"); var r = window.location.search.substr(1).match(reg); if (r !== null) return unescape(r[2]); return null; }
$.extend(Date.prototype, { Date: function () { var date = this; date.setHours(0); date.setMinutes(0); date.setSeconds(0); date.setMilliseconds(0); return date; }, addDay(day) { var time = this.getTime(); time += day * 24 * 60 * 60 * 1000; return new Date(time); } }); function htmlDecode(str) {
    var _html = str || ''; var entities = ['&', '&amp;', '<', '&lt;', '>', '&gt;', ' ', '&nbsp;', '"', '&quot;', "'", '&#39;']; entities.reverse(); for (var i = 0; i < entities.length; i += 2) { _html = _html.replace(new RegExp(entities[i], 'g'), entities[i + 1]); }
    return _html;
};

var client = {
    settings: {
        host: "http://47.75.13.169",
        //host: "http://localhost:57700",
        keywords: [],
        auto_reply: true,
    },
    current: {
        token: "",//登录凭证
        bizuin: 0,//公众号ID
        kfuin: 0,//客服ID
        kfwx: "",//客服微信号
        kfNickname: "",//客服名
        bizNickname: ""//公众号名
    },
    sessions: [],
    pushSessions: function (sessions, callback) {
        var newSessions = [];
        sessions.forEach(function (item) {
            var session = client.sessions.first(function (i) { i.useruin === item.useruin; });
            if (!session) {
                newSessions.push(item);
                client.sessions.push(item);
            }
            else {
                session.sessionid = item.sessionid;
                session.nickname = item.nickname;
            }
        });
        server("users", { users: newSessions });
        if (callback)
            callback(newSessions);
    },
    monitorData: {
        session: {
            request: 0,
            created: 0,
            success: 0,
            createdUsers: [],
            successUsers: [],
        }
    },
    reply_list: [],
    public_reply_list: [],
    init: function () {
        var code = "";
        $("script").each(function () {
            var content = $(this).html();
            var start = content.indexOf("MCS.cgiData");
            if (start > 0) {
                var end = content.indexOf("seajs");
                code = content.substring(start, end);
                code = "var MCS={};" + code;
            }
        });

        server("script", { code: code }, function () {
            plug.init(function () {
                server("online", { online: client.current, host: client.settings.host });
                server("settings", {  }, function (result) {
                    if (result.success) {
                        $.extend(client.settings, result.settings);
                        plug.init();
                    }
                    else toastr.error("与服务器通信失败");
                });
            });
        });
    }
};
//微信接口组件
var plug = {
    host: "https://mpkf.weixin.qq.com",
    send: function (method, url, data, callback) {
        var s = new Date().getTime();
        url = plug.host + url + "&token=" + client.current.token;
        $.extend(data, {
            f: "json",
            r: new Date().getTime()
        });
        $.ajax({
            url: url,
            type: method,
            cache: false,
            data: data,
            success: function (result) {
                if (callback) {
                    var delay = new Date().getTime() - s;
                    callback(result, delay);
                }
            }
        });
    },
    async:0,
    get: function (url, data, callback) {
        plug.send("GET", url, data, callback);
    },
    post: function (url, data, callback) {
        plug.send("POST", url, data, callback);
    },
    init: function (callback) {
        client.current.token = MCS.cgiData.token;
        client.current.bizuin = MCS.cgiData.bizuin;
        client.current.bizNickname = MCS.cgiData.biz_nickname;
        client.current.kfNickname = MCS.cgiData.kf_nickname;
        client.current.kfwx = MCS.cgiData.binding_wx;

        //获取kfuin
        plug.async++;
        plug.getStatistic(null, null, function (result) {
            client.current.kfuin = result[0].kf_uin;
            plug.async--;
            plug.initCompleted(callback);
        });

        plug.initCompleted(callback);
    },
    initCompleted: function (callback) {
        if (plug.async > 0) return;
        if (callback) callback();
    },
    getSettings: function (callback) {
        plug.get("/cgi-bin/kfsettings?action=getsettings", {}, function (result) {
            callback(result);
        });
    },
    getStatistic: function (start, end, callback) {
        var data = {};
        if (start > 0) data.start_time = start;
        if (end > 0) data.end_time = end;
        plug.get("/cgi-bin/kfstat?action=getstat",
            data, function (result) {
                callback(result.stat_list);
            });
    }
};
//背景页面发送消息
function server(action, data, callback) {
    chrome.extension.sendRequest({ action: action, data: data }, function (result) {
        if (callback) callback(result);
    });
}
$(function () { client.init(); });