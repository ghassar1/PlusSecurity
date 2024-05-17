(function () {

    angular.module('app.core')
        //.constant('_', window._)
        .factory('_', underscore);

    function underscore() {
        return window._; // assumes underscore has already been loaded on the page
    }

})();
