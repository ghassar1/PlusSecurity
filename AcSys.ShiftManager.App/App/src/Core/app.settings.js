
(function () {
    'use strict';

    angular.module('app.core')
        .factory('setting', settingFactory);

    settingFactory.$inject = ['$rootScope'];

    function settingFactory($rootScope) {
        var setting = {
            layout: {
                pageSidebarMinified: false,
                pageFixedFooter: false,
                pageRightSidebar: false,
                pageTwoSidebar: false,
                pageTopMenu: false,
                pageBoxedLayout: false,
                pageWithoutSidebar: false,
                pageContentFullHeight: false,
                pageContentFullWidth: false,
                pageContentInverseMode: false,
                pageSidebarTransparent: false,
                pageWithFooter: false,
                pageLightSidebar: false,
                pageMegaMenu: false,
                pageBgWhite: false,
                pageWithoutHeader: false,
                paceTop: false,
                appName: 'Shift Manager'
            },
            api: {
                baseUrl: '/api'
            }
        };

        return setting;
    }

})();
