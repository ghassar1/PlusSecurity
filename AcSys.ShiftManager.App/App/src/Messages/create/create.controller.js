(function () {
    'use strict';

    angular.module('app.messages')
        .controller('messagesCreateController', createController);

    createController.$inject = ['$scope', '$rootScope', '$state', '$stateParams', '$localStorage', 'messagesService', '$q', '$http', '_'];

    function createController($scope, $rootScope, $state, $stateParams, $localStorage, service, $q, $http, _) {

        //try {
        //    var m = angular.module('ngTagsInput');
        //    console.log(m.name + ' loaded!');
        //} catch (err) { console.log('failed to require'); }

        var vm = $scope;
        vm.formData = { recipients: [], subject: '', text: '' };
        vm.recipients = [];
        vm.users = [];

        vm.init = init;
        vm.create = create;
        vm.validate = validate;
        vm.loadTags = loadTags;
        
        vm.init();
        
        function init() {

            vm.inReplyToMessage = $stateParams.inReplyToMessage;
            //console.log('In Reply To: ', vm.inReplyToMessage);
            if (vm.inReplyToMessage) {

                var to = [];
                angular.forEach(vm.inReplyToMessage.recipients, function (recipient) {
                    to.push(recipient.firstName + ' ' + recipient.lastName + ' <' + recipient.email + '>');
                });

                if (vm.inReplyToMessage.subject.startsWith('re: ') || 
                    vm.inReplyToMessage.subject.startsWith('Re: ') || 
                    vm.inReplyToMessage.subject.startsWith('RE: ')) {
                    vm.formData.subject = '' + vm.inReplyToMessage.subject;
                } else {
                    vm.formData.subject = 'Re: ' + vm.inReplyToMessage.subject;
                }

                var text = [
                    '', '', '', '', '',
                    'From: ' + vm.inReplyToMessage.sender.firstName + ' ' + vm.inReplyToMessage.sender.lastName + ' <' + vm.inReplyToMessage.sender.email + '>',
                    'To: ' + to.join(', '),
                    'Subject: ' + vm.inReplyToMessage.subject,
                    //'Date: ' + vm.inReplyToMessage.sentAt,
                    '',
                    vm.inReplyToMessage.text
                ].join('\n');

                vm.formData.text = text;
                vm.inReplyToRecipients = _.pluck(vm.inReplyToMessage.recipients, 'email');
                //console.log('InReplyToRecipients: ', vm.inReplyToRecipients);
            }

            service.getRecipients().then(function (users) {
                angular.forEach(users, function (user) {
                    user.text = user.firstName + ' ' + user.lastName + ' <' + user.email + '>';
                    vm.users.push(user);

                    if (_.contains(vm.inReplyToRecipients, user.email)) {
                        vm.formData.recipients.push(angular.copy(user));
                    }
                    
                });
            }, function (error) { });
        }

        function create(form) {
            
            if (!validate(form)) return;

            service.create(vm.formData).then(function (data) {
                $state.go('app.messages.inbox.list');
            }, function (error) { });

        }

        function validate(form) {

            $rootScope.notifications.clear();
            $rootScope.FormValidator.clear(form);

            var isValid = $rootScope.FormValidator.validate(form);
            if (!isValid)
                $rootScope.notifications.error('Please fill in all the required fields.');
            return isValid;
        }

        function loadTags(query) {
            //return $http.get('tags.json');
            //console.log(query);

            vm.selectedUsers = [];

            angular.forEach(vm.users, function (user) {

                var text = user.firstName + ' ' + user.lastName + ' <' + user.email + '>';
                if (text.indexOf(query) !== -1) {
                    vm.selectedUsers.push(user);
                }
            });

            return vm.selectedUsers;
        }
    }
})();
