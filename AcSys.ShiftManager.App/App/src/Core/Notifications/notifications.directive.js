(function () {
    'use strict';

    angular.module('app.core')
        .directive('notifications', notificationsFactory);

    function notificationsFactory() {

        function link($scope, element, attrs) {
            //console.log('Element: ', element, attrs);

            var vm = $scope;
            vm.closeMessages = closeMessages;
            vm.closeErrors = closeErrors;

            function closeMessages() {
                vm.messages = [];
            }

            function closeErrors() {
                vm.errors = [];
            }
        }

        return {
            //restrict: 'AEC',
            link: link,
            scope: {
                messagesTitle: '=messagesTitle',
                messages: '=messages',

                errorsTitle: '=errorsTitle',
                errors: '=errors'
            },
            //templateUrl: 'App/dist/Views/Core/Notifications/notifications.html'
            template: [
                '<!-- begin errors -->', 
                '<div ng-show="errors && errors.length > 0" class="alert alert-block alert-danger">', 
                '    <!--<button type="button" class="close" data-dismiss="alert"><span aria-hidden="true">&times;</span></button>-->', 
                '    <button type="button" class="close" ng-click="closeErrors()"><span aria-hidden="true">&times;</span></button>', 
                '    <h4 class=""><i class="fa fa-exclamation"></i> {{ errorsTitle || "Error" }}</h4>', 
                '    <p>', 
                '        <ul>', 
                '            <li class="" ng-repeat="error in errors track by $index">{{ error }}</li>', 
                '        </ul>', 
                '    </p>', 
                '</div>', 
                '<!-- end errors -->', 
                '<!-- begin messages -->', 
                '<div ng-show="messages && messages.length > 0" class="alert alert-block alert-success">', 
                '    <!--<button type="button" class="close" data-dismiss="alert"><span aria-hidden="true">&times;</span></button>-->', 
                '    <button type="button" class="close" ng-click="closeMessages()"><span aria-hidden="true">&times;</span></button>', 
                '    <h4 class=""><i class="fa fa-check-square-o"></i> {{messagesTitle || "Messages"}}</h4>', 
                '    <p>', 
                '        <ul>', 
                '            <li class="" ng-repeat="message in messages track by $index">{{ message }}</li>', 
                '        </ul>', 
                '    </p>', 
                '</div>', 
                '<!-- end messages -->'
            ].join('\n')
        };
    }
})();
