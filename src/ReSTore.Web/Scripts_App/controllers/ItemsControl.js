
ngRestore.controller("ItemsControl",
    ['$scope', '$http', '$routeParams', '$location', function ($scope, $http, $routeParams, $location) {

        $http.defaults.headers.common['Accept'] = 'application/vnd.collection+json';

        var href = $routeParams.href;
        if (href == undefined)
            href = '/api/areas';
        
        $scope.items = [];

        $scope.refresh = function() {
            $http.get(href).success(function(data) {
                $scope.items = CollectionJson.parseItems(data);
            });
        };

        $scope.select = function(item) {
            $location.search({ href: item.href });
        };

        $scope.refresh();
    }]);


