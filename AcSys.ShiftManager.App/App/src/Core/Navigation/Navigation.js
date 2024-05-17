(function () {
    'use strict';

    angular.module('app.core')
        .run(setup);

    setup.$inject = ['$rootScope'];
    function setup($rootScope) {

        var navigation = new Navigation($rootScope);
        $rootScope.Navigation = navigation;
    }

    function Navigation($rootScope) {

        var me = this;
        me.Menu = [];

        me.Build = build;
        me.Add = add;

        me.Rebuild = rebuild;
        
        var menuItems = [
            { 'name': 'Login', 'state': 'app.account.login', unauth: true, roles: [] },
            { 'name': 'Dashboard', 'state': 'app.dashboard.home', roles: [] },
            { 'name': 'Notifications', 'state': 'app.notifications.list', roles: [] },
            {
                'name': 'Messages',
                'state': 'app.messages',
                roles: [],
                subMenuItems: [
                    { 'name': 'Inbox', 'state': 'app.messages.inbox', roles: [] },
                    { 'name': 'Sent', 'state': 'app.messages.sent', roles: [] }
                ]
            },
            { 'name': 'Shifts', 'state': 'app.shifts.list', roles: [] },
            { 'name': 'Users', 'state': 'app.users.list', roles: ['SuperAdmin', 'Admin'] },
            { 'name': 'Logs', 'state': 'app.logs.list', roles: ['SuperAdmin'] },
            { 'name': 'Logout', 'state': 'app.account.logout', roles: [] }
        ];

        function rebuild() {
            me.Menu = [];

            me.Build();
        }

        function build() {

            me.Menu = [];
            //console.log('User Data: ', $rootScope.User);
            angular.forEach(menuItems, function (menuItem) {
                //console.log(menuItem);
                add(menuItem, $rootScope.User);
            });

            $rootScope.Menu = me.Menu;

            return me;
        }

        function add(menuItem, user) {

            var userIsAuthenticated = user && user.Role;
            var userIsUnauthenticated = !userIsAuthenticated;

            if (userIsAuthenticated) {

                var menuItemHasRoles = menuItem.roles && menuItem.roles.length > 0;
                var menuItemHasNoRoles = !menuItemHasRoles;
                var menuItemHasUsersRole = menuItem.roles.indexOf(user.Role.Name) > -1;

                if (!menuItem.unauth && (menuItemHasNoRoles || userIsAuthenticated && menuItemHasRoles && menuItemHasUsersRole)) {
                    me.Menu.push(menuItem);
                }

            } else if (menuItem.unauth) {
                me.Menu.push(menuItem);
            }

            return me;
        }
    }
})();
