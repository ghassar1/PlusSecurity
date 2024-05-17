(function () {
    'use strict';

    // Directive for showing the overlay loading message or spinner during http calls
    // Example: <div class="loading" loading>Loading&#8230;</div>
    // http://stackoverflow.com/a/19628104/3423802
    angular.module('app.core')
        .directive('loading', directive);

    directive.$inject = ['$http'];
    function directive($http) {
        return {
            restrict: 'A',
            link: function (scope, elm, attrs) {

                scope.isLoading = function () {
                    return $http.pendingRequests.length > 0;
                };

                scope.$watch(scope.isLoading, function (v) {
                    if (v) {
                        elm.show();
                    } else {
                        elm.hide();
                    }
                });
            }
        };
    }
})();
