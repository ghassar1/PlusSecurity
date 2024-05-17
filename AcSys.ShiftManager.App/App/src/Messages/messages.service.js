(function () {
    'use strict';

    angular.module('app.messages')
        .factory('messagesService', serviceFactory);

    serviceFactory.$inject = ['$http', '$q', '$log', 'AppHttpService'];

    function serviceFactory($http, $q, $log, AppHttpService) {

        var ep = '/Messages';

        return {
            create: create,
            list: list, //list_fake
            listSent: listSent,
            update: update,
            get: get,
            del: del,
            markAsUnread: markAsUnread,
            markAsRead: markAsRead,
            getRecipients: getRecipients
        };

        function create(data) {
            return AppHttpService.Post(ep, data);
        }

        function list(query) {
            return AppHttpService.Get(ep, query);
        }

        function listSent(query) {
            var url = ep + '/sent';
            return AppHttpService.Get(url, query);
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

        function markAsRead(id) {
            var url = ep + '/' + id + '/read';
            return AppHttpService.Put(url);
        }

        function markAsUnread(id) {
            var url = ep + '/' + id + '/unread';
            return AppHttpService.Put(url);
        }

        function getRecipients() {
            var url = ep + '/Recipients';
            return AppHttpService.Get(url);
        }

        function list_fake() {

            return $q.resolve([
                { id: '234', from: 'Martin', subject: 'Message from Martin', date: '10-11-2016', message: 'Message from Martin.......' },
                { id: '235', from: 'Tom', subject: 'Message from Tom', date: '11-11-2016', message: 'Message from Tom.......' },
                { id: '236', from: 'Peter', subject: 'Message from Peter', date: '12-11-2016', message: 'Message from Peter.......' }
            ]);
        }
    }

})();
