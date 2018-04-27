/**
 * 聊天记录监视器
 */
var monitorMessages = {
    init: function () {
        var sessions = MCS.cgiData.sessions.kfsession_and_msgsummary
            .select(function (item) {
            return {
                useruin: item.kfsession.fans_uin,
                nickname: item.kfsession.nickname,
                openid: item.kfsession.fans_openid,
                sessionid: item.kfsession.session_id,
                msgid: item.msgsummary.msg.msgid
            };
        });
        client.pushSessions(sessions, function (newlist) {
            newlist.forEach(function (item) {
                monitorMessages.sendData(item.useruin, item.msgid + 1);
            });
        });
    },
    sendData: function (useruin, msgid, size, next) {
        size = size || 20;
        plug.getFansMessageList(useruin, msgid, size, function (messages) {
            var nextSend = next || true;
            if (messages.length === 0) return;//无后续消息
            if (messages.length < size)
                nextSend = false;//消息已查询完毕

            var data = messages.select(function (item) {
                return {
                    msgid: item.msgid,
                    bizuin: item.bizuin,
                    useruin: item.useruin,
                    kfuin: item.kfuin,
                    createtime: item.createtime,
                    content: item.content,
                    msgtype: item.msgtype,
                    type: item.type,
                    kf_openid: item.kf_openid
                };
            });
            server("messages", { messages: data }, function (result) {
                if (result.count < data.length)
                    nextSend = false;//提交的消息已覆盖
                if (nextSend) {
                    var min = data.min(function (i) { return i.msgid; });
                    monitorMessages.sendData(useruin, min);
                }
            });
        });
    }
};

$.extend(plug, {
    //获取客户聊天记录
    getFansMessageList: function (user, msgid, count, callback) {
        var data = { fansuin: user, count: count || 20 };
        if (msgid > 0) data.msgid = msgid;
        plug.get("/cgi-bin/kfmsg?action=getfansmsglist",
            data, function (result) {
                callback(result.msglist);
            });
    }
});



builder.register(monitorMessages.init, 10);