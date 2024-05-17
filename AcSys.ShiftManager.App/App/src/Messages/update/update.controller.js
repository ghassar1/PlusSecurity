(function () {
    'use strict';

    angular.module('app.messages')
        .controller('messagesUpdateController', updateController);

    updateController.$inject = ['$scope', '$rootScope', '$state', '$localStorage', 'messagesService'];

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
                    $state.go('app.messages.inbox.list');
                }, function (error) { });
            }
        };
    }
})();
