(function () {
    'use strict';

    angular.module('app.shifts')
        .controller('shiftsUpdateController', udpateController);

    udpateController.$inject = ['$scope', '$rootScope', '$state', '$stateParams', '$localStorage', 'shiftsService', 'employeesService'];

    function udpateController($scope, $rootScope, $state, $stateParams, $localStorage, service, employeesService) {

        var vm = $scope;

        vm.employeesQuery = { searchCriteria: '', pageNo: 0, pageSize: 0, sortColumn: 'FirstName', sortType: 1 };
        
        vm.employees = [];
        vm.selectedEmployees = [];

        vm.timePicker = { startTime: new Date(), endTime: new Date() };
        vm.formData = { type: 'open', employees: [], groups: [], shiftsPerDay: 1, startTime: '', endTime: '', title: '', breakMins: 30, startDate: moment().format('YYYY-MM-DD'), endDate: moment().format('YYYY-MM-DD'), days: [], notes: '' };
        //vm.date = new Date();

        vm.init = init;
        vm.get = get;
        vm.validate = validate;
        vm.update = update;

        vm.getEmployees = getEmployees;
        vm.loadTags = loadTags;

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
                        setEnabledDays(enabledDays);
                    },
                    'cancel.daterangepicker': function (ev, picker) {
                        console.log('cancel.daterangepicker');
                        vm.datePicker.date = { startDate: null, endDate: null };
                    }
                }
            }
        };

        function setEnabledDays(enabledDays) {
            angular.forEach(vm.daysOfWeek, function (day) {
                day.enabled = _.contains(enabledDays, day.no);
                day.checked = day.enabled;
            });
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

            vm.id = $stateParams.id;

            vm.dateRangeHasMultipleDays = false;

            vm.canManage = $rootScope.User.hasAnyRole('RecManager, HRManager') ? true : false;

            console.debug($rootScope.User.hasAnyRole('RecManager, HRManager'));

            //if (!vm.canManage) throw new AuthError();

            //vm.getEmployees();
            vm.get();
        }

        function getEmployees() {
            employeesService.list(vm.employeesQuery).then(function (employees) {
                vm.employees = employees.items;

                angular.forEach(vm.employees, function (employee) {
                    employee.text = employee.firstName + ' ' + employee.lastName + ' <' + employee.email + '>';
                });
            }, function (error) { });
        }

        function get() {

            employeesService.list(vm.employeesQuery).then(function (employees) {
                vm.employees = employees.items;

                angular.forEach(vm.employees, function (employee) {
                    employee.text = employee.firstName + ' ' + employee.lastName + ' <' + employee.email + '>';
                });

                service.get(vm.id).then(function (data) {

                    console.debug(data);

                    vm.model = data;
                    vm.formData = angular.copy(vm.model);

                    //vm.formData.date = new Date(vm.formData.startTime); //moment(vm.formData.startTime);
                    vm.formData.date = new Date(moment(vm.formData.startTime).format('YYYY-MM-DD'));
                    vm.date = new Date(moment(vm.formData.startTime).format('YYYY-MM-DD'));

                    var date = moment(new Date()),
                        startTime = moment(vm.formData.startTime),
                        endTime = moment(vm.formData.endTime);

                    endTime.date(startTime.date());
                    endTime.month(startTime.month());
                    endTime.year(startTime.year());

                    vm.formData.startTime = startTime;
                    vm.timePicker.startTime = startTime;
                    
                    vm.formData.endTime = endTime;
                    vm.timePicker.endTime = endTime;

                    vm.endTimeIsLessThanStartTime = moment(vm.timePicker.endTime).isBefore(moment(vm.timePicker.startTime));

                    //vm.formData.type = vm.formData.employee !== undefined && vm.formData.employee !== null ? 'allocated' : 'open';
                    if (vm.formData.employee !== undefined && vm.formData.employee !== null) {
                        vm.formData.type = 'allocated';
                        setEmployee();
                    } else {
                        vm.formData.type = 'open';
                    }
                }, function (error) { });

            }, function (error) { });
        }

        function setEmployee() {
            vm.selectedEmployees = [];
            angular.forEach(vm.employees, function (employee) {
                if (employee.id === vm.formData.employee.id) {
                    vm.selectedEmployees.push(employee);
                }
            });
        }

        function startTimeChanged() {
            vm.formData.startTime = moment(vm.timePicker.startTime).format('HH:mm:ss');
            console.log('From: ', vm.formData.startTime, vm.timePicker.startTime);
            updateTitle();

            vm.endTimeIsLessThanStartTime = moment(vm.timePicker.endTime).isBefore(moment(vm.timePicker.startTime));
        }

        function endTimeChanged() {
            vm.formData.endTime = moment(vm.timePicker.endTime).format('HH:mm:ss');
            console.log('To: ', vm.formData.endTime, vm.timePicker.endTime);
            updateTitle();

            vm.endTimeIsLessThanStartTime = moment(vm.timePicker.endTime).isBefore(moment(vm.timePicker.startTime));
        }

        function updateTitle() {
            vm.formData.title = moment(vm.timePicker.startTime).format('hh:mm A') + ' - ' + moment(vm.timePicker.endTime).format('hh:mm A');
        }

        function loadTags(query) {
            vm.selected = [];
            angular.forEach(vm.employees, function (o) {
                var text = o.firstName + ' ' + o.lastName + ' <' + o.email + '>';
                if (text.toUpperCase().indexOf(query.toUpperCase()) !== -1) {
                    vm.selected.push(o);
                }
            });
            return vm.selected;
        }

        function update(form) {

            //$('#dateOfBirthField').trigger('input');

            if (!validate(form)) return;

            prepareFormData();

            console.debug(vm.selectedEmployees);
            console.debug(vm.formData);
            //return;

            service.update(vm.id, vm.formData).then(function (data) {
                $state.go('app.shifts.view', { id: vm.id });
            }, function (error) { });
        }

        function prepareFormData() {
            
            if (vm.selectedEmployees && vm.selectedEmployees !== null && vm.selectedEmployees.length > 0) {
                vm.formData.employeeId = vm.selectedEmployees[0].id;
            }

            var date = moment(vm.date);
            vm.formData.date = date.format('YYYY-MM-DD');

            vm.formData.startTime = moment(vm.timePicker.startTime).format('HH:mm:ss');
            vm.formData.endTime = moment(vm.timePicker.endTime).format('HH:mm:ss');

            console.log(vm.formData.date, vm.datePicker.date);
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
