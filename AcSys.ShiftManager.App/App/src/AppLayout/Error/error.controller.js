/* -------------------------------
   CONTROLLER - Error General
------------------------------- */
(function () {
    'use strict';
    angular.module('app.core')
        .controller('errorController', errorController);

    errorController.$inject = ['$scope', '$rootScope', '$state', '$stateParams', '$location'];
    function errorController($scope, $rootScope, $state, $stateParams, $location) {

        var vm = $scope;
        vm.init = init;
        vm.getDashboardUrl = getDashboardUrl;

        vm.init();

        function init() {

            vm.isNotFoundError = $state.$current.name === 'app.error.notfound';
            vm.isAuthError = $state.$current.name === 'app.error.unauth';

            vm.error = $stateParams.error;

            if (vm.isNotFoundError) {

                $location.replace();

                //vm.path = $location.path();
                vm.path = $stateParams.path;
                if (!vm.path) {
                    //$state.go('app.dashboard.home');
                    var LoggedInUser = $rootScope.User;
                    LoggedInUser.goToHome();
                }
            }

            //if (vm.isAuthError) { }

            if (vm.error) {

                if (typeof vm.error === 'string') {
                    vm.errorMessage = vm.error;
                    
                } else if (vm.error instanceof Error) {
                    vm.errorName = vm.error.name;
                    vm.errorMessage = vm.error.message;

                    //if (vm.error instanceof AuthError) {
                    //} else {
                    //}

                } else {
                    if (vm.error.message)
                        vm.errorMessage = vm.error.message;
                    else if (vm.error.exceptionType)
                        vm.errorMessage = vm.error.exceptionType;
                    else if (vm.error.exceptionMessage)
                        vm.errorMessage = vm.error.exceptionMessage;
                }
            }

            if (vm.errorMessage) {
                $rootScope.notifications.notify(vm.errorMessage, vm.errorName);
            }
        }

        function getDashboardUrl() {

            var href = '#';

            if ($rootScope.User.hasAnyRole('SuperAdmin, Admin'))
                href = $state.href('app.dashboard.home');
            else if ($rootScope.User.hasAnyRole('RecManager'))
                href = $state.href('app.employees.list');
            else if ($rootScope.User.hasAnyRole('HRManager, Employee'))
                href = $state.href('app.shifts.list');

            console.debug(href);
            return href;
        }
    }
})();
