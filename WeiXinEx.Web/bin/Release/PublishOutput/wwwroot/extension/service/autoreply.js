var autoReply = {
    init: function () {
        //获取自动回复相关配置
        plug.getSettings(function (result) {
            var config = result.config;
            client.reply_list = config.fastreply_list;
            client.public_reply_list = result.public_reply.fastreply_list;
        });
    },
    sendReply: function (sessions) {
        sessions.forEach(function (session) {
            plug.sendReply(session, 0, autoReply.sendCompleted);
        });
    },
    sendCompleted: function (session, replyIndex, result)
    {
        replyIndex++;
        var max = client.settings.auto_count || 4;
        if (replyIndex < max)
            plug.sendReply(session, replyIndex, autoReply.sendCompleted);
    }
};

$.extend(plug, {
    sendReply: function (session, replyIndex, callback) {
        if (client.reply_list.length <= replyIndex) {
            console.log("自动回复失败,消息索引有误" + replyIndex);
            return;
        }
        var reply = client.reply_list[replyIndex];
        var data = {
            isautoreply: 0,
            isfastreply: 1,
            sessionid: session.sessionid,
            fans_openid: session.openid,
            msgtype: reply.msgtype,
            content: reply.content
        };
        data.clientmsgid = window.btoa([data.fans_openid, client.current.kfwx, Date.now(), Math.floor(1e7 * Math.random())].join("|"));
        plug.post("/cgi-bin/kfmsg?action=send", data, function (result) {
            if (callback) callback(session, replyIndex, result);
            
        });
    }
});
builder.register(autoReply.init, 20);