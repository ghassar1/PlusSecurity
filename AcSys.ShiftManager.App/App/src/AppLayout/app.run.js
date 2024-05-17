(function () {
    'use strict';

    angular.module('app')
        .run(runAppModule);

    runAppModule.$inject = ['$rootScope', '$state', '$ocLazyLoad', 'setting', '$localStorage', 'accountService', 'notificationsService'];
    function runAppModule($rootScope, $state, $ocLazyLoad, setting, $localStorage, accountService, notificationsService) {

        //moment.tz.setDefault("Europe/London");

        $rootScope.$on('$includeContentLoaded', includeContentLoaded);
        $rootScope.$on('$viewContentLoaded', viewContentLoaded);

        $rootScope.$on('$stateChangeStart', appStateChangeStart);
        $rootScope.$on('$stateChangeSuccess', appStateChangeSuccess);
        $rootScope.$on('$stateNotFound', appStateNotFound);
        $rootScope.$on('$stateChangeError', appStateChangeError);

        function includeContentLoaded() {
            //console.debug('Include Content Loaded!');

            handleSlimScroll();
        }

        function viewContentLoaded() {
            //console.debug('View Content Loaded!');
        }

        function appStateNotFound(event, unfoundState, fromState, fromParams) {
            Pace.stop();

            console.log(unfoundState.to); // 'lazy.state'
            console.log(unfoundState.toParams); // {a:1, b:2}
            console.log(unfoundState.options); // {inherit:false} + default options

            //$rootScope.requestInProcess = false;

            $state.go('app.error.notfound');
        }

        appStateChangeError.$inject = ['event', 'toState', 'toParams', 'fromState', 'fromParams'];
        function appStateChangeError(event, toState, toParams, fromState, fromParams, error) {
            Pace.stop();

            //$rootScope.requestInProcess = false;

            console.log('$stateChangeError', error);
        }

        appStateChangeStart.$inject = ['event', 'toState', 'toParams', 'fromState', 'fromParams'];
        function appStateChangeStart(event, toState, toParams, fromState, fromParams) {

            //console.trace('appStateChangeStart: started', fromState, toState);

            //$rootScope.requestInProcess = true;

            resetLayoutSettings();

            App.scrollTop();

            $('.pace .pace-progress').addClass('hide');
            $('.pace').removeClass('pace-inactive');

            if (!fromState.data) fromState.data = {};
            if (!toState.data) toState.data = {};
            if (!toState.data.auth) toState.data.auth = { allowAnonymous: false, roles: [] };

            //console.trace('From State: ', fromState);
            //console.trace('To State: ', toState);

            // Check if the user must be authenticated to visit the target state. Either AllowAnonymous is false in state auth data or roles array exists (even if empty)
            if (!toState.data.auth.allowAnonymous || toState.data.auth.roles) {

                var LoggedInUser = $rootScope.User;
                if (LoggedInUser.hasNotLoggedIn()) { //  && toState.name !== 'app.account.login'
                    return forwardToLogin(event);
                }

                var toStateHasRoles = toState.data.auth.roles && toState.data.auth.roles.length > 0;
                if (LoggedInUser.hasUserData()) {
                    if (toStateHasRoles && !LoggedInUser.hasAnyRole(toState.data.auth.roles)) {
                        return forwardToUnath(event);   //forwardToLogin(event);
                    }
                } else {

                    //console.debug('appStateChangeStart: Get User Data.');
                    accountService.getLoggedInUserDetails().then(function (data) {

                        //console.debug('appStateChangeStart: Got User Data: ', data);

                        LoggedInUser.setUserData(data);
                        $rootScope.Navigation.Build();

                        if (toStateHasRoles && !LoggedInUser.hasAnyRole(toState.data.auth.roles)) {
                            return forwardToUnath(event);  //forwardToLogin(event);
                        }

                    }, function (error) {
                        console.log('Error Getting Logged In User Details.');
                        event.preventDefault();
                        return $state.go('app.error.app', { error: error });
                    });
                }
            }

            //$rootScope.$broadcast('update_menu');
            //$rootScope.Navigation.Build();

            //console.trace('appStateChangeStart: completed');
        }

        function forwardToUnath(event) {
            //$rootScope.notifications.notify('You are not authorized to visit this page.');
            event.preventDefault();
            throw new AuthError();
            //$state.go('app.error.unauth');
        }

        function forwardToLogin(event) {
            $rootScope.notifications.notify('You must be logged in to visit this page. Please log in.');
            event.preventDefault();
            $state.go('app.account.login');
        }

        appStateChangeSuccess.$inject = ['event', 'toState', 'toParams', 'fromState', 'fromParams'];
        function appStateChangeSuccess(event, toState, toParams, fromState, fromParams) {

            //console.trace('appStateChangeSuccess: started', fromState, toState);

            Pace.restart();
            App.initPageLoad();
            App.initSidebarSelection();
            App.initSidebarMobileSelection();
            setTimeout(function () {
                App.initLocalStorage();
                App.initComponent();
            }, 0);

            $rootScope.Navigation.Build();
            //$rootScope.$broadcast('update_menu');

            //$rootScope.notifications.clear();
            $rootScope.notifications.clearMessages();
            $rootScope.notifications.clearErrors();

            var LoggedInUser = $rootScope.User;

            $rootScope.breadcrumbs.clear();
            if (toState.data.breadcrumbs) {

                toState.data.breadcrumbs.forEach(function (el) {
                    var bcState = $state.get(el);
                    var href = el === 'app.dashboard.home' ? LoggedInUser.getHomeHref() : $state.href(el);
                    $rootScope.breadcrumbs.add({ text: bcState.data.pageTitle, href: href });
                });
                $rootScope.breadcrumbs.add({ text: toState.data.pageTitle, href: '' });
            }

            //if (toState.data.roles) { }

            if (LoggedInUser.hasLoggedIn() && (!toState.data.auth || toState.data.auth.allowAnonymous === false)) {
                $rootScope.refreshHeaderNotifications();
            }

            // set page layout settings from state
            if (toState.data.layout) {
                $rootScope.setting.layout.pageWithoutHeader = toState.data.layout.header === false;
                $rootScope.setting.layout.pageWithoutSidebar = toState.data.layout.sidebar === false;
            }

            $rootScope.currentPage = { title: '' };
            if (toState.data) {
                $rootScope.currentPage.title = toState.data.pageTitle || 'Page';
            }

            //$rootScope.requestInProcess = false;

            //console.trace('appStateChangeSuccess: completed');
        }

        function resetLayoutSettings() {
            // reset layout setting
            $rootScope.setting.layout.pageSidebarMinified = false;
            $rootScope.setting.layout.pageFixedFooter = false;
            $rootScope.setting.layout.pageRightSidebar = false;
            $rootScope.setting.layout.pageTwoSidebar = false;
            $rootScope.setting.layout.pageTopMenu = false;
            $rootScope.setting.layout.pageBoxedLayout = false;
            $rootScope.setting.layout.pageWithoutSidebar = false;
            $rootScope.setting.layout.pageContentFullHeight = false;
            $rootScope.setting.layout.pageContentFullWidth = false;
            $rootScope.setting.layout.paceTop = false;
            $rootScope.setting.layout.pageLanguageBar = false;
            $rootScope.setting.layout.pageSidebarTransparent = false;
            $rootScope.setting.layout.pageWideSidebar = false;
            $rootScope.setting.layout.pageLightSidebar = false;
            $rootScope.setting.layout.pageFooter = false;
            $rootScope.setting.layout.pageMegaMenu = false;
            $rootScope.setting.layout.pageWithoutHeader = false;
            $rootScope.setting.layout.pageBgWhite = false;
            $rootScope.setting.layout.pageContentInverseMode = false;
        }
    }
})();
