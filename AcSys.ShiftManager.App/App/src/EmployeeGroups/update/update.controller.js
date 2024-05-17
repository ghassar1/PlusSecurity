(function () {
    'use strict';

    angular.module('app.employees.groups')
        .controller('employeeGroupsUpdateController', udpateController);

    udpateController.$inject = ['$scope', '$rootScope', '$state', '$stateParams', '$localStorage', 'employeeGroupsService'];

    function udpateController($scope, $rootScope, $state, $stateParams, $localStorage, service) {

        var vm = $scope;
        vm.id = null;
        vm.model = {};
        vm.roles = [];
        vm.formData = { name: '' };

        vm.init = init;
        vm.get = get;

        vm.validate = validate;
        vm.update = update;
                
        vm.init();

        function init() {
            vm.id = $stateParams.id;
            vm.get();
        }

        function get() {
            service.get(vm.id).then(function (data) {
                vm.model = data;
                vm.formData = angular.copy(vm.model);
            }, function (error) { });
        }
        
        function update(form) {
            
            if (!validate(form)) return;

            service.update(vm.id, vm.formData).then(function (data) {
                //$state.go('app.employees.groups.view', { id: vm.id });
                $state.go('app.employees.groups.list');
            }, function (error) { });
        }

        function validate(form) {

            $rootScope.notifications.clear();
            $rootScope.FormValidator.clear(form);

            var isValid = $rootScope.FormValidator.validate(form);
            if (!isValid)
                $rootScope.notifications.error('Please fill in all the required fields.');
            return isValid;
        }
    }
})();
