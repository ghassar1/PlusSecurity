(function () {
    'use strict';

    angular.module('app.messages')
        .controller('messagesViewController', viewController);

    viewController.$inject = ['$scope', '$rootScope', '$state', '$stateParams', '$localStorage', 'messagesService', '$sce'];

    function viewController($scope, $rootScope, $state, $stateParams, $localStorage, service, $sce) {

        var vm = $scope;
        vm.model = {};
        vm.recipients = [];
        vm.init = init;
        vm.get = get;
        vm.del = del;
        vm.markAsRead = markAsRead;

        vm.init();

        function init() {
            vm.id = $stateParams.id;
            vm.get(vm.id);
        }

        function get(id) {

            service.get(id).then(function (data) {

                vm.model = data;

                //vm.isSentMessage = vm.model.sender.email === $rootScope.userData.email;
                //vm.isIncomingMessage = !vm.isSentMessage;
                vm.isSentMessage = $stateParams.isSentMessage;
                vm.isIncomingMessage = $stateParams.isIncomingMessage;

                if (vm.isIncomingMessage) {
                    $rootScope.currentPage.title = 'Incoming Message';
                    vm.markAsRead();
                } else {
                    $rootScope.currentPage.title = 'Sent Message';
                }
                
                //$('#jquery-tagIt-recipients').tagit({
                //    fieldName: 'recipients',
                //    showAutocompleteOnFocus: true,
                //    autocomplete: { delay: 0, minLength: 0 },
                //    removeConfirmation: false,
                //    caseSensitive: true,
                //    allowDuplicates: false,
                //    allowSpaces: true,
                //    readOnly: true,
                //    tagLimit: null,
                //    placeholderText: null
                //    //availableTags: null //vm.recipients
                //});

                angular.forEach(vm.model.recipients, function (recipient) {
                    var recipientTag = recipient.firstName + ' ' + recipient.lastName + ' <' + recipient.email + '>';
                    vm.recipients.push(recipientTag);

                    //$('#jquery-tagIt-recipients').tagit('createTag', recipientTag);
                });

                vm.recipientsList = vm.recipients.join('\n');

                vm.model.textHtml = $sce.trustAsHtml(vm.model.text);

            }, function (error) { });
        }

        function del() {
            service.del(vm.id).then(function (data) {
                if (vm.isIncomingMessage) {
                    $state.go('app.messages.inbox.list');
                } else {
                    $state.go('app.messages.sent.list');
                }
            }, function (error) { });
        }

        function markAsRead() {
            service.markAsRead(vm.id).then(function () {
                $rootScope.refreshHeaderNotifications();
            }, function (error) { });
        }
    }
})();
