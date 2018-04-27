/**
 * 客户接入监视器
 */
var monitorSession = {
    isEnable: true,
    timer: null,
    data: { request: 0, users: 0, success: 0 },
    start: function () {
        if (!monitorSession.isEnable) return;
        if (monitorSession.timer) return;
        if (client.settings.monitorInterval > 0)
            monitorSession.timer = setInterval(monitorSession.unAccepted, client.settings.monitorInterval);
        else
            monitorSession.unAccepted();
    },
    unAccepted: function ()
    {
        plug.getUnAccepted(function (data, delay) {
            client.monitorData.session.request++;
            if (data.length > 0)
            {
                var users = data.select(function (item) { return item.msg.useruin; });
                if (users.length > 1)
                    plug.createSession(users.join(","), true, monitorSession.created);
                else
                    plug.createSession(users[0], false, monitorSession.created);
                client.monitorData.session.created += users.length;
                data.forEach(function (item) {
                    client.monitorData.session.createdUsers.push({
                        useruin: item.msg.useruin,
                        time: item.msg.createtime,
                        delay: delay
                    });
                });
            } else
                monitorSession.start();
        });
    },
    created: function (result, delay)
    {
        monitorSession.start();
        var createsessions = result.kfcreatesession_resp.where(function (i) { return i.base_resp.ret === 0; });
        if (createsessions.length === 0) return;
        toastr.success("接入新客户!"); 
        client.monitorData.session.success += createsessions.length;
        var sessions = createsessions.select(function (item) {
            return {
                useruin: item.kfsession.fans_uin,
                nickname: item.kfsession.nickname,
                openid: item.kfsession.fans_openid,
                sessionid: item.kfsession.session_id,
                msgid: item.msgsummary.msg.msgid,
                time: item.kfsession.start_time
            };
        });
        client.pushSessions(sessions);
        sessions.forEach(function (item) {
            client.monitorData.session.successUsers.push({
                useruin: item.useruin,
                time: item.time,
                delay: delay
            });
        });
        if (client.settings.auto_reply && autoReply)
            autoReply.sendReply(sessions);
    },
    enable: function () {
        monitorSession.isEnable = true;
        monitorSession.start();
    },
    disable: function () {
        monitorSession.isEnable = false;
        if (monitorSession.timer) {
            clearInterval(monitorSession.timer);
            monitorSession.timer = null;
        }
    },
    init: function ()
    {
        //监视器开关
        var box = '<div class="state" style="top:-100px;top:-50px">\
                   <input class="tgl tgl-skewed" id="chkMonitor" type="checkbox" />\
                   <label class="tgl-btn" data-tg-off="关" data-tg-on="开" for="chkMonitor">\
                   </label></div>';
        $(".main_header").append(box);
        $(".main_content").css("overflow", "visible");
        $("#chkMonitor").change(function () {
            var val = $(this).is(":checked");
            if (val) monitorSession.enable();
            else monitorSession.disable();
        });
    }
};

$.extend(plug, {
    getUnAccepted: function (callback) {
        var data = {
            count: 5,
            direct: 1,
            dayoption: 1
        };
        plug.get("/cgi-bin/kfunaccepted?action=getmsglist",
            data, function (result, delay) {
                if (result.base_resp.ret > 0) callback([], delay);
                else callback(result.msglist, delay);
            });
    },
    createSession: function (users, isAll, callback) {
        var data = {
            fansuinlist: users,
            isselectall: isAll ? 1 : 0,
            isauto: 0
        };
        plug.post("/cgi-bin/kfunaccepted?action=kfbatchcreatesession",
            data, function (result, delay) {
                callback(result, delay);
            });
    }
});

builder.register(monitorSession.init, 10);
