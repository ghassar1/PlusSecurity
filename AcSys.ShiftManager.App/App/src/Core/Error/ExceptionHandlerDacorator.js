(function () {
    'use strict';

    angular.module('app.core')
        .config(configCoreModule);

    configCoreModule.$inject = ['$stateProvider', '$provide', '$httpProvider', '$logProvider'];
    function configCoreModule($stateProvider, $provide, $httpProvider, $logProvider) {

        $provide.decorator('$exceptionHandler', decorator);

        decorator.$inject = ['$injector', '$log', '$delegate', 'errorLogService'];
        function decorator($injector, $log, $delegate, errorLogService) {

            return function (error, cause) {

                $delegate(error, cause);

                var $rootScope = $injector.get('$rootScope');
                var $state = $injector.get('$state');
                var $location = $injector.get('$location');
                
                $log.debug('Default error handler.');

                try {
                    if (error instanceof AuthError) {
                        $log.error('An Auth Error!');
                        $location.replace();
                        $state.go('app.error.unauth', { error: error });
                    } else {
                        $log.error('An Error!');
                        $state.go('app.error.app', { error: error });
                    }

                    errorLogService(error, cause);

                } catch (loggingError) {
                    $log.warn('Error logging failed');
                    $log.log(loggingError);
                }
            };
        }
    }
    //TODO: http://blog.loadimpact.com/blog/exception-handling-in-an-angularjs-web-application-tutorial/
})();
