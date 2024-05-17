(function () {
    'use strict';

    angular.module('app.users')
        .controller('usersViewController', viewController);

    viewController.$inject = ['$scope', '$rootScope', '$state', '$stateParams', '$localStorage', 'usersService', 'employeesService'];

    function viewController($scope, $rootScope, $state, $stateParams, $localStorage, usersService, employeesService) {

        var service = usersService;
        var vm = $scope;
        vm.id = null;
        vm.model = {};
        vm.init = init;

        vm.getUpdateUrl = getUpdateUrl;
        vm.getBackUrl = getBackUrl;

        vm.get = get;
        vm.del = del;

        vm.activate = activate;
        vm.deactivate = deactivate;

        vm.dateOfBirthPopup = {
            opened: false,
            dateFormat: 'dd/MM/yyyy',
            altInputFormats: ['M!/d!/yyyy'],
            dateOptions: {
                //dateDisabled: disabled,
                formatYear: 'yyyy',
                //maxDate: new Date(),//new Date(2100, 12, 31),
                //minDate: new Date(1900, 1, 1),//new Date(),
                startingDay: 1
            }
        };

        // Disable weekend selection
        function disabled(data) {
            var date = data.date,
              mode = data.mode;
            return mode === 'day' && (date.getDay() === 0 || date.getDay() === 6);
        }

        vm.dateOfBirthPopup.open = function () {
            vm.dateOfBirthPopup.opened = true;
            console.log('opened');
        };

        vm.init();

        function init() {

            vm.isProfilePage = $state.$current.name === 'app.account.profile';
            vm.id = vm.isProfilePage ? $rootScope.User.Id : $stateParams.id;
            
            vm.isUsersModule = $state.$current.name === 'app.users.view';
            vm.isEmployeesModule = $state.$current.name === 'app.employees.view';
            service = vm.isEmployeesModule ? employeesService : usersService;

            vm.canManage = vm.isProfilePage ? true :
                vm.isEmployeesModule ? $rootScope.User.hasAnyRole('RecManager, HRManager') :
                vm.isUsersModule ? $rootScope.User.hasAnyRole('SuperAdmin, Admin') :
                false;

            get();
        }

        function get() {
            service.get(vm.id).then(function (data) {
                vm.model = data;
            }, function (error) { });
        }

        function del() {
            service.del(vm.id).then(function (data) {
                //$state.go('app.users.list');
                init();
            }, function (error) { });
        }

        function activate() {
            service.activate(vm.id).then(function (data) {
                $rootScope.notifications.message('User activated successfully!');
                init();
            }, function (error) { });
        }

        function deactivate() {
            service.deactivate(vm.id).then(function (data) {
                $rootScope.notifications.message('User deactivated successfully!');
                init();
            }, function (error) { });
        }

        function getUpdateUrl() {
            return vm.isProfilePage ? $state.href('app.account.update')
                : vm.isEmployeesModule ? $state.href('app.employees.update', { id: vm.id })
                : $state.href('app.users.update', { id: vm.id });
        }

        function getBackUrl() {
            return vm.isProfilePage ? $state.href('app.dashboard.home')
                : vm.isEmployeesModule ? $state.href('app.employees.list')
                : $state.href('app.users.list');
        }
    }
})();
