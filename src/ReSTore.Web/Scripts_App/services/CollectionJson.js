
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
        o._links = {};
        for (var link in item.links) {
            var linkRelArray = o._links[item.links[link].rel];
            if (linkRelArray == null) {
                o._links[item.links[link].rel] = linkRelArray = [];
            }
            linkRelArray.push(item.links[link]);
        }
        o._href = item.href;
        console.log(o);
        return o;
    };

    return {
        parseItems: parseItems
    };
}();


