
ngRestore.service('commandHandler',
    ['$http', '$q', 'collectionJson', function ($http, $q, collectionJson) {

        $http.defaults.headers.common['Accept'] = 'application/vnd.collection+json';
        $http.defaults.headers.post['Content-Type'] = 'application/vnd.collection+json';

        var executeCommand = function (href, context, missingDataPrompt) {
            missingDataPrompt = missingDataPrompt || defaultPrompt;

            $http.get(href).success(function (data) {
                var template = data.collection.template;

                var dataMissing = collectionJson.fillTemplate(template, context);
                if (!dataMissing) {
                    // All fields have been filled. Dispatch command immediately
                    dispatch(href, template);
                    return;
                }
                // Some fields missing, try to get them from the user
                missingDataPrompt(template).then(
                    function() {
                        dispatch(href, template);
                    }
                );
            });
        };

        var dispatch = function(href, template) {
            $http.post(href, { template: template });
        };
        
        var defaultPrompt = function (template) {
            var deferred = $q.defer();
            
            for (var item in template.data) {
                var dataItem = template.data[item];
                var name = dataItem.name;
                var val = dataItem.value;
                var promptText = dataItem.prompt;
                if (!val && promptText) {
                    dataItem.value = prompt(promptText);
                }
            }
            deferred.resolve(template);
            return deferred.promise;
        };

        return {
            exec: executeCommand
        };
    }]);


