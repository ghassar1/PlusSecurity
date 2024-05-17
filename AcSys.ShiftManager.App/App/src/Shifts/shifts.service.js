(function () {
    'use strict';

    angular.module('app.shifts')
        .factory('shiftsService', serviceFactory);

    serviceFactory.$inject = ['$http', '$q', '$log', 'AppHttpService'];

    function serviceFactory($http, $q, $log, AppHttpService) {

        var ep = '/Shifts';

        return {
            create: create,
            list: list,
            update: update,
            assign: assign,

            take: take,
            leave: leave,

            clockIn: clockIn,
            clockOut: clockOut,

            get: get,
            del: del
        };

        function create(data) {
            return AppHttpService.Post(ep, data);
        }

        function list(query) {
            return AppHttpService.Get(ep, query);
        }

        function update(id, data) {
            var url = ep + '/' + id;
            return AppHttpService.Put(url, data);
        }

        function assign(id, data) {
            var url = ep + '/' + id + '/Assign';
            return AppHttpService.Put(url, data);
        }

        function take(id) {
            var url = ep + '/' + id + '/Take';
            return AppHttpService.Put(url);
        }

        function leave(id) {
            var url = ep + '/' + id + '/Leave';
            return AppHttpService.Put(url);
        }

        function clockIn(id) {
            var url = ep + '/' + id + '/clockin';
            return AppHttpService.Put(url);
        }

        function clockOut(id) {
            var url = ep + '/' + id + '/clockout';
            return AppHttpService.Put(url);
        }

        function get(id) {
            var url = ep + '/' + id;
            return AppHttpService.Get(url);
        }

        function del(id) {
            var url = ep + '/' + id;
            return AppHttpService.Delete(url);
        }
    }
})();
