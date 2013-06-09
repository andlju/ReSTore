ngRestore.service('collectionJson', function() {

    // Return an array of parsed Collection+JSON items and corresponding links
    var parse = function (collectionJson) {
        var obj = {};
        obj.items = $.map(collectionJson.collection.items, parseItem);
        obj._links = parseLinks(collectionJson.collection);
        return obj;
    };

    // Return a single parsed item with links
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

    // Return a HAL-style array of links
    var parseLinks = function (item) {
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

    var fillTemplate = function (template, context) {
        var dataMissing = false;
        for (var item in template.data) {
            var dataItem = template.data[item];
            var name = dataItem.name;
            var val = getContextValue(name, context);
            if (val) {
                dataItem.value = val;
            } else {
                dataMissing = true;
            }
        }
        return dataMissing;
    };

    var getContextValue = function (name, context) {
        for (var idx in context) {
            var obj = context[idx];
            var val = obj[name];
            if (val) {
                return val;
            }
        }
        return null;
    };

    return {
        parse: parse,
        fillTemplate: fillTemplate
    };
});
