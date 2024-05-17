(function () {
    'use strict';

    angular.module('app', ['app.core', 'app.account', 'app.dashboard', 'app.users', 'app.activityLogs', 'app.notifications', 'app.messages', 'app.shifts', 'app.employees', 'app.reports'])
        .config(configAppModule);

    configAppModule.$inject = ['$stateProvider', '$urlRouterProvider'];
    function configAppModule($stateProvider, $urlRouterProvider) {

        // Global namespace for the app
        //window.app = function () { };

        // having a secure state as default causes infinite $digest Loop issue: https://docs.angularjs.org/error/$rootScope/infdig?p0=10&p1=%5B%5D
        // Explained more at: https://github.com/angular-ui/ui-router/issues/1699
        //$urlRouterProvider.otherwise('/app/dashboard/home');
        
        $urlRouterProvider.otherwise(function ($injector, $location) {

            //$location.path('/NotFound');
            //$state.go('NotFound');

            // An array with annotated injections cannot be passed to 'otherwise'. It only accepts a function with $injector 
            // and $location services as arguments. So $state cannot be passed as an argument to this function and has to be 
            // resolved using $injector. Issue explained here: https://github.com/angular-ui/ui-router/issues/1742
            $injector.invoke(['$rootScope', '$state', '$localStorage', 'accountService', function ($rootScope, $state, $localStorage, accountService) {

                var path = $location.path();
                console.log('Not Found (404): ' + path);
                if (path && path !== '/') {
                    //$state.transitionTo('NotFound');
                    //$state.go('NotFound');
                    $state.go('app.error.notfound', { path: path });
                } else {
                    console.log('Path is empty!');

                    var LoggedInUser = $rootScope.User;
                    if (LoggedInUser.hasLoggedIn()) {   //if ($localStorage.authData) {

                        console.log('User is logged in!');

                        //$state.go('app.dashboard.home');

                        if (LoggedInUser.hasUserData()) {

                            console.log('User data available!');

                            //if (LoggedInUser.hasAnyRole('SuperAdmin, Admin')) {
                            //    console.log('1');
                            //    $state.go('app.dashboard.home');
                            //} else if (LoggedInUser.hasAnyRole('RecManager')) {
                            //    console.log('2');
                            //    $state.go('app.employees.list');
                            //} else if (LoggedInUser.hasAnyRole('HRManager, Employee')) {
                            //    console.log('3');
                            //    $state.go('app.shifts.list');
                            //}
                            LoggedInUser.goToHome();

                        } else {

                            console.log('User data not available!');

                            accountService.getLoggedInUserDetails().then(function (data) {
                                console.log('Got user data!');
                                LoggedInUser.setUserData(data);
                                $rootScope.Navigation.Build();
                                LoggedInUser.goToHome();
                            }, function (error) {
                                console.log('Error Getting Logged In User Details.');
                                event.preventDefault();
                                LoggedInUser.logout();
                                return $state.go('app.error.app', { error: error });
                            });
                        }
                        console.log('4');
                    } else {
                        console.log('User is not logged in!');
                        $state.go('app.account.login');
                    }
                }
            }]);
        });

        $stateProvider
            .state('app', {
                url: '/app',
                templateUrl: 'App/dist/Views/AppLayout/Base/app.html',
                abstract: true,
                resolve: { authUser: resolveAuthUser }
            }).state('app.error', {
                url: '/error',
                template: '<div ui-view></div>',
                abstract: true
            }).state('app.error.app', {
                url: '/app',
                templateUrl: 'App/dist/Views/AppLayout/Error/error.app.html',
                params: { error: null },
                data: {
                    auth: { allowAnonymous: true },
                    pageTitle: 'Application Error',
                    layout: { headerAndSidebar: false, header: false, sidebar: false }
                }
            }).state('app.error.notfound', {
                url: '/notfound?{path}',
                templateUrl: 'App/dist/Views/AppLayout/Error/error.404.html',
                params: { error: null },
                data: {
                    auth: { allowAnonymous: true },
                    pageTitle: 'Page Not Found!',
                    layout: { headerAndSidebar: false, header: false, sidebar: false }
                }
            }).state('app.error.unauth', {
                url: '/unauthorized',
                templateUrl: 'App/dist/Views/AppLayout/Error/error.unauth.html',
                params: { error: null },
                data: {
                    auth: { allowAnonymous: true },
                    pageTitle: 'Not Authorized!',
                    layout: { headerAndSidebar: false, header: false, sidebar: false }
                }
            });
    }

    resolveAuthUser.$inject = ['$rootScope', '$q', 'accountService'];
    function resolveAuthUser($rootScope, $q, accountService) {

        //return $q.resolve(null);

        //$rootScope.userData = data;
        //$localStorage.userData = data;

        var LoggedInUser = $rootScope.User;

        if (LoggedInUser.hasNotLoggedIn()) return $q.resolve(LoggedInUser.getUserData());

        if (LoggedInUser.hasUserData()) return $q.resolve(LoggedInUser.getUserData());

        //console.debug('App: Get User Data.');

        return accountService.getLoggedInUserDetails().then(function (data) {
            //console.debug('App: Got User Data: ', data);

            LoggedInUser.setUserData(data);

            $rootScope.Navigation.Build();

            return $q.resolve(LoggedInUser.getUserData());

        }, function (error) {
            console.log('Error Getting Logged In User Details.');
            LoggedInUser.logout();
            //return $q.reject(error);
            return $q.resolve(null);
        });
    }

    //resolve.$inject = ['$ocLazyLoad'];
    function resolve() {

        //return null;
        console.log('Resolving gritter...');

        return $ocLazyLoad.load({
            serie: true,
            files: [
                'App/ColorAdmin/assets/plugins/gritter/css/jquery.gritter.css',
                'App/ColorAdmin/assets/plugins/gritter/js/jquery.gritter.min.js'
                //'dist/js/stacktrace.min.js'
            ]
        });

        //return $ocLazyLoad.load({
        //    serie: true,
        //    files: [
        //        'App/ColorAdmin/assets/plugins/jquery-jvectormap/jquery-jvectormap-1.2.2.css',
        //        'App/ColorAdmin/assets/plugins/bootstrap-calendar/css/bootstrap_calendar.css',
        //        'App/ColorAdmin/assets/plugins/gritter/css/jquery.gritter.css',
        //        'App/ColorAdmin/assets/plugins/morris/morris.css',
        //        'App/ColorAdmin/assets/plugins/morris/raphael.min.js',
        //        'App/ColorAdmin/assets/plugins/morris/morris.js',
        //        'App/ColorAdmin/assets/plugins/jquery-jvectormap/jquery-jvectormap-1.2.2.min.js',
        //        'App/ColorAdmin/assets/plugins/jquery-jvectormap/jquery-jvectormap-world-merc-en.js',
        //        'App/ColorAdmin/assets/plugins/bootstrap-calendar/js/bootstrap_calendar.min.js',
        //        'App/ColorAdmin/assets/plugins/gritter/js/jquery.gritter.min.js'
        //    ]
        //});
    }

})();
