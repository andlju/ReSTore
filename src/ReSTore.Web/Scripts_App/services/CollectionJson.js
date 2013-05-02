
var CollectionJson = function () {

    var parseItems = function (collectionJson) {
        return $.map(collectionJson.collection.items, parseItem);
    };

    var parseItem = function (item) {
        var o = {};
        for (var dataItem in item.data) {
            var name = item.data[dataItem].name;
            var val = item.data[dataItem].value;
            o[name] = val;
        }
        o.links = item.links;
        o.href = item.href;
        return o;
    };

    return {
        parseItems: parseItems
    };
}();


