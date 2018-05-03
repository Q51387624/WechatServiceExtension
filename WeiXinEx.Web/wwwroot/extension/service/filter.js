/**
 * 关键词过滤器
 */
var filter = {
    keywords: client.settings.keywords,
    init: function () {
        //关键字屏蔽
        $("body").on("keydown", "#editor", function (e) {
            var text = $("#editor").text();
            if (e.which === 13 && text) {
                var keyword = filter.match(text);
                if (keyword) {
                    toastr.error("含有禁止关键字[" + keyword + "]!");
                    return false;//包含关键字 不予发送
                }

                //获取当前用户ID
                var id = $(".card_selected").data("reactid");
                var index = id.indexOf("$") + 1;
                var useruin = id.substring(index);
                client.messages.push(useruin);
            }
        });
    },
    match: function (text) {
        for (var i = 0; i < filter.keywords.length; i++) {
            var key = filter.keywords[i];
            if (text.indexOf(key) >= 0)
                return key;
        }
        return null;
    }
};
builder.register(filter.init, 20);