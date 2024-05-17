(function () {
    'use strict';

    angular.module('app.messages')
        .controller('messagesListController', listController);

    listController.$inject = ['$scope', '$rootScope', '$state', '$localStorage', 'messagesService', 'DTOptionsBuilder', 'DTColumnDefBuilder', 'DTDefaultOptions', '$timeout'];

    function listController($scope, $rootScope, $state, $localStorage, service, DTOptionsBuilder, DTColumnDefBuilder, DTDefaultOptions, $timeout) {

        var vm = $scope;

        vm.init = init;
        vm.list = list;
        vm.del = del;
        vm.markAsUnread = markAsUnread;
        vm.inbox = true;

        vm.model = {};
        vm.query = { searchCriteria: '', pageNo: 1, pageSize: 10, sortColumn: 'FirstName', sortType: 1 };
        vm.pageSizes = [10, 25, 50, 100, 200];

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
            vm.inbox = $state.$current.name === 'app.messages.inbox.list';
            vm.sent = $state.$current.name === 'app.messages.sent.list';

            vm.list();
        }

        function list() {

            //if (vm.inbox) {
            //    service.list().then(function (data) {
            //        vm.model.list = data;
            //    }, function (error) { });
            //} else {
            //    service.listSent().then(function (data) {
            //        vm.model.list = data;
            //    }, function (error) { });
            //}

            var callback = vm.inbox ? service.list(vm.query) : service.listSent(vm.query);
            callback.then(function (data) {
                console.log(data);
                vm.model = data;
                //vm.model.list = data.data;
                //vm.totalItems = data.totalItems;
                //vm.filteredItems = data.filteredItems;
                ////vm.totalPages = vm.totalItems / data.query.pageSize;
                //vm.totalPages = Math.ceil(vm.totalItems / data.query.pageSize);
            });
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
