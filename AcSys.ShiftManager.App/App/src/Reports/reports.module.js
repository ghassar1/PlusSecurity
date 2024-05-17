(function () {
    'use strict';

    angular.module('app.reports', ['app.core', 'daterangepicker', 'ui.select', 'ngSanitize'])
        .config(configModule)
        .run(runModule);

    configModule.$inject = ['$stateProvider', '$urlRouterProvider'];
    function configModule($stateProvider, $urlRouterProvider) {

        //uiSelectConfig.theme = 'bootstrap';

        $stateProvider
            .state('app.reports', {
                url: '/Reports',
                template: '<div ui-view></div>',
                abstract: true,
                data: {
                    auth: { allowAnonymous: false, roles: ['SuperAdmin', 'Admin'] },
                    pageTitle: 'Reports'
                }
            }).state('app.reports.attendance', {
                url: '/Attendance',
                templateUrl: 'App/dist/Views/Reports/Attendance/report.html',
                controller: 'attendanceReportController',
                data: {
                    auth: { allowAnonymous: false, roles: ['SuperAdmin', 'Admin'] },
                    pageTitle: 'Detailed Attendance Report',
                    breadcrumbs: ['app.dashboard.home']
                },
                resolve: { dateRangePicker: resolveDateRangePicker }
            }).state('app.reports.attendanceSummary', {
                url: '/Attendance/Summary',
                templateUrl: 'App/dist/Views/Reports/AttendanceSummary/report.html',
                controller: 'attendanceSummaryReportController',
                data: {
                    auth: { allowAnonymous: false, roles: ['SuperAdmin', 'Admin'] },
                    pageTitle: 'Attendance Summary Report',
                    breadcrumbs: ['app.dashboard.home']
                },
                resolve: { dateRangePicker: resolveDateRangePicker }
            });
    }

    runModule.$inject = [];
    function runModule() {
        // ...
    }

    resolveDateRangePicker.$inject = ['$ocLazyLoad'];
    function resolveDateRangePicker($ocLazyLoad) {
        return $ocLazyLoad.load({
            serie: true,
            files: [
                'App/dist/js/moment.min.js',
                'App/dist/js/daterangepicker.js',
                'App/dist/js/angular-daterangepicker.js',
                'App/dist/css/daterangepicker.css'
            ]
        });
    }

})();
