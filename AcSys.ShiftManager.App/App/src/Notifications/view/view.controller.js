(function () {
    'use strict';

    angular.module('app.notifications')
        .controller('notificationsViewController', viewController);

    viewController.$inject = ['$scope', '$rootScope', '$state', '$stateParams', '$localStorage', 'notificationsService', '$sce'];

    function viewController($scope, $rootScope, $state, $stateParams, $localStorage, service, $sce) {

        var vm = $scope;
        vm.model = {};
        vm.init = init;
        vm.get = get;
        vm.del = del;
        vm.markAsRead = markAsRead;

        vm.init();

        function init() {
            vm.id = $stateParams.id;
            vm.get(vm.id);
        }

        function get(id) {

            service.get(id).then(function (data) {

                vm.model = data;
                console.debug(vm.model);

                vm.model.textHtml = $sce.trustAsHtml(vm.model.text);

                vm.markAsRead();
            }, function (error) { });
        }

        function del() {
            service.del(vm.id).then(function (data) {
                $state.go('app.notifications.list');
            }, function (error) { });
        }

        function markAsRead() {
            service.markAsRead(vm.id).then(function () {
                $rootScope.refreshHeaderNotifications();
            }, function (error) { });
        }
    }
})();
