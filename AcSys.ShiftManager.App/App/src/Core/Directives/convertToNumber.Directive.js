(function () {
    'use strict';

    // Directive for running callbacks upon pressing Enter key in text input fields.
    // <input type='search' ng-model='query.searchCriteria' ng-enter='search()'>
    // http://stackoverflow.com/a/17364716/3423802
    // https://docs.angularjs.org/api/ng/directive/select#binding-select-to-a-non-string-value-via-ngmodel-parsing-formatting
    angular.module('app.core')
        .directive('convertToNumber', directive);

    function directive() {
        return {
            require: 'ngModel',
            link: function (scope, element, attrs, ngModel) {
                ngModel.$parsers.push(function (val) {
                    return parseInt(val, 10);
                });
                ngModel.$formatters.push(function (val) {
                    return '' + val;
                });
            }
        };
    }
})();
