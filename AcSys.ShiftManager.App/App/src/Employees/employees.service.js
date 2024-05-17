(function () {
    'use strict';

    angular.module('app.employees')
        .factory('employeesService', serviceFactory);

    serviceFactory.$inject = ['$http', '$q', '$log', 'AppHttpService'];

    function serviceFactory($http, $q, $log, AppHttpService) {

        var ep = '/Employees';
        
        return {
            create: create,
            list: list,
            listAll: listAll,
            update: update,
            get: get,
            del: del,
            activate: activate,
            deactivate: deactivate
        };

        function create(data) {
            return AppHttpService.Post(ep, data);
        }

        function list(query) {
            return AppHttpService.Get(ep, query);
        }

        function listAll(query) {
            return AppHttpService.Get(ep + '/All', query);
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

        function activate(id) {
            var url = ep + '/' + id + '/activate';
            return AppHttpService.Put(url);
        }

        function deactivate(id) {
            var url = ep + '/' + id + '/deactivate';
            return AppHttpService.Put(url);
        }
    }
})();
