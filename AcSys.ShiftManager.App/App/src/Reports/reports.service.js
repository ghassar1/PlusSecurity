(function () {
    'use strict';

    angular.module('app.reports')
        .factory('reportsService', serviceFactory);

    serviceFactory.$inject = ['$http', '$q', '$log', 'AppHttpService'];

    function serviceFactory($http, $q, $log, AppHttpService) {

        var ep = '/Shifts';

        return {
            getAttendanceData: getAttendanceData,
            getAttendanceSummaryData: getAttendanceSummaryData
        };

        function getAttendanceData(query) {
            return AppHttpService.Get(ep + '/Reports/Attendance', query);
        }

        function getAttendanceSummaryData(query) {
            return AppHttpService.Get(ep + '/Reports/Attendance/Summary', query);
        }
    }
})();
