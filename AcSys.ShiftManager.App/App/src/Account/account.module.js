(function () {
    'use strict';

    angular.module('app.account', ['app.core'])
        .config(configModule)
        .run(runModule);

    function runModule() {
    }

    configModule.$inject = ['$stateProvider', '$urlRouterProvider'];

    function configModule($stateProvider, $urlRouterProvider) {

        $stateProvider
            .state('app.account', {
                url: '/account',
                template: '<div ui-view></div>',
                abstract: true
                //resolve: { service: resolve }
            }).state('app.account.login', {
                url: '/login',
                data: {
                    auth: { allowAnonymous: true },
                    pageTitle: 'Login'
                    //layout: { headerAndSidebar: false, header: false, sidebar: false }
                },
                templateUrl: 'App/dist/Views/Account/login/login.html'
            }).state('app.account.logout', {
                url: '/logout',
                //controller: 'logoutController',
                //template: 'Loging out...'
                templateUrl: 'App/dist/Views/Account/logout/logout.html',
                data: {
                    auth: { allowAnonymous: true },
                    pageTitle: 'Logout'
                    //layout: { headerAndSidebar: false, header: false, sidebar: false }
                }
            }).state('app.account.profile', {
                url: '/profile',
                templateUrl: 'App/dist/Views/Users/view/view.html',
                controller: 'usersViewController',
                data: {
                    pageTitle: 'Profile',
                    breadcrumbs: ['app.dashboard.home']
                }
            }).state('app.account.update', {
                url: '/profile/update',
                templateUrl: 'App/dist/Views/Users/update/update.html',
                controller: 'usersUpdateController',
                data: {
                    pageTitle: 'Update Profile',
                    breadcrumbs: ['app.dashboard.home', 'app.account.profile']
                },
                resolve: { data: resolveDatePicker }
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

})();
