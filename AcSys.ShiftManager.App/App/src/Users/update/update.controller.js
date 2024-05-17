(function () {
    'use strict';

    angular.module('app.users')
        .controller('usersUpdateController', udpateController);

    udpateController.$inject = ['$scope', '$rootScope', '$state', '$stateParams', '$localStorage', 'usersService', 'employeesService', 'employeeGroupsService'];

    function udpateController($scope, $rootScope, $state, $stateParams, $localStorage, usersService, employeesService, employeeGroupsService) {

        var service = usersService;
        var vm = $scope;
        vm.id = null;
        vm.model = {};
        vm.roles = [];
        vm.employeeGroups = [];
        vm.formData = { firstName: '', lastName: '', email: '', dateOfBirth: '15/11/2016', mobile: '', phoneNumber: '', role: null };

        vm.init = init;
        vm.get = get;

        vm.validate = validate;
        vm.update = update;
        vm.getRoles = getRoles;
        vm.getEmployeeGroups = getEmployeeGroups;
        vm.getBackUrl = getBackUrl;

        vm.citiesInEngland = ['Bath', 'Birmingham', 'Bradford', 'Brighton and Hove', 'Bristol', 'Cambridge', 'Canterbury', 'Carlisle', 'Chester', 'Chichester', 'Coventry', 'Derby', 'Durham', 'Ely', 'Exeter', 'Gloucester', 'Hereford', 'Kingston upon Hull', 'Lancaster', 'Leeds', 'Leicester', 'Lichfield', 'Lincoln', 'Liverpool', 'City of London', 'Manchester', 'Newcastle upon Tyne', 'Norwich', 'Nottingham', 'Oxford', 'Peterborough', 'Plymouth', 'Portsmouth', 'Preston', 'Ripon', 'Salford', 'Salisbury', 'Sheffield', 'Southampton', 'St Albans', 'Stoke-on-Trent', 'Sunderland', 'Truro', 'Wakefield', 'Wells', 'Westminster', 'Winchester', 'Wolverhampton', 'Worcester', 'York'];
        vm.citiesInWales = ['Bangor', 'Cardiff', 'Newport', 'St Davids', 'Swansea'];
        vm.citiesInScotland = ['Aberdeen', 'Dundee', 'Edinburgh', 'Glasgow', 'Inverness', 'Stirling'];
        vm.citiesInNorthernIreland = ['Armagh', 'Belfast', 'Londonderry', 'Lisburn', 'Newry'];
        
        vm.dateOfBirthPopup = {
            opened: false,
            dateFormat: 'dd/MM/yyyy',
            altInputFormats: ['M!/d!/yyyy'],
            dateOptions: {
                //dateDisabled: function (data) { // Disable weekend selection
                //    var date = data.date,
                //        mode = data.mode;
                //    return mode === 'day' && (date.getDay() === 0 || date.getDay() === 6);
                //},
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

        vm.init();

        function init() {

            vm.isProfilePage = $state.$current.name === 'app.account.update';
            vm.isUsersModule = $state.$current.name === 'app.users.update';
            vm.isEmployeesModule = $state.$current.name === 'app.employees.update';
            vm.canManage = vm.isProfilePage ? true :
                vm.isEmployeesModule ? $rootScope.User.hasAnyRole('RecManager, HRManager') :
                vm.isUsersModule ? $rootScope.User.hasAnyRole('SuperAdmin, Admin') :
                false;

            vm.id = vm.isProfilePage ? $rootScope.User.Id : $stateParams.id;

            service = vm.isEmployeesModule ? employeesService : usersService;

            vm.getRoles();

            if (!vm.isProfilePage && vm.canManage)
                vm.getEmployeeGroups();
            
            vm.get();
        }

        //function onReady() { angular.element(document).ready(function () { }); }

        function get() {

            service.get(vm.id).then(function (data) {
                vm.model = data;
                vm.formData = angular.copy(vm.model);
                vm.formData.dateOfBirth = new Date(vm.formData.dateOfBirth);
            }, function (error) { });
        }
        
        function getRoles() {
            usersService.getRoles().then(function (data) {
                vm.roles = data;
            }, function (error) { });
        }

        function getEmployeeGroups() {
            employeeGroupsService.list().then(function (data) {
                vm.employeeGroups = data.items;
            }, function (error) { });
        }

        function update(form) {
            
            if (!validate(form)) return;

            if (vm.formData.password) {
                if (!vm.formData.confirmPassword || vm.formData.password !== vm.formData.confirmPassword) {
                    $rootScope.notifications.error('Confirm password field should match password field.');
                    return;
                }
            }

            service.update(vm.id, vm.formData).then(function (data) {
                if (vm.isProfilePage) {
                    $state.go('app.account.profile');
                } else if (vm.isEmployeesModule) {
                    $state.go('app.employees.view', { id: vm.id });
                } else {
                    $state.go('app.users.view', { id: vm.id });
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

        function getBackUrl() {
            return vm.isEmployeesModule && !vm.isProfilePage ? $state.href('app.employees.view', { id: vm.id })
                : vm.isProfilePage && !vm.isEmployeesModule ? $state.href('app.account.profile')
                : $state.href('app.users.view', { id: vm.id });
        }
    }
})();
