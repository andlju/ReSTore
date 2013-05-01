
ngRestore.controller("AreasControl",
    ['$scope', '$http', function($scope, $http) {

        $http.defaults.headers.common['Accept'] = 'application/vnd.collection+json';
        
        $scope.areas = [];

        $scope.refresh = function() {
            $http.get('/api/areas').success(function(data) {
                $scope.areas = CollectionJson.parseItems(data);
            });
        };

        $scope.refresh();
    }]);


