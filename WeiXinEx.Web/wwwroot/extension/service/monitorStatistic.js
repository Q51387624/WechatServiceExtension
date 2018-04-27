/**
 * 客服统计监视器
 */
var monitorStatistic = {
    timer:null,
    init: function () {
        monitorStatistic.sendRange(client.settings.monitorStatisticRange||30);//发送30天内统计数据
        monitorStatistic.start();//启动循环发送当天统计数据
    },
    start: function () {
        var today = new Date().Date();
        var next = today.addDay(1);
        var start = today.getTime() / 1000;
        var end = next.getTime() / 1000;
        monitorStatistic.timer = setInterval(() => {
            monitorStatistic.sendData(start, end);
        }, client.settings.monitorStatisticInterval || 30000);
    },
    sendRange: function (range) {
        var today = new Date().Date();
        var prev = today.addDay(-range);
        var start = prev.getTime() / 1000;
        var end = today.getTime() / 1000;
        monitorStatistic.sendData(start, end);
    },
    sendData: function (start, end) {
        plug.getStatistic(start, end, function (result) {
            var data = [];
            for (var i = 0; i < result.length; i++) {
                var item = result[i];
                data.push({
                    date: item.date,
                    kf_uin: item.kf_uin,
                    bizuin: client.current.bizuin,
                    online_time: item.stat_data.online_time,
                    session_count: item.stat_data.session_count,
                    msg_send: item.stat_data.msg_send,
                    msg_recv: item.stat_data.msg_recv
                });
            }
            var sessions = client.monitorData.session;
            client.monitorData.session = {
                request: 0,
                created: 0,
                success: 0,
                createdUsers: [],
                successUsers: [],
            };
            var data = {
                bizuin: client.current.bizuin,
                kfuin: client.current.kfuin,
                statistics: data,
                sessions: sessions
            };
            console.log(data);
            server("statistic", data);
        });
    }
};
$.extend(plug, {
    getStatistic: function (start, end, callback) {
        var data = {};
        if (start > 0) data.start_time = start;
        if (end > 0) data.end_time = end;
        plug.get("/cgi-bin/kfstat?action=getstat",
            data, function (result) {
                callback(result.stat_list);
            });
    }
});
builder.register(monitorStatistic.init, 10);