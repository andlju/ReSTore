// Add app module

var ngRestore = angular.module('ngRestore', []).
    config(['$routeProvider', function($routeProvider) {
        $routeProvider.
            when('/items', { templateUrl: 'Home/Items', controller: 'ItemsControl' }).
            otherwise({ redirectTo: '/items' });
    }]);