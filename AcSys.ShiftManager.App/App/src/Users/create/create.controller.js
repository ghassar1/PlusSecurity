(function () {
    'use strict';

    angular.module('app.users')
        .controller('usersCreateController', createController);

    createController.$inject = ['$scope', '$rootScope', '$state', '$localStorage', 'usersService', 'employeesService', 'employeeGroupsService'];

    function createController($scope, $rootScope, $state, $localStorage, usersService, employeesService, employeeGroupsService) {

        var service = usersService;

        var vm = $scope;
        vm.model = { roles: [] };
        vm.roles = [];
        vm.employeeGroups = [];
        vm.formData = { firstName: '', lastName: '', email: '', dateOfBirth: '15/11/2016', mobile: '', phoneNumber: '', role: null };

        vm.getBackUrl = getBackUrl;
        vm.init = init;

        vm.validate = validate;
        vm.create = create;
        vm.getRoles = getRoles;
        vm.getEmployeeGroups = getEmployeeGroups;
        
        vm.citiesInEngland = ['Bath', 'Birmingham', 'Bradford', 'Brighton and Hove', 'Bristol', 'Cambridge', 'Canterbury', 'Carlisle', 'Chester', 'Chichester', 'Coventry', 'Derby', 'Durham', 'Ely', 'Exeter', 'Gloucester', 'Hereford', 'Kingston upon Hull', 'Lancaster', 'Leeds', 'Leicester', 'Lichfield', 'Lincoln', 'Liverpool', 'City of London', 'Manchester', 'Newcastle upon Tyne', 'Norwich', 'Nottingham', 'Oxford', 'Peterborough', 'Plymouth', 'Portsmouth', 'Preston', 'Ripon', 'Salford', 'Salisbury', 'Sheffield', 'Southampton', 'St Albans', 'Stoke-on-Trent', 'Sunderland', 'Truro', 'Wakefield', 'Wells', 'Westminster', 'Winchester', 'Wolverhampton', 'Worcester', 'York'];
        vm.citiesInWales = ['Bangor', 'Cardiff', 'Newport', 'St Davids', 'Swansea'];
        vm.citiesInScotland = ['Aberdeen', 'Dundee', 'Edinburgh', 'Glasgow', 'Inverness', 'Stirling'];
        vm.citiesInNorthernIreland = ['Armagh', 'Belfast', 'Londonderry', 'Lisburn', 'Newry'];

        vm.dateOfBirthPopup = {
            opened: false,
            dateFormat: 'dd/MM/yyyy',
            altInputFormats: ['M!/d!/yyyy'],
            dateOptions: {
                //dateDisabled: disabled,
                formatYear: 'yyyy',
                //maxDate: new Date(),//new Date(2100, 12, 31),
                //minDate: new Date(1900, 1, 1),//new Date(),
                startingDay: 1
            },
            open: function () {
                vm.dateOfBirthPopup.opened = true;
                console.log('opened');
            }
        };

        // Disable weekend selection
        function disabled(data) {
            var date = data.date,
              mode = data.mode;
            return mode === 'day' && (date.getDay() === 0 || date.getDay() === 6);
        }

        init();

        function init() {

            $('.default-select2').select2();

            vm.isUsersModule = $state.$current.name === 'app.users.create';
            vm.isEmployeesModule = $state.$current.name === 'app.employees.create';
            service = vm.isEmployeesModule ? employeesService : usersService;

            getRoles();

            if (vm.isEmployeesModule)
                vm.getEmployeeGroups();
        }

        function getBackUrl() {
            return vm.isEmployeesModule ?
                $state.href('app.employees.list') :
                $state.href('app.users.list');
        }

        function dateChagned(event) {
            console.log(event);
        }

        function getRoles() {

            usersService.getRoles().then(function (data) {

                var roles = data;

                if ($rootScope.User.hasAnyRole('SuperAdmin, Admin')) {
                    roles = removeRole(roles, 'Employee');
                }

                if ($rootScope.User.hasAnyRole('Admin, RecManager, HRManager')) {
                    roles = removeRole(roles, 'SuperAdmin');
                    roles = removeRole(roles, 'Admin');
                }

                if ($rootScope.User.hasAnyRole('RecManager, HRManager')) {
                    roles = removeRole(roles, 'RecManager');
                    roles = removeRole(roles, 'HRManager');
                }
                
                vm.roles = roles;

                angular.forEach(vm.roles, function (role) {
                    console.debug(role);
                    if ($rootScope.User.hasAnyRole('RecManager')) {

                        if (role.name === 'Employee') {
                            vm.formData.role = role;
                        }

                    }
                });
            }, function (error) { });
        }

        function removeRole(roles, roleName) {

            angular.forEach(roles, function (role) {
                if (role.name === roleName) {
                    var index = roles.indexOf(role);
                    console.debug(roles, role, index);
                    if (index > -1) {
                        roles.splice(index, 1);
                    }
                }
            });
            return roles;
        }

        function getEmployeeGroups() {
            employeeGroupsService.list().then(function (data) {
                vm.employeeGroups = data.items;
            }, function (error) { });
        }

        function create(form) {

            //$('#dateOfBirthField').trigger('input');

            if (!validate(form)) return;

            if (vm.formData.password) {
                if (!vm.formData.confirmPassword || vm.formData.password !== vm.formData.confirmPassword) {
                    $rootScope.notifications.error('Confirm password field should match password field.');
                    return;
                }
            }

            service.create(vm.formData).then(function (data) {
                if (vm.isEmployeesModule) {
                    $state.go('app.employees.list');
                } else {
                    $state.go('app.users.list');
                }
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
