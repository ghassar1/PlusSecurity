(function () {
    'use strict';

    angular.module('app.users')
        .controller('usersListController', listController);

    listController.$inject = ['$scope', '$rootScope', '$state', '$localStorage', 'usersService', 'employeesService', 'DTOptionsBuilder', 'DTColumnDefBuilder'];

    function listController($scope, $rootScope, $state, $localStorage, usersService, employeesService, DTOptionsBuilder, DTColumnDefBuilder) {

        //(function isGuid() {
        //    var value = '67560688-d6a9-e611-9c3b-28924a44c492';
        //    var regex = /[a-f0-9]{8}(?:-[a-f0-9]{4}){3}-[a-f0-9]{12}/i;
        //    var match = regex.exec(value);
        //    $rootScope.notifications.message((match !== null) + '');
        //})();

        var vm = $scope;

        var service = usersService;

        vm.getNewUrl = getNewUrl;
        vm.getDetailsUrl = getDetailsUrl;
        vm.init = init;
        vm.list = list;

        vm.model = {};
        //vm.query = { searchCriteria: '', pageNo: 1, pageSize: 10, sortColumn: 'Date', sortType: 1 };
        vm.query = { searchCriteria: '', pageNo: 1, pageSize: 10, sortColumn: 'LASTNAME', sortType: 1 };
        vm.pageSizes = [10, 25, 50, 100, 200];

        vm.pageChanged = pageChanged;
        vm.pageSizeChanged = pageSizeChanged;

        // http://l-lin.github.io/angular-datatables/#/api
        vm.dtOptions = {
            paginationType: 'full_numbers',
            //displayLength: 2,
            paging: false,
            ordering: true,
            info: false,
            searching: false
        };

        //vm.dtOptions = DTOptionsBuilder.newOptions()
        //    .withPaginationType('full_numbers')
        //    .withDisplayLength(2)
        //    .withOption('paging', false)
        //    .withOption('ordering', true)
        //    .withOption('info', false)
        //    .withOption('searching', false);

        vm.dtColumnDefs = [
            //DTColumnDefBuilder.newColumnDef(0),
            //DTColumnDefBuilder.newColumnDef(1),//.notVisible(),
            //DTColumnDefBuilder.newColumnDef(2),//.notSortable()
            //DTColumnDefBuilder.newColumnDef(3),
            //DTColumnDefBuilder.newColumnDef(4),
            //DTColumnDefBuilder.newColumnDef(5)
        ];

        vm.init();

        function init() {

            //throw new AuthError();

            //$state.go('app.employees1.list', { a: 'AA', b: 'BB' });

            vm.isUsersModule = $state.$current.name === 'app.users.list';
            vm.isEmployeesModule = $state.$current.name === 'app.employees.list';
            service = vm.isEmployeesModule ? employeesService : usersService;

            vm.list();

            //throw new Error('error....!');
        }

        function getNewUrl() {
            return vm.isEmployeesModule ?
                $state.href('app.employees.create') :
                $state.href('app.users.create');
        }

        function getDetailsUrl(item) {
            return vm.isEmployeesModule ?
                $state.href('app.employees.view', { id: item.id }) :
                $state.href('app.users.view', { id: item.id });
        }

        function list() {
            service.listAll(vm.query).then(function (data) {
                vm.model = data;
            }, function (error) { });
        }

        function pageSizeChanged() {
            vm.query.pageNo = 1;
            list();
        }

        function pageChanged() {
            list();
        }
    }
})();
