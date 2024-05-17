(function () {
    'use strict';

    angular.module('app.employees.groups')
        .controller('employeeGroupsListController', listController);

    listController.$inject = ['$scope', '$rootScope', '$state', '$localStorage', 'employeeGroupsService', 'DTOptionsBuilder', 'DTColumnDefBuilder'];

    function listController($scope, $rootScope, $state, $localStorage, service, DTOptionsBuilder, DTColumnDefBuilder) {

        var vm = $scope;

        vm.model = {};
                
        // http://l-lin.github.io/angular-datatables/#/api
        vm.dtOptions = {
            paginationType: 'full_numbers',
            //displayLength: 2,
            paging: false,
            ordering: true,
            info: false,
            searching: false
        };

        vm.list = list;

        vm.del = del;
        vm.askToDelete = askToDelete;
        vm.okDelete = okDelete;
        vm.cancelDelete = cancelDelete;

        vm.init = init;

        vm.init();

        function init() {
            vm.list();
        }

        function list() {
            service.list(vm.query).then(function (data) {
                vm.model = data;
                console.log(vm.model);
            }, function (error) { });
        }

        var curId = null;
        function askToDelete(id) {
            curId = id;
            $('#modal-ask-to-delete').modal('show');
        }

        function okDelete() {
            if (angular.copy(curId)) {
                del(curId);
                curId = null;
            }
        }

        function cancelDelete() {
            curId = null;
        }

        function del(id) {
            console.log(id);
            service.del(id).then(function (data) {
                vm.list();
            }, function (error) { });
        }
    }
})();
