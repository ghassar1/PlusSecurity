(function () {
    'use strict';

    angular.module('app.core')
        .config(configCoreModule)
        .run(runCoreModule);

    configCoreModule.$inject = ['$stateProvider', '$provide', '$httpProvider', '$logProvider'];
    function configCoreModule($stateProvider, $provide, $httpProvider, $logProvider) {
    }

    runCoreModule.$inject = ['$window'];
    function runCoreModule($window) {
        AuthError.prototype = Object.create(Error.prototype);
        AuthError.prototype.constructor = AuthError;
        $window.AuthError = AuthError;
    }

    function AuthError(message) {
        this.name = 'AuthError';
        this.message = message || 'You are not authorized to perform this operation.';
        this.stack = (new Error()).stack;
    }

})();
