(function () {
    'use strict';

    angular.module('app.core')
        .run(setup);

    setup.$inject = ['$rootScope', '$state', '_', '$localStorage'];
    function setup($rootScope, $state, _, $localStorage) {
        var user = new User(_, $localStorage, $state);
        $rootScope.User = user;
    }

    function User(_, $localStorage, $state) {

        var me = this;
        me.$state = $state;
        
        var userData = null;
        var authData = null;

        me.Id = null;
        me.Name = null;
        me.Email = null;
        me.Role = { Id: null, Name: null };

        me.login = login;
        me.logout = logout;
        me.goToHome = goToHome;
        me.getHomeHref = getHomeHref;

        me.hasLoggedIn = hasLoggedIn;
        me.hasNotLoggedIn = hasNotLoggedIn;

        me.getAuthData = getAuthData;
        me.getAccessToken = getAccessToken;

        me.hasUserData = hasUserData;
        me.hasNoUserData = hasNoUserData;
        me.setUserData = setUserData;
        me.getUserData = getUserData;
        
        me.hasRole = hasRole;
        me.hasRoles = hasRoles;
        me.hasAnyRole = hasAnyRole;

        function goToHome() {
            if (!$state)
                throw new Error('$state not defined');

            if (me.hasAnyRole('SuperAdmin, Admin')) {
                $state.go('app.dashboard.home');
            } else if (me.hasAnyRole('RecManager')) {
                $state.go('app.employees.list');
            } else if (me.hasAnyRole('HRManager, Employee')) {
                $state.go('app.shifts.list');
            }
        }

        function getHomeHref() {
            if (!$state)
                throw new Error('$state not defined');

            var href = '';
            if (me.hasAnyRole('SuperAdmin, Admin')) {
                href = $state.href('app.dashboard.home');
            } else if (me.hasAnyRole('RecManager')) {
                href = $state.href('app.employees.list');
            } else if (me.hasAnyRole('HRManager, Employee')) {
                href = $state.href('app.shifts.list');
            }
            return href;
        }

        function login(data) {

            $localStorage.authData = data;
            //$rootScope.authData = data;

            authData = data;

            if (authData === null)
                logout();
        }

        function getAuthData() {

            if (authData === undefined || authData === null)
                authData = $localStorage.authData;

            if (authData === undefined)
                authData = null;

            return authData;
        }

        function getAccessToken() {
            var authData = getAuthData();
            return authData && authData.access_token ? authData.access_token : '';
        }

        function setUserData(user) {

            $localStorage.userData = user;
            //$rootScope.userData = user;
            userData = user;

            setupUserData(user);
        }

        function setupUserData(user) {
            me.Id = user.id;
            me.Name = user.firstName + ' ' + user.lastName;
            me.Email = user.email;
            me.Role = { Id: user.role.id, Name: user.role.name };
        }

        function logout() {

            //delete $rootScope.userData;
            //delete $rootScope.authData;
            delete $localStorage.authData;

            authData = null;
            userData = null;

            me.Id = null;
            me.Name = null;
            me.Email = null;
            me.Role = null;
        }

        function getUserData() {
            return userData;
        }

        function hasNoUserData() {
            return hasNotLoggedIn() || userData === undefined || userData === null || userData.role === undefined || userData.role === null;
        }

        function hasUserData() {
            return !hasNoUserData();
        }

        function hasLoggedIn() {
            return !me.hasNotLoggedIn();
        }

        function hasNotLoggedIn() {
            var authData = getAuthData();
            return authData === undefined || authData === null || authData.access_token === undefined || authData.access_token === null;
        }

        function hasAnyRole(roles) {

            if (me.hasNoUserData()) return false;

            if (roles === undefined || roles === null) throw new Error('No roles specified.');

            //console.log('Roles: ', roles, typeof roles);

            var has = false;
            if (typeof roles === 'string' || roles instanceof String) { // roles.constructor === String
                //console.log('Roles is String');
                if (roles.indexOf(',') > -1) {
                    //console.log('Roles is String with ,');
                    has = hasRoles(roles.split(','));
                } else {
                    //console.log('Roles is String without ,');
                    has = hasRole(roles);
                }
            } else if (roles.constructor === Array) {
                //console.log('Roles is Array');
                has = hasRoles(roles);
            }
            return has;
        }

        function hasRole(role) {
            if (me.hasNoUserData()) return false;
            
            if (role === undefined || role === null) throw new Error('No roles specified.');

            return role.toUpperCase() === userData.role.name.toUpperCase();
        }

        function hasRoles(roles) {

            //console.log('Type of Roles', typeof roles);
            if (me.hasNoUserData()) return false;
            
            if (roles === undefined || roles === null || roles.length < 1) {
                throw new Error('No roles specified.');
            }
            //console.log('Roles are defined.');
            //var has = false;
            //angular.forEach(roles, function (role) {
            //    has = has || role.toUpperCase() === userData.role.name.toUpperCase();
            //    console.debug(role.toUpperCase(), userData.role.name.toUpperCase(), has);
            //});
            //return has;

            //return _.contains(roles, userData.role.name);
            var filtered = _.filter(roles, function (role) {
                
                //console.log('Type of role: ' + typeof role, role.trim().toUpperCase(), userData.role.name.trim().toUpperCase(), role.trim().toUpperCase() === userData.role.name.trim().toUpperCase());
                // trim the role for comparison in case csv contains spaces i.e. 'role1, role2' instead of 'role1,role2'
                return role.trim().toUpperCase() === userData.role.name.trim().toUpperCase();
            });

            //console.log('Filtered: ', filtered.length);
            return filtered.length > 0;
        }

        //function hasAnyRoles(roles) {

        //    if (me.hasNoUserData()) return false;

        //    //var userCanAccessState = true;
        //    //var userMustHaveSpecificRole = roles.length > 0;
        //    //if (userMustHaveSpecificRole) {
        //    //    var toStateHasRoles = roles && roles.length > 0;
        //    //    var toStateHasNoRoles = !toStateHasRoles;
        //    //    var userHasRequiredRole = roles.indexOf(me.RoleName) > -1;
        //    //    userCanAccessState = toStateHasNoRoles || userHasRequiredRole;
        //    //}

        //    //var userCanAccessState = roles && roles.length > 0 && (roles.indexOf(me.RoleName) > -1);

        //    var rolesArr = [];
        //    if (roles instanceof String) {
        //        if (roles.indexOf(',') > -1) {
        //            rolesArr = roles.split(',');
        //        } else {
        //            rolesArr.push(roles);
        //        }
        //    } else if (roles && roles.constructor === Array) {
        //        rolesArr = roles;
        //    }
        //    var userCanAccessState = hasAnyRole(rolesArr);
        //    return userCanAccessState;
        //}
    }
})();
