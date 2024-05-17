(function () {
    'use strict';

    angular.module('app.core')
        .factory('stacktraceService', factory);

    function factory() {
        // 'printStackTrace' is a global object.
        return {
            //print: printStackTrace
            StackTrace: StackTrace
        };
    }
})();
