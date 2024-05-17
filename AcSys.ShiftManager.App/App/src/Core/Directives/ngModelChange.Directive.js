(function () {
    'use strict';

    // Directive for running callbacks when ngModel changes.
    // http://stackoverflow.com/questions/14693052/watch-ngmodel-from-inside-directive-using-isolate-scope
    // http://jsfiddle.net/BtrZH/5/
    angular.module('app.core')
        .directive('ngModelChange', directive);

    function directive() {
        return {
            //restrict: 'AEC',
            require: 'ngModel',
            scope: {
                model: '=ngModel'
            },
            replace: true,
            link: link
        };

        function link(scope, element, attrs, controller, transcludeFn) {

            console.log(scope.model);

            scope.$watch('model', function () {

                console.log('Model Changed', scope.model);

                scope.$evalAsync(function () {
                    scope.$eval(attrs.ngModelChanged, { 'event': event });
                });
            });
        }
    }
})();
