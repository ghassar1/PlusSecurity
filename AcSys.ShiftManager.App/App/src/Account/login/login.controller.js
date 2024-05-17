(function () {
    'use strict';

    angular.module('app.account')
        .controller('loginController', loginController);

    loginController.$inject = ['$scope', '$rootScope', '$state', '$localStorage', 'accountService'];

    function loginController($scope, $rootScope, $state, $localStorage, accountService) {

        var vm = $scope;
        vm.model = {};
        vm.init = init;
        
        vm.init();

        function init() {
            //delete $rootScope.userData;
            //delete $rootScope.authData;
            //delete $localStorage.authData;
            $rootScope.User.logout();
        }

        vm.submitForm = function (form) {
            if (form.$valid) {

                console.debug(vm.model);
                accountService.authenticate(vm.model).then(function (data) {

                    console.debug(data);
                    //$localStorage.authData = data;
                    //$rootScope.authData = data;

                    var LoggedInUser = $rootScope.User;
                    LoggedInUser.login(data);

                    //$rootScope.$broadcast('update_menu');

                    //console.debug('appStateChangeStart: Get User Data.');
                    accountService.getLoggedInUserDetails().then(function (data) {

                        LoggedInUser.setUserData(data);
                        $rootScope.Navigation.Build();

                        //if (LoggedInUser.hasAnyRole('SuperAdmin, Admin')) {
                        //    $state.go('app.dashboard.home');
                        //} else if (LoggedInUser.hasAnyRole('RecManager')) {
                        //    $state.go('app.employees.list');
                        //} else if (LoggedInUser.hasAnyRole('HRManager, Employee')) {
                        //    $state.go('app.shifts.list');
                        //}
                        LoggedInUser.goToHome();

                    }, function (error) {
                        console.log('Error Getting Logged In User Details.');
                        return $state.go('app.error.app', { error: error });
                    });

                }, function (error) { });
            }
        };
    }
})();
