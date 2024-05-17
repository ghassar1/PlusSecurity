(function () {
    'use strict';

    angular.module('app.shifts')
        .controller('shiftsCreateController', createController);

    createController.$inject = ['$scope', '$rootScope', '$state', '$stateParams', '$localStorage', '_', 'shiftsService', 'employeesService', 'employeeGroupsService'];

    function createController($scope, $rootScope, $state, $stateParams, $localStorage, _, service, employeesService, employeeGroupsService) {

        var vm = $scope;

        vm.enabledDays = [0, 1, 2, 3, 4, 5, 6];
        vm.getEnabledDays = getEnabledDays;

        vm.employeesQuery = { searchCriteria: '', pageNo: 0, pageSize: 0, sortColumn: 'FirstName', sortType: 1 };
        vm.employeeGroupsQuery = { searchCriteria: '', pageNo: 0, pageSize: 0, sortColumn: 'FirstName', sortType: 1 };

        vm.employees = [];
        vm.employeeGroups = [];

        vm.selectedEmployeesAndGroups = [];
        vm.timePicker = { startTime: new Date(), endTime: new Date() };
        vm.formData = { type: 'open', employees: [], groups: [], shiftsPerDay: 1, startTime: '', endTime: '', title: '', breakMins: 30, startDate: new Date(), endDate: new Date(), days: [], notes: '' };
        
        vm.init = init;
        vm.validate = validate;
        vm.create = create;
        vm.getEmployeesAndGroups = getEmployeesAndGroups;
        vm.buildEmployeesAndGroups = buildEmployeesAndGroups;
        vm.loadTags = loadTags;

        vm.timePickerOptions = {
            //hstep: [1, 2, 3],
            //mstep: [1, 5, 10, 15, 25, 30],
            hstep: 1,
            mstep: 15,
            ismeridian: true
        };

        vm.startTimeChanged = startTimeChanged;
        vm.endTimeChanged = endTimeChanged;

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
            }
        };

        var startOfWeek = moment().startOf('isoweek').toDate();
        var endOfWeek = moment().endOf('isoweek').toDate();
        vm.datePicker = {
            date: { startDate: moment(), endDate: moment() },
            options: {

                //singleDatePicker: true,
                showDropdowns: true,
                //autoUpdateInput: true,

                //timePicker: true,
                //timePickerIncrement: 15,

                showWeekNumbers: true,

                alwaysShowCalendars: false,
                linkedCalendars: false,

                locale: {
                    //format: 'DD/MM/YYYY h:mm A',
                    //format: 'DD/MM/YYYY',
                    format: 'dddd, DD MMMM YYYY',
                    cancelLabel: 'Clear'
                },

                //startDate: '23/02/2014',
                //endDate: '25/02/2015',

                //dateLimit: { days: 7 },

                ranges: {
                    'Today': [moment(), moment()],
                    'Tomorrow': [moment().add(1, 'days'), moment().add(1, 'days')],
                    //'Next 2 Days': [moment().add(1, 'days'), moment().add(2, 'days')],
                    //'Next 3 Days': [moment().add(1, 'days'), moment().add(3, 'days')],
                    //'Next 4 Days': [moment().add(1, 'days'), moment().add(4, 'days')],
                    //'Next 5 Days': [moment().add(1, 'days'), moment().add(5, 'days')],
                    //'Next 6 Days': [moment().add(1, 'days'), moment().add(6, 'days')],
                    'Next 7 Days': [moment().add(1, 'days'), moment().add(7, 'days')],
                    'This Week': [moment(), moment().endOf('isoweek')],
                    'Next Week': [moment().startOf('isoweek').add(1, 'week'), moment().endOf('isoweek').add(1, 'week')],
                    'Week After Next': [moment().startOf('isoweek').add(2, 'week'), moment().endOf('isoweek').add(2, 'week')],
                    'Next 30 Days': [moment().add(1, 'days'), moment().add(30, 'days')],
                    'This Month': [moment(), moment().endOf('month')],
                    'Next Month': [moment().add(1, 'month').startOf('month'), moment().add(1, 'month').endOf('month')],
                    'Month After Next': [moment().add(2, 'month').startOf('month'), moment().add(2, 'month').endOf('month')]
                },

                eventHandlers: {
                    'show.daterangepicker': function (ev, picker) {
                        console.log('show.daterangepicker');
                    },
                    'hide.daterangepicker': function (ev, picker) {
                        console.log('hide.daterangepicker');
                    },
                    'showCalendar.daterangepicker': function (ev, picker) {
                        console.log('showCalendar.daterangepicker');
                    },
                    'hideCalendar.daterangepicker': function (ev, picker) {
                        console.log('hideCalendar.daterangepicker');
                    },
                    'apply.daterangepicker': function (ev, picker) {

                        console.log('apply.daterangepicker', 'Start Date: ', vm.datePicker.date.startDate, 'End Date: ', vm.datePicker.date.endDate);

                        var rangeDays = vm.datePicker.date.endDate.diff(vm.datePicker.date.startDate, 'days');
                        rangeDays = rangeDays + 1; // include start date
                        console.log('Range Days: ', rangeDays);

                        vm.dateRangeHasMultipleDays = rangeDays > 1;

                        var enabledDays = [0, 1, 2, 3, 4, 5, 6];
                        if (rangeDays < 7) {
                            enabledDays = [];
                            var dates = enumerateDaysBetweenDates(vm.datePicker.date.startDate, vm.datePicker.date.endDate);
                            angular.forEach(dates, function (date) {
                                console.log('Date: ', date);
                                enabledDays.push(date.weekday());
                            });
                            console.log('Enabled Days: ', enabledDays);
                        }
                        vm.enabledDays = enabledDays;
                    },
                    'cancel.daterangepicker': function (ev, picker) {
                        console.log('cancel.daterangepicker');
                        vm.datePicker.date = { startDate: null, endDate: null };
                    }
                }
            }
        };

        function getEnabledDays() {
            return vm.enabledDays;
        }

        function enumerateDaysBetweenDates(startDate, endDate) {

            var dates = [];

            var currDate = startDate.clone().startOf('day');
            var lastDate = endDate.clone().startOf('day');

            console.log('Current Date: ', currDate, 'Last Date: ', lastDate);

            while (lastDate.diff(currDate, 'days') >= 0) {
                dates.push(currDate.clone());
                currDate.add('days', 1);
            }
            console.log('Dates: ', dates);
            return dates;
        }

        // http://stackoverflow.com/a/16599233/3423802
        // http://plnkr.co/edit/U4VD61?p=preview
        //vm.daysOfWeek = { Monday: true, Tuesday: true, Wednesday: true, Thursday: true, Friday: true, Saturday: false, Sunday: false };
        vm.daysOfWeek = [
            { no: 1, name: 'Monday', checked: true, enabled: true },
            { no: 2, name: 'Tuesday', checked: true, enabled: true },
            { no: 3, name: 'Wednesday', checked: true, enabled: true },
            { no: 4, name: 'Thursday', checked: true, enabled: true },
            { no: 5, name: 'Friday', checked: true, enabled: true },
            { no: 6, name: 'Saturday', checked: false, enabled: true },
            { no: 0, name: 'Sunday', checked: false, enabled: true }
        ];

        vm.selectedDaysOfWeek = [];

        function dateChagned(event) {
            console.log(event);
        }

        // Disable weekend selection
        function disabled(data) {
            var date = data.date,
              mode = data.mode;
            return mode === 'day' && (date.getDay() === 0 || date.getDay() === 6);
        }

        vm.dateOfBirthPopup.open = function () {
            vm.dateOfBirthPopup.opened = true;
            console.log('opened');
        };

        vm.init();

        function init() {

            console.debug('$stateParams: ', $stateParams);

            if ($stateParams.date) {
                vm.datePicker.date.startDate = $stateParams.date;
                vm.datePicker.date.endDate = $stateParams.date;
            }

            console.debug(vm.datePicker.date.startDate, vm.datePicker.date.endDate);

            if ($stateParams.employeeId) {
                if ($stateParams.employeeId === 'open') {
                    vm.formData.type = 'open';
                } else {
                    vm.formData.type = 'allocated';
                }
            }

            vm.timePicker.startTime.setHours(9, 0, 0, 0);
            vm.timePicker.endTime.setHours(17, 0, 0, 0);

            vm.formData.startTime = moment(vm.timePicker.startTime).format('HH:mm:ss');
            vm.formData.endTime = moment(vm.timePicker.endTime).format('HH:mm:ss');

            vm.dateRangeHasMultipleDays = false;
            updateTitle();

            $('.default-select2').select2();            
            vm.getEmployeesAndGroups();
        }

        function startTimeChanged() {
            vm.formData.startTime = moment(vm.timePicker.startTime).format('HH:mm:ss');
            console.log('From: ', vm.formData.startTime);
            updateTitle();
        }

        function endTimeChanged() {
            vm.formData.endTime = moment(vm.timePicker.endTime).format('HH:mm:ss');
            console.log('To: ', vm.formData.endTime);
            updateTitle();
        }

        function updateTitle() {
            vm.formData.title = moment(vm.timePicker.startTime).format('hh:mm A') + ' - ' + moment(vm.timePicker.endTime).format('hh:mm A');
        }

        function getEmployeesAndGroups() {
            employeeGroupsService.list().then(function (employeeGroups) {
                vm.employeeGroups = employeeGroups.items;
                console.log('Groups: ', vm.employeeGroups);

                employeesService.list(vm.employeesQuery).then(function (employees) {
                    vm.employees = employees.items;
                    console.log('Employees: ', vm.employees);

                    vm.buildEmployeesAndGroups();
                }, function (error) { });
            }, function (error) { });
        }

        function buildEmployeesAndGroups() {

            vm.employeesAndGroups = [];

            angular.forEach(vm.employeeGroups, function (employeeGroup) {
                employeeGroup.text = 'Group: ' + employeeGroup.name + ' [' + employeeGroup.noOfEmployees + ']';
                employeeGroup.isGroup = true;
                vm.employeesAndGroups.push(employeeGroup);

                if (employeeGroup.id === $stateParams.employeeId) {
                    vm.selectedEmployeesAndGroups.push(employeeGroup);
                }
            });

            angular.forEach(vm.employees, function (employee) {
                employee.text = employee.firstName + ' ' + employee.lastName + ' <' + employee.email + '>';
                employee.isGroup = false;
                vm.employeesAndGroups.push(employee);

                if (employee.id === $stateParams.employeeId) {
                    vm.selectedEmployeesAndGroups.push(employee);
                }
            });
        }

        function loadTags(query) {
            vm.selected = [];
            angular.forEach(vm.employeesAndGroups, function (o) {
                var text = '';
                if (o.isGroup) {
                    text = 'Group: ' + o.name + ' [' + o.noOfEmployees + ']';
                    if (text.toUpperCase().indexOf(query.toUpperCase()) !== -1) {
                        vm.selected.push(o);
                    }
                } else {
                    text = o.firstName + ' ' + o.lastName + ' <' + o.email + '>';
                    if (text.toUpperCase().indexOf(query.toUpperCase()) !== -1) {
                        vm.selected.push(o);
                    }
                }
            });
            return vm.selected;
        }

        function create(form) {

            //$('#dateOfBirthField').trigger('input');

            console.log(form);
            if (!validate(form)) return;

            if (vm.formData.password) {
                if (!vm.formData.confirmPassword || vm.formData.password !== vm.formData.confirmPassword) {
                    $rootScope.notifications.error('Confirm password field should match password field.');
                    return;
                }
            }

            prepareFormData();
            console.debug(vm.formData);
            service.create(vm.formData).then(function (data) {
                $state.go('app.shifts.list');
            }, function (error) { });
        }

        function prepareFormData() {
            
            angular.forEach(vm.selectedEmployeesAndGroups, function (o) {
                if (o.isGroup) {
                    vm.formData.groups.push({ id: o.id });
                } else {
                    vm.formData.employees.push({ id: o.id });
                }
            });

            //vm.formData.startDate = vm.datePicker.date.startDate;
            //vm.formData.endDate = vm.datePicker.date.endDate;

            vm.formData.startDate = vm.datePicker.date.startDate.format('YYYY-MM-DD');
            vm.formData.endDate = vm.datePicker.date.endDate.format('YYYY-MM-DD');
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
