
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

    var fillTemplate = function(template, items) {
        for (var item in template.data) {
            var dataItem = template.data[item];
            var name = dataItem.name;
            var val = getValue(name, items);
            if (val) {
                dataItem.value = val;
            }
        }
    };

    var getValue = function(name, items) {
        for (var idx in items) {
            var obj = items[idx];
            var val = obj[name];
            if (val) {
                return val;
            }
        }
        return null;
    };

    return {
        parseItems: parseItems,
        fillTemplate : fillTemplate
    };
}();


