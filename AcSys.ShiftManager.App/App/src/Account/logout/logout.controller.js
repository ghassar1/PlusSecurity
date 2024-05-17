(function () {
    'use strict';

    angular.module('app.account')
        .controller('logoutController', logoutController);

    logoutController.$inject = ['$scope', '$rootScope', '$state', '$localStorage', 'accountService'];

    function logoutController($scope, $rootScope, $state, $localStorage, accountService) {

        var vm = $scope;
        vm.init = init;

        vm.init();

        function init() {

            //delete $rootScope.userData;
            //delete $rootScope.authData;
            //delete $localStorage.authData;

            $rootScope.User.logout();

            $rootScope.Navigation.Build();

            //$state.go('app.account.login');
        }
    }

})();
