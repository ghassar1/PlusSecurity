(function () {
    'use strict';

    angular.module('app.core')
        .directive('weekDaysSelection', directive);

    directive.$inject = ['_'];
    function directive(_) {

        // http://stackoverflow.com/a/16599233/3423802
        // http://plnkr.co/edit/U4VD61?p=preview
        //vm.daysOfWeek = { Monday: true, Tuesday: true, Wednesday: true, Thursday: true, Friday: true, Saturday: false, Sunday: false };
        var daysOfWeek = [
            { no: 1, name: 'Monday', checked: true, enabled: true },
            { no: 2, name: 'Tuesday', checked: true, enabled: true },
            { no: 3, name: 'Wednesday', checked: true, enabled: true },
            { no: 4, name: 'Thursday', checked: true, enabled: true },
            { no: 5, name: 'Friday', checked: true, enabled: true },
            { no: 6, name: 'Saturday', checked: false, enabled: true },
            { no: 0, name: 'Sunday', checked: false, enabled: true }
        ];

        var vm;
        function link($scope, element, attrs, ngModel) {

            vm = $scope;

            vm.daysOfWeek = daysOfWeek;

            vm.checkAllDays = checkAllDays;
            vm.uncheckAllDays = uncheckAllDays;
            vm.checkWeekdays = checkWeekdays;
            vm.checkWeekends = checkWeekends;
            vm.invertCheckedDays = invertCheckedDays;

            vm.updateModel = updateModel;
            vm.setEnabledDays = setEnabledDays;

            if (vm.enabledDays) {
                //set the value of ngModel when the local date property changes
                vm.$watch('enabledDays', function () {
                    console.debug('Enabled Days: ', vm.enabledDays);
                    vm.setEnabledDays();
                });
            }
        }

        return {
            require: 'ngModel',
            //restrict: 'AEC',
            link: link,
            scope: {
                selectedDays: '=ngModel',
                enabledDays: '=enabledDays'
                //getEnabledDays: '&getEnabledDaysFn'
            },
            template: [
                //'<div class="form-group">',
                '   <label class="control-label"><strong>Days of Week <span class="text-danger">*</span></strong></label>',
                '   <div class="controls">',
                '       <div class="col-sm-6">',
                '           <div ng-repeat="day in daysOfWeek">',
                '               <label><input type="checkbox" ng-disabled="!day[\'enabled\']" ng-model="day[\'checked\']" ng-change="updateModel()"> {{day.name}}</label>',
                '           </div>',
                '       </div>',
                '       <div class="col-sm-6">',
                '           <button type="button" class="btn btn-success btn-block" ng-click="checkWeekdays()">Weekdays</button>',
                '           <button type="button" class="btn btn-success btn-block" ng-click="checkWeekends()">Weekends</button>',
                '           <button type="button" class="btn btn-success btn-block" ng-click="checkAllDays()">Check All</button>',
                '           <button type="button" class="btn btn-success btn-block" ng-click="uncheckAllDays()">Uncheck All</button>',
                '           <button type="button" class="btn btn-success btn-block" ng-click="invertCheckedDays()">Invert Checks</button>',
                '       </div>',
                '   </div>'
                //'</div>'
            ].join('\n')
        };

        function checkAllDays() {
            angular.forEach(vm.daysOfWeek, function (day) {
                if (day.enabled)
                    day.checked = true;
            });
            vm.updateModel();
        }

        function uncheckAllDays() {
            angular.forEach(vm.daysOfWeek, function (day) {
                if (day.enabled)
                    day.checked = false;
            });
            vm.updateModel();
        }

        function checkWeekdays() {
            angular.forEach(vm.daysOfWeek, function (day) {
                if (day.enabled)
                    day.checked = day.no > 0 && day.no < 6;
            });
            vm.updateModel();
        }

        function checkWeekends() {
            angular.forEach(vm.daysOfWeek, function (day) {
                if (day.enabled)
                    day.checked = day.no === 0 || day.no === 6;
            });
            vm.updateModel();
        }

        function invertCheckedDays() {
            angular.forEach(vm.daysOfWeek, function (day) {
                if (day.enabled)
                    day.checked = !day.checked;
            });
            vm.updateModel();
        }

        function updateModel() {

            //var enabledDays = vm.getEnabledDays();
            //if (enabledDays) {
            //    vm.enabledDays = enabledDays;
            //    vm.setEnabledDays();
            //}

            vm.selectedDaysOfWeek = [];
            angular.forEach(vm.daysOfWeek, function (day) {
                if (day.enabled && day.checked)
                    vm.selectedDaysOfWeek.push(day.no);
            });

            vm.selectedDays = vm.selectedDaysOfWeek;
            return vm.selectedDaysOfWeek;
        }

        function setEnabledDays() {

            if (!vm.enabledDays) return;

            angular.forEach(vm.daysOfWeek, function (day) {
                day.enabled = _.contains(vm.enabledDays, day.no);
                day.checked = day.enabled;
            });
        }
    }
})();
