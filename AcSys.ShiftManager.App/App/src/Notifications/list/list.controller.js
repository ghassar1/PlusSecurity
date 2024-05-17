(function () {
    'use strict';

    angular.module('app.notifications')
        .controller('notificationsListController', listController);

    listController.$inject = ['$scope', '$rootScope', '$state', '$localStorage', 'notificationsService'];

    function listController($scope, $rootScope, $state, $localStorage, service) {

        var vm = $scope;

        vm.init = init;

        vm.list = list;
        vm.del = del;
        vm.markAsUnread = markAsUnread;

        vm.model = {};
        //vm.model = data;
        console.log(vm.model);
        vm.query = { searchCriteria: '', pageNo: 1, pageSize: 10, sortColumn: 'Date', sortType: 1 };
        vm.pageSizes = [1, 10, 25, 50, 100, 200];

        vm.pageChanged = pageChanged;
        vm.pageSizeChanged = pageSizeChanged;

        vm.dtOptions = {
            paginationType: 'full_numbers',
            //displayLength: 2,
            paging: false,
            ordering: true,
            info: false,
            searching: false
        };

        vm.init();

        function init() {
            vm.list();
            
            //$rootScope.notifications.message({ title: 'custom message title', message: ['test message', 'test message two'] });
            //$rootScope.notifications.error({ title: 'custom error title', message: ['test error', 'test error two'] });
            
            //$rootScope.notifications.error({ message: 'test error', title: 'test error two' });
            //$rootScope.notifications.error(['test error', 'test error two']);
            //$rootScope.notifications.message(['test message', 'test message two']);
            
            //$rootScope.breadcrumbs.set([
            //    { text: 'Home', href: $state.href('app.dashboard.home') },
            //    { text: 'Notifications', href: $state.href('app.notifications.list') },
            //    { text: 'List', href: '' }
            //]);
        }

        function list() {
            console.log($rootScope.User.hasAnyRole('SuperAdmin, Admin'));
            //var listMethod = $rootScope.User.hasAnyRole('SuperAdmin, Admin') ? service.list : service.listMine;
            var listMethod = service.listMine;
            listMethod(vm.query).then(function (data) {
                vm.model = data;
            }, function (error) { });
        }

        function del(id) {
            service.del(id).then(function (data) { }, function (error) { });
        }

        function markAsUnread(id) {
            service.markAsUnread(id).then(function () {
                vm.list();
                $rootScope.refreshHeaderNotifications();
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
