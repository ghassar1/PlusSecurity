(function () {
    'use strict';

    angular.module('app.dashboard')
        .factory('dashboardService', serviceFactory);

    serviceFactory.$inject = ['$http', '$q', '$log', 'AppHttpService'];

    function serviceFactory($http, $q, $log, AppHttpService) {

        var ep = '/Shifts';

        return {
            getDashboardData: getDashboardData
        };

        function getDashboardData() {
            return AppHttpService.Get(ep + '/Dashboard');
        }
    }
})();
