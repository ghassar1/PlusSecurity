(function () {
    'use strict';

    angular.module('app.core')
        .factory('interceptorService', interceptorService);

    interceptorService.$inject = ['$q', '$log', '$rootScope', '$injector'];

    function interceptorService($q, $log, $rootScope, $injector) {

        return {
            request: _request,
            requestError: _requestError,
            response: _response,
            responseError: _responseError
        };

        function _request(config) {

            //$rootScope.mainViewAnimation = true;

            config.headers = config.headers || {};

            var token = null;
            if ($rootScope.User.hasLoggedIn()) {
                token = $rootScope.User.getAccessToken();
            }

            //$log.log('intercepter > Token: ' + token);
            if (token) {
                config.headers.Authorization = 'Bearer ' + token;
            }

            $rootScope.notifications.clear();

            //setProgressFlags(config);

            return config;
        }

        function _requestError(rejection) {
            //resetProgressFlags();
            return $q.reject(rejection);
        }

        function _response(response) {
            //resetProgressFlags();
            return response;
        }

        function _responseError(rejection) {

            //resetProgressFlags();
            console.error(rejection);
            if (!rejection.data) { rejection.data = {}; }
            console.error(rejection.data);

            // Get $state from $injector. Injecting $state into interceptor causes circular dependency error. Details in link below.
            // http://stackoverflow.com/questions/20230691/injecting-state-ui-router-into-http-interceptor-causes-circular-dependency/20230786#20230786
            var $state = $injector.get('$state');

            //$rootScope.mainViewAnimation = false;

            var errorMessage = '';

            //var ret = null;
            if (rejection.status === 400) {
                if (rejection.data.error && rejection.data.error_description) {
                    console.debug(1);
                    $rootScope.notifications.error(rejection.data.error_description, rejection.data.error);
                    //ret = rejection.data;
                } else if (rejection.data.modelState) {
                    console.debug(2);
                    angular.forEach(rejection.data.modelState, function (value, key) {
                        console.log(key + ': ' + value);
                        //$rootScope.notifications.error(key, value);
                        $rootScope.notifications.error(value);
                    });
                    //ret = rejection.data.modelState;
                } else if (rejection.data.message) {
                    console.debug(3);
                    $rootScope.notifications.error(rejection.data.message);
                    //ret = rejection.data.message;
                } else if (isObject(rejection.data)) {
                    console.debug(4);
                    $log.log(toString.call(rejection.data));
                    $log.log(rejection.data);

                    angular.forEach(rejection.data, function (error, fieldName) {
                        console.log(fieldName + ': ' + error);
                        //$rootScope.notifications.error(fieldName, error);
                        //$rootScope.notifications.error(error, fieldName);
                        $rootScope.notifications.error(error);
                    });
                    //ret = rejection.data.modelState;
                }
            }
            else if (rejection.status === 401) {

                //ret = rejection.data;

                // Remove auth data if it already exists. 
                //delete $rootScope.authData;
                //delete $localStorage.authData;

                $rootScope.notifications.notify('You are not authorized to perform this operation.');
                $state.go('app.error.unauth');

                //if ($rootScope.authData) {
                //   $state.go('app.dashboard.home');
                //} else {
                //    $state.go('app.account.login');
                //}
                
            }
            else if (rejection.status === 403) {

                //ret = rejection.data;
                console.log(rejection.data);
                errorMessage = getMessageFromException(rejection.data);
                $rootScope.notifications.error(errorMessage ? errorMessage :
                    'You are forbidden to perform this operation.');
                //$state.go('app.error.unauth');

            }
            else if (rejection.status === 500) {

                console.error(rejection.data);
                errorMessage = getMessageFromException(rejection.data);
                $rootScope.notifications.error(errorMessage);

                //$state.go('app.error.app', { error: ret });
            } else {
                //$rootScope.notifications.error('', rejection.data);
                console.debug(rejection);
                throw new Error(rejection);
            }
            return $q.reject(rejection);
        }

        function getMessageFromException(ex) {
            var msg = '';
            if (ex.exceptionMessage)
                msg = ex.exceptionMessage;
            else if (ex.message)
                msg = ex.message;
            return msg;
        }

        //function setProgressFlags(config) {
        //    //console.debug(config.method);
        //    $rootScope.requestInProcess = true;
        //    switch (config.method) {
        //        case 'GET':
        //            $rootScope.fetchingData = true;
        //            break;
        //            //case 'POST':
        //            //    $rootScope.sendingData = true;
        //            //    break;
        //            //case 'PUT':
        //            //    $rootScope.sendingData = true;
        //            //    break;
        //            //case 'DELETE':
        //            //    $rootScope.sendingData = true;
        //            //    break;
        //        default:
        //            $rootScope.sendingData = true;
        //            break;
        //    }
        //}

        //function resetProgressFlags() {

        //    var $http = $injector.get('$http');
        //    if ($http.pendingRequests.length > 0) return;
            
        //    $rootScope.requestInProcess = false;
        //    $rootScope.fetchingData = false;
        //    $rootScope.sendingData = false;
        //}
    }
})();
