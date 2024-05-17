(function () {
    'use strict';

    angular.module('app.notifications')
        .controller('notificationsUpdateController', updateController);

    updateController.$inject = ['$scope', '$rootScope', '$state', '$localStorage', 'notificationsService'];

    function updateController($scope, $rootScope, $state, $localStorage, service) {

        var vm = $scope;
        vm.model = {};
        vm.init = init;

        vm.init();

        function init() {
        }

        vm.update = function (form) {
            //$state.go('app.dashboard');
            if (form.$valid) {
                console.debug(vm.model);

                service.update(vm.model).then(function (data) {
                    $state.go('app.notifications.list');
                }, function (error) { });
            }
        };
    }
})();
