//(function () {
//    'use strict';

//    angular.module('app.core')
//        .provider('$exceptionHandler', { $get: exceptionHandler });


//    // By default, AngularJS will catch errors and log them to
//    // the Console. We want to keep that behavior; however, we
//    // want to intercept it so that we can also log the errors
//    // to the server for later analysis.
//    exceptionHandler.$inject = ['$log', 'errorLogService'];
//    function exceptionHandler($log, errorLogService) {
//        //return errorLogService;
//        return handleError;

//        // I log the given error to the remote server.
//        function handleError(error, cause) {

//            try {
//                //$log.error('Cause: ' + cause);
//                //$log.error('Error: ', error);
//                //$log.error('Error Name: ', error.name);
//                //$log.error('Error Message: ', error.message);

//                if (error instanceof AuthError) {
//                    $log.error('An Auth Error!');
//                } else {
//                    $log.error('An Error!');
//                }

//            } catch (loggingError) {
//                // For Developers - log the log-failure.
//                $log.warn('Error logging failed');
//                $log.log(loggingError);
//            }
//        }
//    }

//    //TODO: http://blog.loadimpact.com/blog/exception-handling-in-an-angularjs-web-application-tutorial/

//})();
