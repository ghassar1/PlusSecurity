(function () {
    'use strict';

    angular.module('app.core', ['ui.router', 'ui.bootstrap', 'oc.lazyLoad', 'ngStorage'])
        .config(configCoreModule)
        .run(runCoreModule);

    configCoreModule.$inject = ['$stateProvider', '$provide', '$httpProvider', '$logProvider'];
    function configCoreModule($stateProvider, $provide, $httpProvider, $logProvider) {

        $httpProvider.interceptors.push('interceptorService');

        $logProvider.debugEnabled(true);

        $provide.decorator('$log', ['$delegate', 'logger', loggerDelegate]);

    }

    function loggerDelegate($delegate, logger) {
        return logger($delegate);
    }

    function runCoreModule() {
        // ...
    }

})();
