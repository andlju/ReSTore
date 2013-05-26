
ngRestore.controller("ItemsControl",
    ['$scope', '$http', '$routeParams', '$location', '$timeout', function ($scope, $http, $routeParams, $location, $timeout) {

        $http.defaults.headers.common['Accept'] = 'application/vnd.collection+json';
        $http.defaults.headers.post['Content-Type'] = 'application/vnd.collection+json';

        $scope.href = $routeParams.href;
        if ($scope.href == undefined)
            $scope.href = '/api/areas';
        
        $scope.items = [];

        $scope.refresh = function() {
            $http.get($scope.href).success(function (data) {
                $scope.items = CollectionJson.parse(data).items;
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


        $scope.postCommand = function (item, command) {
            $http.get(command.href).success(function (data) {
                var template = data.collection.template;

                CollectionJson.fillTemplate(template, [item, $scope]);
                
                $http.post(command.href, { template: template });
            });
        };

        $scope.orderItems = [];

        $scope.createOrder = function () {
            $http.post('/api/order').success(
                function (data, status, headers) {
                    $scope.orderId = headers('Order-Id');
                    $timeout($scope.refreshOrder, 2000);
                    // $scope.registerSignalR();
                });
        };

        /*
        var orderHub = $.connection.orderHub;
        $.connection.hub.start().done($scope.registerSignalR);

        $scope.registerSignalR = function() {
            orderHub.server.registerForOrder($scope.orderId);
        };
        
        orderHub.client.viewModelUpdated = function (id, type, content) {
            $scope.$apply(function() {
                if ($scope.orderId != id) {
                    alert('Unknown id ' + id);
                    return;
                }
                $scope.refreshOrder();
                console.log('ViewModel updated');
            });
        };
        */
        
        $scope.refreshOrder = function () {
            if (!$scope.orderId)
                return;
            
            $http.get('/api/order', {
                headers: {
                    'Order-Id': $scope.orderId,
                    'Cache-Control' : 'no-cache'
                }
            }).success(
                function(data) {
                    $scope.order = CollectionJson.parse(data);
                    console.log('Order refreshed');
                    $timeout($scope.refreshOrder, 2000);
                }
            ).error(function() {
                $timeout($scope.refreshOrder, 2000);
            });
        };

        $scope.refresh();
        $scope.refreshOrder();
    }]);


