(function () {
    'use strict';

    angular.module('app.shifts')
        .controller('shiftsViewController', viewController);

    viewController.$inject = ['$scope', '$rootScope', '$state', '$stateParams', '$localStorage', 'shiftsService', '$interval', '$timeout'];

    function viewController($scope, $rootScope, $state, $stateParams, $localStorage, service, $interval, $timeout) {

        var vm = $scope;
        vm.model = {};

        vm.userIsShiftEmployee = false;

        vm.init = init;
        vm.get = get;
        vm.del = del;

        vm.takeShift = takeShift;
        vm.leaveShift = leaveShift;
        vm.clockIn = clockIn;
        vm.clockOut = clockOut;
                
        vm.init();

        function init() {

            vm.id = $stateParams.id;

            vm.canManage = $rootScope.User.hasAnyRole('HRManager');
            //if (!vm.canManage) throw new AuthError();

            get();
        }

        var stopIntervalPromise = null;
        function get() {

            service.get(vm.id).then(function (data) {

                vm.model = data;
                console.debug(vm.model);

                stopInterval();
                evaluateShiftTimingsAndStatus();

                //if ((vm.minsRemainingInShiftStart - 15) < 1440) {
                //    var t = (vm.minsRemainingInShiftStart - 15) * 60 * 1000;
                //    console.debug('Ms:', t);
                //    stopIntervalPromise = $timeout(evaluateShiftTimingsAndStatus, t);
                //}
                stopIntervalPromise = $interval(evaluateShiftTimingsAndStatus, 10 * 1000);
            }, function (error) { });
        }

        // Make sure that the interval is destroyed too
        vm.$on('$destroy', function () {
            stopInterval();
        });

        function evaluateShiftTimingsAndStatus() {

            //console.debug(moment.tz.names());
            console.debug(new Date().getTime());
            //console.debug('Local & UK Time Diff: ', moment(new Date()).format(), moment(new Date()).tz('Europe/London').format(), moment().tz(new Date().toString(), 'Europe/London').format());
            vm.canManage = $rootScope.User.hasAnyRole('HRManager') && vm.model.status === 0;

            var now = moment(new Date()),
                    startTime = moment(vm.model.startTime),
                    endTime = moment(vm.model.endTime);

            vm.startTimeHasPassed = startTime.isBefore(now);
            vm.startTimeHasNotPassed = !vm.startTimeHasPassed;

            vm.endTimeHasPassed = endTime.isBefore(now);
            vm.endTimeHasNotPassed = !vm.endTimeHasPassed;

            vm.shiftIsOpen = vm.model.employee === undefined || vm.model.employee === null;
            vm.shiftIsTaken = !vm.shiftIsOpen;
            vm.userIsShiftEmployee = vm.shiftIsTaken && vm.model.employee.id === $rootScope.User.Id;

            vm.shiftNotClockedIn = vm.model.clockInTime === null || vm.model.clockInTime === undefined;
            vm.shiftClockedIn = !vm.shiftNotClockedIn;

            vm.shiftNotClockedOut = vm.model.clockOutTime === null || vm.model.clockOutTime === undefined;
            vm.shiftClockedOut = !vm.shiftNotClockedOut;

            vm.userCanTakeShift = vm.shiftIsOpen && vm.userIsShiftEmployee;
            vm.userCanLeaveShift = vm.userIsShiftEmployee && vm.shiftNotClockedIn;

            vm.minsRemainingInShiftStart = startTime.diff(now, 'minute');
            vm.minsRemainingInShiftEnd = endTime.diff(now, 'minute');

            vm.shiftCanBeStarted = vm.endTimeHasNotPassed && vm.minsRemainingInShiftStart < 15 && vm.minsRemainingInShiftEnd > 0;
            vm.shiftCanBeEnded = vm.endTimeHasPassed && vm.minsRemainingInShiftEnd < 0 && vm.minsRemainingInShiftEnd > -15;

            vm.canClockIn = vm.userIsShiftEmployee && vm.shiftNotClockedIn && vm.shiftNotClockedOut && vm.shiftCanBeStarted && vm.endTimeHasNotPassed;
            vm.canClockOut = vm.userIsShiftEmployee && vm.shiftClockedIn && vm.shiftCanBeEnded && vm.shiftNotClockedOut;
            
            vm.shiftInProgress = vm.shiftClockedIn && vm.shiftNotClockedOut;
            vm.showStatus = !vm.shiftInProgress && !vm.canClockOut;
            //if (false) stopInterval();

            console.debug('vm.endTimeHasPassed', vm.endTimeHasPassed, 'vm.minsRemainingInShiftStart', vm.minsRemainingInShiftStart, 'vm.shiftCanBeStarted', vm.shiftCanBeStarted);
            console.debug('vm.minsRemainingInShiftEnd', vm.minsRemainingInShiftEnd, 'vm.shiftCanBeEnded', vm.shiftCanBeEnded);
            console.debug('vm.shiftIsOpen', vm.shiftIsOpen, 'vm.userIsShiftEmployee', vm.userIsShiftEmployee, 'vm.userCanTakeShift', vm.userCanTakeShift, 'vm.shiftIsTaken', vm.shiftIsTaken);
            console.debug('vm.canClockIn', vm.canClockIn, 'vm.canClockOut', vm.canClockOut);
            console.debug('vm.userCanLeaveShift', vm.userCanLeaveShift, 'vm.canManage', vm.canManage, 'vm.canClockIn', vm.canClockIn, 'vm.canClockOut', vm.canClockOut);
            console.debug('=============================================================================================================================================');
        }

        function stopInterval() {
            if (angular.isDefined(stopIntervalPromise)) {
                $interval.cancel(stopIntervalPromise);
                stopIntervalPromise = null;
            }
        }

        function del() {
            service.del(vm.id).then(function (data) {
                $state.go('app.shifts.list');
            }, function (error) { });
        }

        function takeShift() {
            service.take(vm.id).then(function (data) {
                get();
            }, function (error) { });
        }

        function leaveShift() {
            service.leave(vm.id).then(function (data) {
                get();
            }, function (error) { });
        }

        function clockIn() {
            service.clockIn(vm.id).then(function (data) {
                get();
            }, function (error) { });
        }

        function clockOut() {
            service.clockOut(vm.id).then(function (data) {
                get();
            }, function (error) { });
        }
    }
})();
