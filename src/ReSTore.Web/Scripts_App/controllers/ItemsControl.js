
ngRestore.controller("ItemsControl",
    ['$scope', '$http', '$routeParams', '$location', function ($scope, $http, $routeParams, $location) {

        $http.defaults.headers.common['Accept'] = 'application/vnd.collection+json';

        $scope.href = $routeParams.href;
        if ($scope.href == undefined)
            $scope.href = '/api/areas';
        
        $scope.items = [];

        $scope.refresh = function() {
            $http.get($scope.href).success(function (data) {
                $scope.items = CollectionJson.parseItems(data);
            });
        };

        $scope.select = function (item) {
            var itemHref = item._links.children[0].href;
            $scope.href = itemHref;
            $location.search({ href: itemHref });
            $scope.refresh();
        };

        $scope.hasChildren = function(item) {
            return item._links.children != null;
        };

        $scope.orderItems = [];

        $scope.postCommand = function (item, command) {
            $http.get(command.href).success(function(data) {
                var template = data.collection.template;
                CollectionJson.fillTemplate(template, [item, $scope]);
                console.log(template);
            });
        };
        $scope.createOrder = function() {
            $http.post('/api/order').success(
                function (data, status, headers) {
                    $scope.orderId = headers('Order-Id');
                });
        };
        
        $scope.refreshOrder = function () {
            if (!$scope.orderId)
                return;
            
            $http.get('/api/order', {
                headers: {
                    'Order-Id' : $scope.orderId
                }
            }).success(
                function(data) {
                    $scope.orderItems = CollectionJson.parseItems(data);
                }
            );
        };

        $scope.refresh();
        $scope.refreshOrder();
    }]);


