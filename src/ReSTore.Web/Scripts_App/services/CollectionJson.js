
var CollectionJson = function () {

    var parse = function (collectionJson) {
        var obj = {};
        obj.items = $.map(collectionJson.collection.items, parseItem);
        obj._links = parseLinks(collectionJson.collection);
        return obj;
    };

    var parseItem = function (item) {
        var o = {};
        for (var dataItem in item.data) {
            var name = item.data[dataItem].name;
            var val = item.data[dataItem].value;
            o[name] = val;
        }
        
        o._links = parseLinks(item);
        o._href = item.href;
        return o;
    };

    var parseLinks = function(item) {
        var colJsonLinks = item.links;
        var links = {};
        for (var link in colJsonLinks) {
            var colJsonLink = colJsonLinks[link];
            var linkRelArray = links[colJsonLink.rel];
            if (linkRelArray == null) {
                links[colJsonLink.rel] = linkRelArray = [];
            }
            linkRelArray.push(colJsonLink);
        }
        return links;
    };
    
    var fillTemplate = function(template, items) {
        for (var item in template.data) {
            var dataItem = template.data[item];
            var name = dataItem.name;
            var val = getValue(name, items);
            if (val) {
                dataItem.value = val;
            } else {
                // If there is a prompt value, we should ask the user for a value
                if (dataItem.prompt) {
                    dataItem.value = prompt("Please enter " + dataItem.prompt);
                }
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
        parse: parse,
        fillTemplate : fillTemplate
    };
}();


