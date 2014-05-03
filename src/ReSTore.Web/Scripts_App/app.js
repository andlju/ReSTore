// Add app module

var ngRestore = angular.module('ngRestore', ['ngRoute']).
    config(['$routeProvider', function($routeProvider) {
        $routeProvider.
            when('/items', { templateUrl: 'Home/Items', controller: 'ItemsControl', reloadOnSearch : false }).
            otherwise({ redirectTo: '/items' });
    }]);

console.log(ngRestore);