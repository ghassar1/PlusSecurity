(function () {
    'use strict';

    angular.module('app.core')
        .factory('errorLogService', factory);

    factory.$inject = ['$log', '$window', 'stacktraceService'];
    function factory($log, $window, stacktraceService) {

        // Return the logging function.
        return log;

        // I log the given error to the remote server.
        function log(error, cause) {

            // Pass off the error to the default error handler on the AngularJS logger. This will output 
            // the error to the console and let the application keep running normally for the user.
            //$log.error.apply($log, arguments);

            // Now, we need to try and log the error the server.
            // NOTE: In production, I have some debouncing logic here to prevent the same client from
            // logging the same error over and over again! All that would do is add noise to the log.
            try {

                //$log.error('Cause: ' + cause);

                //var errorMessage = error.toString();
                //$log.error('Error: ', error);
                //$log.error('Error Name: ', error.name);
                //$log.error('Error Message: ', error.message);

                $log.error(error.name, error.message);

                //var stackTrace = stacktraceService.print({ e: error });
                stacktraceService.StackTrace.fromError(error)
                    .then(callback)
                    .catch(errorCallback);

            } catch (loggingError) {
                // For Developers - log the log-failure.
                $log.warn('Error logging failed');
                $log.log(loggingError);
            }

            function callback(stackTrace) {
                // Log the JavaScript error to the server.
                // NOTE: In this demo, the POST URL doesn't exists and will simply return a 404.
                //$rootScope.notifications.error(errorMessage, stackTrace);

                //$log.error('StackTrace: ' + stackTrace);
                $log.debug('StackTrace: ' + stackTrace);

                //alert('url ' + $window.location.href +' errorMessage ' + errorMessage + ' stackTrace ' + stackTrace);
                //$.ajax({
                //    type: 'POST',
                //    url: './javascript-errors',
                //    contentType: 'application/json',
                //    data: angular.toJson({
                //        errorUrl: $window.location.href,
                //        errorMessage: errorMessage,
                //        stackTrace: stackTrace,
                //        cause: ( cause || '' )
                //    })
                //});
            }

            function errorCallback(loggingError) {
                // For Developers - log the log-failure.
                $log.warn('Error logging using StackTrace.js failed');
                $log.log(loggingError);
            }
        }
    }
})();
