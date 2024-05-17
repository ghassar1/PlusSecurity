(function () {
    'use strict';

    angular.module('app.notifications')
        .factory('notificationsService', serviceFactory);

    serviceFactory.$inject = ['$http', '$q', '$log', 'AppHttpService'];

    function serviceFactory($http, $q, $log, AppHttpService) {

        var ep = '/Notifications';

        return {
            create: create,
            list: list, //list_fake
            listMine: listMine,
            listMineNew: listMineNew,
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

        function listMine(query) {
            //ep = '/Notifications1';
            var url = ep + '/Mine';
            return AppHttpService.Get(url, query);
        }

        function listMineNew() {
            var url = ep + '/Mine/New';
            return AppHttpService.Get(url);
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
                { id: '234', from: 'Martin', title: 'Notification from Martin', date: '10-11-2016', body: 'Notification from Martin.......' },
                { id: '235', from: 'Tom', title: 'Notification from Tom', date: '11-11-2016', body: 'Notification from Tom.......' },
                { id: '236', from: 'Peter', title: 'Notification from Peter', date: '12-11-2016', body: 'Notification from Peter.......' }
            ]);
        }
    }
})();
