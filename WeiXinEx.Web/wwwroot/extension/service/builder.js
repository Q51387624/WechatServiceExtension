var builder = {
    list: [],
    register: function (init, priority) {
        builder.list.push({
            priority: priority,
            init: init
        });
    },
    initialization: function () {
        builder.list.order(function (i) {
            return i.priority;
        }).forEach(function (item) {
            item.init();
        });
    }
}
