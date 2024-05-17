(function () {
    'use strict';

    angular.module('app.reports')
        .controller('attendanceReportController', listController);

    listController.$inject = ['$scope', '$rootScope', '$state', '$localStorage', 'reportsService'];

    function listController($scope, $rootScope, $state, $localStorage, service) {

        var vm = $scope;

        vm.init = init;

        vm.list = list;
        vm.model = {};

        vm.query = { startDate: null, endDate: null };

        //vm.pageSizes = [10, 25, 50, 100, 200];
        //vm.pageChanged = pageChanged;
        //vm.pageSizeChanged = pageSizeChanged;

        vm.dtOptions = {
            paginationType: 'full_numbers',
            //displayLength: 2,
            paging: false,
            ordering: true,
            info: false,
            searching: false
        };

        vm.datePicker = {
            //date: { startDate: null, endDate: null },
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
                    format: 'DD/MM/YYYY',
                    cancelLabel: 'Clear'
                },

                //startDate: '23/02/2014',
                //endDate: '25/02/2015',
                
                //dateLimit: { days: 7 },

                ranges: {
                    'Today': [moment(), moment()],
                    'Yesterday': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
                    'Last 7 Days': [moment().subtract(6, 'days'), moment()],
                    'Last 30 Days': [moment().subtract(29, 'days'), moment()],
                    'This Month': [moment().startOf('month'), moment().endOf('month')],
                    'Last Month': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]
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
                        console.log('apply.daterangepicker');
                    },
                    'cancel.daterangepicker': function (ev, picker) {
                        console.log('cancel.daterangepicker');
                        vm.datePicker = { startDate: null, endDate: null };
                    }
                }
            }
        };

        vm.init();

        function init() {
            //vm.listUsers();
            vm.list();
        }

        function list() {
            if (vm.datePicker.date) {
                if (vm.datePicker.date.startDate) {
                    vm.query.startDate = new Date(vm.datePicker.date.startDate);
                }
                if (vm.datePicker.date.endDate) {
                    vm.query.endDate = new Date(vm.datePicker.date.endDate);
                }
            }
            service.getAttendanceData(vm.query).then(function (data) {
                vm.model = data;
            }, function (error) { });
        }

        function listUsers() {
            usersService.list(vm.usersQuery).then(function (data) {
                vm.users = data.items;
                console.log('Users', vm.users);
            }, function (error) { });
        }

        //function getUsers(criteria) {
        //    console.log('UI-Select Criteria:', criteria);
        //    vm.usersQuery.searchCriteria = criteria;
        //    vm.listUsers();
        //}

        function del(id) {
            service.del(id).then(function (data) { }, function (error) { });
        }

        function pageSizeChanged() {
            vm.query.pageNo = 1;
            list();
        }

        function pageChanged() {
            list();
        }
    }
})();
