(function () {
    'use strict';

    angular.module('app.activityLogs', ['app.core', 'daterangepicker', 'ui.select', 'ngSanitize'])
        .config(configModule)
        .run(runModule);

    configModule.$inject = ['$stateProvider', '$urlRouterProvider'];
    function configModule($stateProvider, $urlRouterProvider) {

        //uiSelectConfig.theme = 'bootstrap';

        $stateProvider
            .state('app.activityLogs', {
                url: '/logs',
                template: '<div ui-view></div>',
                abstract: true,
                data: {
                    auth: { allowAnonymous: false, roles: ['SuperAdmin'] },
                    pageTitle: 'Activity Logs'
                }
            }).state('app.activityLogs.list', {
                url: '',
                templateUrl: 'App/dist/Views/ActivityLogs/list/list.html',
                controller: 'activityLogsListController',
                data: {
                    auth: { allowAnonymous: false, roles: ['SuperAdmin'] },
                    pageTitle: 'Activity Logs',
                    breadcrumbs: ['app.dashboard.home']
                },
                resolve: { dateRangePicker: resolveDateRangePicker }
            }).state('app.activityLogs.view', {
                url: '/{id}',
                templateUrl: 'App/dist/Views/ActivityLogs/view/view.html',
                controller: 'activityLogsViewController',
                data: {
                    auth: { allowAnonymous: false, roles: ['SuperAdmin'] },
                    pageTitle: 'Activity Log Details',
                    breadcrumbs: ['app.dashboard.home', 'app.activityLogs.list']
                }
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
