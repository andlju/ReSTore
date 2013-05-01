
ngRestore.controller("CategoriesControl",
    ['$scope', '$http', '$routeParams', function($scope, $http, $routeParams) {

        $http.defaults.headers.common['Accept'] = 'application/vnd.collection+json';
        
        $scope.categories = [];

        $scope.refresh = function () {
            
            $http.get($routeParams.href).success(function (data) {
                $scope.categories = CollectionJson.parseItems(data);
            });
        };

        $scope.refresh();
    }]);


