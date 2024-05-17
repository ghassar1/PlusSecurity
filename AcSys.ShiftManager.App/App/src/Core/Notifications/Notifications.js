(function () {
    'use strict';

    angular.module('app.core')
        .run(setup);

    setup.$inject = ['$rootScope', 'usersService'];
    function setup($rootScope, usersService) {

        var notifications = new Notifications();
        $rootScope.notifications = notifications;

        $rootScope.refreshHeaderNotifications = refreshHeaderNotifications;
        function refreshHeaderNotifications() {
            usersService.listNotifications().then(function (notifications) {

                $rootScope.headerNotifications = notifications;
                //console.log($rootScope.headerNotifications);

                $rootScope.totalNewNotifications = 0;
                $rootScope.totalNewMessages = 0;
                angular.forEach(notifications, function (notification) {
                    if (notification.isMessage) {
                        $rootScope.totalNewMessages = $rootScope.totalNewMessages + 1;
                    } else {
                        $rootScope.totalNewNotifications = $rootScope.totalNewNotifications + 1;
                    }
                });
            }, function (error) { });
        }
    }

    function Notifications() {

        var me = this;

        me.errors = [];
        me.errorsTitle = 'Error';
        me.error = _error;
        me.addError = _addError;

        me.messages = [];
        me.messagesTitle = 'Done';
        me.message = _message;
        me.addMessage = _addMessage;

        me.notificationTime = 8000;
        me.notify = addGritterNotification;

        me.clear = _clear;
        me.clearMessages = _clearMessages;
        me.clearErrors = _clearErrors;
        me.clearNotifications = _clearGritterNotifications;

        me.clear();

        function _clear() {
            me.clearMessages();
            me.clearErrors();
            me.clearNotifications();
        }

        //#region success messages

        function _clearMessages() {
            //me.messages = [];
            //me.messages.splice(0, me.messages.length);
            me.messages.length = 0;
            me.messagesTitle = 'Done';
        }

        function _message(param) {
            _clearMessages();
            _addMessage(param);
        }
        
        function _addMessage(param) {
            if (Array.isArray(param)) {
                param.forEach(function (el) {
                    me.message(el);
                });
            } else if (typeof param === 'string') {
                me.messages.push(param);
            } else if (typeof param === 'object') {
                if (param.message) {
                    me.message(param.message);
                }
                if (param.title) {
                    me.messagesTitle = param.title;
                }
            } else {
                throw new Error('Invalid notification param type: ' + typeof param);
            }
        }

        //#endregion

        //#region error messages

        function _error(param) {
            _clearErrors();
            _addError(param);
        }

        function _addError(param) {
            if (Array.isArray(param)) {
                param.forEach(function (el) {
                    me.error(el);
                });
            } else if (typeof param === 'string') {
                me.errors.push(param);
            } else if (typeof param === 'object') {
                if (param.message) {
                    me.error(param.message);
                }
                if (param.title) {
                    me.errorsTitle = param.title;
                }
            } else {
                throw new Error('Invalid notification param type: ' + typeof param);
            }
        }

        function _clearErrors() {
            //me.errors = [];
            //me.errors.splice(0, me.errors.length);
            me.errors.length = 0;
            me.errorsTitle = 'Error';
        }

        //#endregion

        function addGritterNotification(text, title, image) {

            var notification = {
                title: title,                   // (string | mandatory) the heading of the notification
                text: text,                     // (string | mandatory) the text inside the notification
                image: image,                   // (string | optional) the image to display on the left
                sticky: false,                  // (bool | optional) if you want it to fade out on its own or just sit there
                time: me.notificationTime,      // (int | optional) the time you want it to be alive for before fading out (milliseconds)
                fade_in_speed: 'medium',        // how fast notifications fade in (string or int)
                fade_out_speed: 2000,           // how fast the notices fade out
                class_name: 'my-sticky-class',  //my-class // (string | optional) the class name you want to apply directly to the notification for custom styling
                before_open: before_open,       // (function | optional) function called before it opens
                after_open: after_open,         // (function | optional) function called after it opens
                before_close: before_close,     // (function | optional) function called before it closes
                after_close: after_close        // (function | optional) function called after it closes
            };

            var gritter_notification_id = $.gritter.add(notification);

            function before_open() {
                //console.debug('I am a sticky called before it opens');
            }

            function after_open(e) {
                //console.debug('I am a sticky called after it opens: \nI am passed the jQuery object for the created Gritter element...\n' + e);
            }

            function before_close(e, manual_close) {
                // the manual_close param determined if they closed it by clicking the 'x'
                //console.debug('I am a sticky called before it closes: I am passed the jQuery object for the Gritter element... \n' + e);
            }

            function after_close() {
                //console.debug('I am a sticky called after it closes');
            }
        }

        function _clearGritterNotifications() {
            // remove all gritter notification
            $.gritter.removeAll({
                before_close: before_close,
                after_close: after_close
            });

            //$.gritter.remove(gritter_notification_id, {
            //    fade: true, // optional
            //    speed: 'fast' // optional
            //});

            function before_close(e) {
                //console.log('I am called before all notifications are closed.  I am passed the jQuery object containing all  of Gritter notifications.\n' + e);
            }

            function after_close() {
                //console.log('I am called after everything has been closed.');
            }
        }
    }
})();
