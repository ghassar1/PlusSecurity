(function () {
    'use strict';

    angular.module('app.account')
        .factory('accountService', serviceFactory);

    serviceFactory.$inject = ['$http', '$q', '$log', 'AppHttpService'];

    function serviceFactory($http, $q, $log, AppHttpService) {

        // Promise-based API
        return {
            authenticate: authenticate,
            getLoggedInUserDetails: getLoggedInUserDetails
        };

        function getLoggedInUserDetails(user) {
            
            var ep = '/users/me';
            return AppHttpService.Get(ep);
        }

        function authenticate(user) {

            //var url = 'http://localhost:64736/Token';
            var url = '/Token';

            var contentType = 'application/x-www-form-urlencoded';

            var loginData = {
                client_id: 'web',
                grant_type: 'password',
                response_type: 'token',
                username: user.username,
                password: user.password
            };

            return AppHttpService.Post(url, loginData, contentType, true);
        }
    }
})();
