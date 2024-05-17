(function () {
    'use strict';

    angular.module('app.dashboard', ['app.core'])
        .config(['$stateProvider', '$urlRouterProvider', configDashboardModule])
        .run([runDashboardModule]);

    function configDashboardModule($stateProvider, $urlRouterProvider) {

        $stateProvider
            .state('app.dashboard', {
                url: '/dashboard',
                template: '<div ui-view></div>',
                abstract: true
                //templateUrl: 'App/dist/Views/Dashboard/dashboard.html',

                //data: { pageTitle: 'Dashboard' },
                //resolve: {
                //    service: ['$ocLazyLoad', resolveDashboard]
                //}
            }).state('app.dashboard.home', {
                url: '/home',
                //template: '<div ui-view></div>',
                templateUrl: 'App/dist/Views/Dashboard/Home/dashboard.html',

                data: {
                    auth: { allowAnonymous: false, roles: ['SuperAdmin', 'Admin'] },
                    pageTitle: 'Dashboard',
                    breadcrumbs: []
                },
                resolve: {
                    service: resolveDashboard,
                    //angles: resolveAngles,
                    angularChartJs: resolveAngularChartJs,
                    morris: resolveMorris
                }
            });
    }

    function runDashboardModule() {
        // ...
    }

    resolveDashboard.$inject = ['$ocLazyLoad'];
    function resolveDashboard($ocLazyLoad) {
        return $ocLazyLoad.load({
            serie: true,
            files: [
                'App/ColorAdmin/assets/plugins/jquery-jvectormap/jquery-jvectormap-1.2.2.css',
                'App/ColorAdmin/assets/plugins/bootstrap-calendar/css/bootstrap_calendar.css',
                'App/ColorAdmin/assets/plugins/gritter/css/jquery.gritter.css',
                'App/ColorAdmin/assets/plugins/morris/morris.css',
                'App/ColorAdmin/assets/plugins/morris/raphael.min.js',
                'App/ColorAdmin/assets/plugins/morris/morris.js',
                'App/ColorAdmin/assets/plugins/jquery-jvectormap/jquery-jvectormap-1.2.2.min.js',
                'App/ColorAdmin/assets/plugins/jquery-jvectormap/jquery-jvectormap-world-merc-en.js',
                'App/ColorAdmin/assets/plugins/bootstrap-calendar/js/bootstrap_calendar.min.js',
                'App/ColorAdmin/assets/plugins/gritter/js/jquery.gritter.min.js'
            ]
        });
    }

    resolveAngles.$inject = ['$ocLazyLoad'];
    function resolveAngles($ocLazyLoad) {
        return $ocLazyLoad.load({
            serie: true,
            name: 'angles',
            files: [
                //'App/ColorAdmin/assets/plugins/chart-js/chart.min.js',
                //'App/ColorAdmin/assets/plugins/chart-js/angular/angles.js',

                'App/dist/js/Chart.min.js',
                'App/dist/js/angles.js'
            ]
        });
    }

    resolveMorris.$inject = ['$ocLazyLoad'];
    function resolveMorris($ocLazyLoad) {
        return $ocLazyLoad.load({
            serie: true,
            name: 'ng-morris-js',
            files: [
                'App/dist/css/morris.css',
                'App/dist/js/raphael.min.js',
                'App/dist/js/morris.min.js',
                'App/dist/js/ng-morris-js.min.js'
            ]
        });
    }

    resolveAngularChartJs.$inject = ['$ocLazyLoad'];
    function resolveAngularChartJs($ocLazyLoad) {
        return $ocLazyLoad.load({
            serie: true,
            name: 'chart.js',
            files: [
                'App/dist/js/Chart.min.js',
                'App/dist/js/angular-chart.min.js'
            ]
        });
    }
})();
