(function () {
    'use strict';

    angular.module('app.shifts', ['app.core', 'ngSanitize', 'ui.bootstrap', 'ui.calendar', 'daterangepicker', 'ui.select', 'uiSwitch'])
        .config(configModule)
        .run(runModule);

    configModule.$inject = ['$stateProvider', '$urlRouterProvider'];
    function configModule($stateProvider, $urlRouterProvider) {

        //uiSelectConfig.theme = 'bootstrap';

        $stateProvider
            .state('app.shifts', {
                url: '/shifts',
                template: '<div ui-view></div>',
                abstract: true,
                data: {
                    auth: { allowAnonymous: false, roles: [] },
                    pageTitle: 'Shifts'
                }
            }).state('app.shifts.create', {
                url: '/new',
                templateUrl: 'App/dist/Views/Shifts/create/create.html',
                controller: 'shiftsCreateController',
                params: { date: null, employeeId: null },
                data: {
                    pageTitle: 'Create Shift',
                    breadcrumbs: ['app.shifts.list']    //'app.dashboard.home', 
                },
                resolve: {
                    dateRangePicker: resolveDateRangePicker,
                    datePicker: resolveDatePicker, 
                    select2: resolveSelect2
                }
            }).state('app.shifts.list', {
                url: '',
                templateUrl: 'App/dist/Views/Shifts/list/list.html',
                controller: 'shiftsListController',
                data: {
                    auth: { allowAnonymous: false, roles: [] },
                    pageTitle: 'Shifts',
                    breadcrumbs: ['app.dashboard.home']
                },
                resolve: {
                    dateRangePicker: resolveDateRangePicker,
                    switchery: resolveSwitchery,
                    //bootstrapMenu: resolveBootstrapMenu,
                    jqueryContextMenu: resolveJqueryContextMenu
                }
            }).state('app.shifts.view', {
                url: '/{id}',
                templateUrl: 'App/dist/Views/Shifts/view/view.html',
                controller: 'shiftsViewController',
                data: {
                    pageTitle: 'Shift Details',
                    breadcrumbs: ['app.dashboard.home', 'app.shifts.list']
                }
            }).state('app.shifts.update', {
                url: '/{id}/Update',
                templateUrl: 'App/dist/Views/Shifts/update/update.html',
                controller: 'shiftsUpdateController',
                data: {
                    pageTitle: 'Update Shift',
                    breadcrumbs: ['app.dashboard.home', 'app.shifts.list']
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

    resolveSwitchery.$inject = ['$ocLazyLoad'];
    function resolveSwitchery($ocLazyLoad) {
        return $ocLazyLoad.load({
            serie: true,
            files: [
                'App/ColorAdmin/assets/plugins/switchery/switchery.min.css',
                'App/ColorAdmin/assets/plugins/switchery/switchery.min.js'
            ]
        });
    }

    resolveBootstrapMenu.$inject = ['$ocLazyLoad'];
    function resolveBootstrapMenu($ocLazyLoad) {
        return $ocLazyLoad.load({
            serie: true,
            files: [
                'App/dist/js/BootstrapMenu.min.js'
            ]
        });
    }

    resolveJqueryContextMenu.$inject = ['$ocLazyLoad'];
    function resolveJqueryContextMenu($ocLazyLoad) {
        return $ocLazyLoad.load({
            serie: true,
            files: [
                'App/dist/css/jquery.contextMenu.min.css',
                'App/dist/js/jquery.contextMenu.min.js',
                'App/dist/js/jquery.ui.position.min.js'
            ]
        });
    }

})();
