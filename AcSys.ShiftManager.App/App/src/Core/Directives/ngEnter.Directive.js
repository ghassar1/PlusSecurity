(function () {
    'use strict';

    // Directive for running callbacks upon pressing Enter key in text input fields.
    // <input type='search' ng-model='query.searchCriteria' ng-enter='search()'>
    // http://stackoverflow.com/a/17364716/3423802
    angular.module('app.core')
        .directive('ngEnter', directive);

    function directive() {
        return function (scope, element, attrs) {

            console.log(element);

            element.bind('keydown keypress', function (event) {
                if (event.which === 13) {
                    scope.$apply(function () {
                        scope.$eval(attrs.ngEnter, { 'event': event });
                    });

                    event.preventDefault();
                }
            });
        };
    }

})();
