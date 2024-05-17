(function () {
    'use strict';

    angular.module('app.activityLogs')
        .factory('activityLogsService', serviceFactory);

    serviceFactory.$inject = ['$http', '$q', '$log', 'AppHttpService'];

    function serviceFactory($http, $q, $log, AppHttpService) {

        var ep = '/ActivityLogs';

        return {
            create: create,
            list: list,
            update: update,
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
