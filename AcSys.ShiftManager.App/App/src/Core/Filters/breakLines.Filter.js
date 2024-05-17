(function () {
    'use strict';

    // http://stackoverflow.com/questions/18513286/angular-displaying-multiple-lined-strings-on-one-line
    // http://stackoverflow.com/a/18513558/3423802
    angular.module('app.core')
        .filter('breakLines', filter);

    function filter() {
        return function (text) {
            if (text !== undefined)
                return text.replace(/\n/g, '<br />');
        };
    }
})();
