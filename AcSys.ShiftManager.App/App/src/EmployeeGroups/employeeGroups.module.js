(function () {
    'use strict';

    angular.module('app.employees.groups', ['app.core', 'ui.bootstrap', 'ngAnimate', 'ngSanitize'])
        .config(['$stateProvider', '$urlRouterProvider', configModule])
        .run([runModule]);

    function configModule($stateProvider, $urlRouterProvider) {

        $stateProvider
            .state('app.employees.groups', {
                url: '/Employees/Groups',
                template: '<div ui-view></div>',
                abstract: true,
                data: {
                    auth: { allowAnonymous: false, roles: ['RecManager'] },
                    pageTitle: 'Employee Groups'
                }
            }).state('app.employees.groups.create', {
                url: '/new',
                templateUrl: 'App/dist/Views/EmployeeGroups/create/create.html',
                controller: 'employeeGroupsCreateController',
                data: {
                    auth: { allowAnonymous: false, roles: ['RecManager'] },
                    pageTitle: 'Create',
                    breadcrumbs: ['app.dashboard.home', 'app.employees.groups.list']
                },
                resolve: { }
            }).state('app.employees.groups.list', {
                url: '',
                templateUrl: 'App/dist/Views/EmployeeGroups/list/list.html',
                controller: 'employeeGroupsListController',
                data: {
                    auth: { allowAnonymous: false, roles: ['RecManager'] },
                    pageTitle: 'Employee Groups',
                    breadcrumbs: ['app.dashboard.home']
                },
                resolve: { }
            }).state('app.employees.groups.update', {
                url: '/{id}/update',
                templateUrl: 'App/dist/Views/EmployeeGroups/update/update.html',
                controller: 'employeeGroupsUpdateController',
                data: {
                    auth: { allowAnonymous: false, roles: ['RecManager'] },
                    pageTitle: 'Update',
                    breadcrumbs: ['app.dashboard.home', 'app.employees.groups.list']
                },
                resolve: { }
            });
            //.state('app.employees.groups.view', {
            //    //url: '/{id:[a-f0-9]{8}(?:-[a-f0-9]{4}){3}-[a-f0-9]{12}}',    // Regex in url enforces Id to be a Guid, otherwise forwards to NotFound (404)
            //    url: '/{id}',
            //    templateUrl: 'App/dist/Views/EmployeeGroups/view/view.html',
            //    controller: 'employeeGroupsViewController',
            //    data: {
            //        pageTitle: 'Details',
            //        breadcrumbs: ['app.dashboard.home', 'app.employees.groups.list']
            //    },
            //    resolve: { }
            //});
    }

    runModule.$inject = [];
    function runModule() {
        // ...
    }

})();
