(function () {
    'use strict';

    // Directive for running callbacks upon pressing Enter key in text input fields.
    // <img ng-src='dynamic_path' ng-on-load='search()'>
    // http://stackoverflow.com/a/17884754/3423802
    angular.module('app.core')
        .directive('ngOnLoad', directive);

    function directive() {
        return function (scope, element, attrs) {
            element.bind('load', function (event) {
                //console.debug(event);
                console.debug('image is loaded');

                scope.$apply(function () {
                    scope.$eval(attrs.ngOnLoad, { 'event': event });
                });
            });

            element.bind('error', function () {

                console.debug('image could not be loaded');

                //scope.$apply(function (event) {
                //    scope.$eval(attrs.ngEnter, { 'event': event });
                //});
                //event.preventDefault();
            });
        };
    }

})();
