// Add app module

var ngRestore = angular.module('ngRestore', []).
    config(['$routeProvider', function($routeProvider) {
        $routeProvider.
            when('/areas', { templateUrl: 'Home/Areas', controller: 'AreasControl' }).
            when('/categories', { templateUrl: 'Home/Categories', controller: 'CategoriesControl' }).
            otherwise({ redirectTo: '/areas' });
    }]);