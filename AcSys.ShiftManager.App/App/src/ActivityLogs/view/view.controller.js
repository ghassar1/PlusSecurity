(function () {
    'use strict';

    angular.module('app.activityLogs')
        .controller('activityLogsViewController', viewController);

    viewController.$inject = ['$scope', '$rootScope', '$state', '$stateParams', '$localStorage', 'activityLogsService'];

    function viewController($scope, $rootScope, $state, $stateParams, $localStorage, service) {

        var vm = $scope;
        vm.model = {};
        vm.init = init;
        vm.get = get;
        vm.del = del;
        
        vm.init();

        function init() {
            vm.id = $stateParams.id;
            vm.get(vm.id);
        }

        function get(id) {

            service.get(id).then(function (data) {
                vm.model = data;
            }, function (error) { });
        }

        function del() {
            service.del(vm.id).then(function (data) {
                $state.go('app.activityLogs.list');
            }, function (error) { });
        }
    }
})();
