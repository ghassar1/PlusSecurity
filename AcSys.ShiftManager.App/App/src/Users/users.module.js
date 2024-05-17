(function () {
    'use strict';

    angular.module('app.users', ['app.core', 'ui.bootstrap', 'ngAnimate', 'ngSanitize'])
        .config(['$stateProvider', '$urlRouterProvider', configModule])
        .run([runModule]);

    function configModule($stateProvider, $urlRouterProvider) {

        $stateProvider
            .state('app.users', {
                url: '/users',
                template: '<div ui-view></div>',
                abstract: true,
                data: {
                    auth: { allowAnonymous: false, roles: ['SuperAdmin', 'Admin'] },
                    pageTitle: 'Users'
                }
            }).state('app.users.create', {
                url: '/new',
                templateUrl: 'App/dist/Views/Users/create/create.html',
                controller: 'usersCreateController',
                data: {
                    auth: { allowAnonymous: false, roles: ['SuperAdmin', 'Admin'] },
                    pageTitle: 'Create User',
                    breadcrumbs: ['app.dashboard.home', 'app.users.list']
                },
                resolve: { datepicker: resolveDatePicker, select2: resolveSelect2 }
            }).state('app.users.list', {
                url: '',
                templateUrl: 'App/dist/Views/Users/list/list.html',
                controller: 'usersListController',
                data: {
                    auth: { allowAnonymous: false, roles: ['SuperAdmin', 'Admin'] },
                    pageTitle: 'Users',
                    breadcrumbs: ['app.dashboard.home']
                },
                resolve: { }
            }).state('app.users.update', {
                url: '/{id}/update',
                templateUrl: 'App/dist/Views/Users/update/update.html',
                controller: 'usersUpdateController',
                data: {
                    auth: { allowAnonymous: false, roles: ['SuperAdmin', 'Admin'] },
                    pageTitle: 'Update User',
                    breadcrumbs: ['app.dashboard.home', 'app.users.list']
                },
                resolve: { datepicker: resolveDatePicker, select2: resolveSelect2 }
            }).state('app.users.view', {
                //url: '/{id:[a-f0-9]{8}(?:-[a-f0-9]{4}){3}-[a-f0-9]{12}}',    // Regex in url enforces Id to be a Guid, otherwise forwards to NotFound (404)
                url: '/{id}',
                templateUrl: 'App/dist/Views/Users/view/view.html',
                controller: 'usersViewController',
                data: {
                    auth: { allowAnonymous: false, roles: ['SuperAdmin', 'Admin'] },
                    pageTitle: 'User Details',
                    breadcrumbs: ['app.dashboard.home', 'app.users.list']
                },
                resolve: { }
            });
    }

    runModule.$inject = [];
    function runModule() {
        // ...
    }

    resolveDatePicker.$inject = ['$ocLazyLoad'];
    function resolveDatePicker($ocLazyLoad) {
        return $ocLazyLoad.load({
            serie: true,
            files: [
                'App/ColorAdmin/assets/plugins/bootstrap-datepicker/css/datepicker.css',
                'App/ColorAdmin/assets/plugins/bootstrap-datepicker/css/datepicker3.css',
                'App/ColorAdmin/assets/plugins/bootstrap-datepicker/js/bootstrap-datepicker.js'
            ]
        });
    }

    resolveSelect2.$inject = ['$ocLazyLoad'];
    function resolveSelect2($ocLazyLoad) {
        return $ocLazyLoad.load({
            serie: true,
            files: [
                'App/ColorAdmin/assets/plugins/select2/dist/css/select2.min.css',
                'App/ColorAdmin/assets/plugins/bootstrap-select/bootstrap-select.min.js',
                'App/ColorAdmin/assets/plugins/select2/dist/js/select2.min.js'
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

})();
