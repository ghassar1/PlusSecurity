(function () {
    'use strict';

    angular.module('app.notifications')
        .controller('notificationsCreateController', createController);

    createController.$inject = ['$scope', '$rootScope', '$state', '$localStorage', 'notificationsService', 'usersService'];

    function createController($scope, $rootScope, $state, $localStorage, service, usersService) {

        var vm = $scope;
        vm.model = {};
        vm.formData = { title: '', text: '', recipients: [] };
        vm.recipientRoles = [];
        vm.init = init;

        vm.validate = validate;
        vm.create = create;
        vm.getRoles = getRoles;
        vm.onReady = onReady;
        vm.loadTags = loadTags;

        vm.init();

        function init() {
            //vm.onReady();
            vm.getRoles();
        }

        function onReady() {
            angular.element(document).ready(function () {
                //$('#jquery-tagIt-recipients').tagit({
                //    availableTags: ['c++', 'java', 'php', 'javascript', 'ruby', 'python', 'c']
                //});
            });
        }

        function getRoles() {
            usersService.getRoles().then(function (data) {
                console.log(data);
                vm.roles = data;

                //vm.roleNames = [];
                //angular.forEach(vm.roles, function (role) {
                //    vm.roleNames.push(role.name);
                //});

                //$('#jquery-tagIt-recipients').tagit({
                //    fieldName: 'roles',
                //    showAutocompleteOnFocus: true,
                //    autocomplete: { delay: 0, minLength: 0 },
                //    removeConfirmation: false,
                //    caseSensitive: true,
                //    allowDuplicates: false,
                //    allowSpaces: true,
                //    readOnly: false,
                //    tagLimit: null,
                //    placeholderText: null,
                //    availableTags: vm.roleNames
                //});
            }, function (error) { });
            console.log(vm.roles);
        }

        function create(form) {

            //vm.formData.recipients = $('#jquery-tagIt-recipients').tagit('assignedTags');

            console.debug(vm.formData);
            
            if (!validate(form)) return;

            vm.formData.recipients = getRecipientsArray(vm.recipientRoles);

            service.create(vm.formData).then(function (data) {
                $state.go('app.notifications.list');
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

        function getRecipientsArray(recipientRoles) {

            var arr = [];
            angular.forEach(recipientRoles, function (role) {
                arr.push(role.name);
            });
            return arr;
        }

        function loadTags(query) {

            vm.selectedRoles = [];

            angular.forEach(vm.roles, function (role) {

                if (role.name.toUpperCase().indexOf(query.toUpperCase()) !== -1) {
                    vm.selectedRoles.push(role);
                }
            });

            return vm.selectedRoles;
        }
    }
})();
