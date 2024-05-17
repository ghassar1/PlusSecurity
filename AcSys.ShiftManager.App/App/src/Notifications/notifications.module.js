(function () {
    'use strict';

    angular.module('app.notifications', ['app.core', 'ngSanitize'])
        .config(configModule)
        .run(runModule);

    configModule.$inject = ['$stateProvider', '$urlRouterProvider'];
    function configModule($stateProvider, $urlRouterProvider) {

        $stateProvider
            .state('app.notifications', {
                url: '/notifications',
                template: '<div ui-view></div>',
                abstract: true,
                data: {
                    auth: { allowAnonymous: false, roles: [] },
                    pageTitle: 'Notifications'
                }
            }).state('app.notifications.create', {
                url: '/new',
                templateUrl: 'App/dist/Views/Notifications/create/create.html',
                controller: 'notificationsCreateController',
                data: {
                    auth: { allowAnonymous: false, roles: ['SuperAdmin', 'Admin'] },
                    pageTitle: 'Create Notification',
                    breadcrumbs: ['app.dashboard.home', 'app.notifications.list']
                },
                resolve: { data: resolveCreate }
            }).state('app.notifications.list', {
                url: '',
                templateUrl: 'App/dist/Views/Notifications/list/list.html',
                controller: 'notificationsListController',
                data: {
                    pageTitle: 'Notifications',
                    breadcrumbs: ['app.dashboard.home']
                },
                resolve: {
                    //data: resolveList
                    //datatables: resolveDataTables
                }
            }).state('app.notifications.update', {
                url: '/{id}/update',
                templateUrl: 'App/dist/Views/Notifications/update/update.html',
                controller: 'notificationsUpdateController',
                data: {
                    pageTitle: 'Update Notification',
                    breadcrumbs: ['app.dashboard.home', 'app.notifications.list']
                },
                resolve: { data: resolveUpdate }
            }).state('app.notifications.view', {
                url: '/{id}',
                templateUrl: 'App/dist/Views/Notifications/view/view.html',
                controller: 'notificationsViewController',
                data: {
                    pageTitle: 'View Notification',
                    breadcrumbs: ['app.dashboard.home', 'app.notifications.list']
                },
                resolve: { data: resolveView }
            });
    }

    runModule.$inject = [];
    function runModule() {
        // ...
    }

    resolveCreate.$inject = ['$ocLazyLoad'];
    function resolveCreate($ocLazyLoad) {
        return $ocLazyLoad.load({
            serie: true,
            files: [
                'App/ColorAdmin/assets/plugins/jquery-tag-it/css/jquery.tagit.css',
                'App/ColorAdmin/assets/plugins/jquery-tag-it/js/tag-it.min.js'
            ]
        });
    }

    resolveDataTables.$inject = ['$ocLazyLoad'];
    function resolveDataTables($ocLazyLoad) {
        return $ocLazyLoad.load({
            serie: true,
            files: [
                'App/ColorAdmin/assets/plugins/DataTables/media/css/dataTables.bootstrap.min.css',
                'App/ColorAdmin/assets/plugins/DataTables/extensions/Buttons/css/buttons.bootstrap.min.css',
                'App/ColorAdmin/assets/plugins/DataTables/extensions/Responsive/css/responsive.bootstrap.min.css',
                'App/ColorAdmin/assets/plugins/DataTables/media/js/jquery.dataTables.js',
                'App/ColorAdmin/assets/plugins/DataTables/media/js/dataTables.bootstrap.min.js',
                'App/ColorAdmin/assets/plugins/DataTables/extensions/Buttons/js/dataTables.buttons.min.js',
                'App/ColorAdmin/assets/plugins/DataTables/extensions/Buttons/js/buttons.bootstrap.min.js',
                'App/ColorAdmin/assets/plugins/DataTables/extensions/Buttons/js/buttons.flash.min.js',
                'App/ColorAdmin/assets/plugins/DataTables/extensions/Buttons/js/jszip.min.js',
                'App/ColorAdmin/assets/plugins/DataTables/extensions/Buttons/js/pdfmake.min.js',
                'App/ColorAdmin/assets/plugins/DataTables/extensions/Buttons/js/vfs_fonts.min.js',
                'App/ColorAdmin/assets/plugins/DataTables/extensions/Buttons/js/buttons.html5.min.js',
                'App/ColorAdmin/assets/plugins/DataTables/extensions/Buttons/js/buttons.print.min.js',
                'App/ColorAdmin/assets/plugins/DataTables/extensions/Responsive/js/dataTables.responsive.min.js'
            ]
        });
    }

    resolveUpdate.$inject = [];
    function resolveUpdate() {
        return null;
    }

    resolveView.$inject = [];
    function resolveView() {
        return null;
    }

    resolveList.$inject = ['notificationsService'];
    function resolveList(service) {
        return service.listMine({ searchCriteria: '', pageNo: 1, pageSize: 10, sortColumn: 'Date', sortType: 1 });
    }
})();
