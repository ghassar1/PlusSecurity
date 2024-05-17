(function () {
    'use strict';

    angular.module('app.employees.groups')
        .controller('employeeGroupsViewController', viewController);

    viewController.$inject = ['$scope', '$rootScope', '$state', '$stateParams', '$localStorage', 'employeeGroupsService', 'employeesService'];

    function viewController($scope, $rootScope, $state, $stateParams, $localStorage, service) {

        var vm = $scope;
        vm.id = null;
        vm.model = {};
        vm.init = init;

        vm.get = get;
        vm.del = del;
        vm.activate = activate;
        vm.deactivate = deactivate;

        vm.init();

        function init() {
            vm.id = $stateParams.id;
            vm.get();
        }

        function get() {
            service.get(vm.id).then(function (data) {
                vm.model = data;
            }, function (error) { });
        }

        function del() {
            service.del(vm.id).then(function (data) {
                $state.go('app.employees.groups.list');
            }, function (error) { });
        }

        function activate() {
            service.activate(vm.id).then(function (data) {
                $rootScope.notifications.message('EmployeeGroup activated successfully!');
                vm.init();
            }, function (error) { });
        }

        function deactivate() {
            service.deactivate(vm.id).then(function (data) {
                $rootScope.notifications.message('EmployeeGroup deactivated successfully!');
                vm.init();
            }, function (error) { });
        }
    }
})();
