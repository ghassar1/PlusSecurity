(function () {
    'use strict';

    angular.module('app.employees.groups')
        .controller('employeeGroupsCreateController', createController);

    createController.$inject = ['$scope', '$rootScope', '$state', '$localStorage', 'employeeGroupsService'];

    function createController($scope, $rootScope, $state, $localStorage, service) {
        
        var vm = $scope;
        vm.model = { roles: [] };
        vm.formData = { name: '' };

        vm.init = init;

        vm.validate = validate;
        vm.create = create;

        vm.init();

        function init() {
        }

        function create(form) {
            if (!validate(form)) return;

            service.create(vm.formData).then(function (data) {
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
