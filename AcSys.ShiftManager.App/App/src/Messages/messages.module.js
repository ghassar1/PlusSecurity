(function () {
    'use strict';

    angular.module('app.messages', ['app.core', 'ngSanitize', 'ngTagsInput', 'datatables'])
        .config(configModule)
        .run(runModule);

    configModule.$inject = ['$stateProvider', '$urlRouterProvider'];
    function configModule($stateProvider, $urlRouterProvider) {

        $stateProvider
            .state('app.messages', {
                url: '/messages',
                template: '<div ui-view></div>',
                abstract: true,
                data: { pageTitle: 'Messages' }
            }).state('app.messages.create', {
                url: '/new',
                templateUrl: 'App/dist/Views/Messages/create/create.html',
                controller: 'messagesCreateController',
                params: { inReplyToMessage: null },
                data: {
                    pageTitle: 'New Message',
                    breadcrumbs: ['app.dashboard.home', 'app.messages.inbox']
                },
                resolve: {
                    jQueryTagIt: resolveJQueryTagIt,
                    ngTagsInput: resolveNgTagsInput
                }
            }).state('app.messages.inbox', {
                url: '/inbox',
                template: '<div ui-view></div>',
                abstract: true
                
            }).state('app.messages.inbox.list', {
                url: '',
                templateUrl: 'App/dist/Views/Messages/list/list.html',
                controller: 'messagesListController',
                data: {
                    pageTitle: 'Inbox',
                    breadcrumbs: ['app.dashboard.home']
                }
                //resolve: { data: resolveDataTables }
            }).state('app.messages.inbox.view', {
                url: '/{id}',
                templateUrl: 'App/dist/Views/Messages/view/view.html',
                controller: 'messagesViewController',
                params: { isIncomingMessage: true, isSentMessage: false },
                data: {
                    pageTitle: 'Message',
                    breadcrumbs: ['app.dashboard.home', 'app.messages.inbox.list']
                },
                resolve: { jQueryTagIt: resolveJQueryTagIt }
            }).state('app.messages.sent', {
                url: '/sent',
                template: '<div ui-view></div>',
                abstract: true

            }).state('app.messages.sent.list', {
                url: '',
                templateUrl: 'App/dist/Views/Messages/list/list.html',
                controller: 'messagesListController',
                data: {
                    pageTitle: 'Sent Messages',
                    breadcrumbs: ['app.dashboard.home']
                }
                //resolve: { data: resolveDataTables }
            }).state('app.messages.sent.view', {
                url: '/{id}',
                templateUrl: 'App/dist/Views/Messages/view/view.html',
                controller: 'messagesViewController',
                params: { isIncomingMessage: false, isSentMessage: true },
                data: {
                    pageTitle: 'Message',
                    breadcrumbs: ['app.dashboard.home', 'app.messages.sent.list']
                },
                resolve: { jQueryTagIt: resolveJQueryTagIt }
            }).state('app.messages.update', {
                url: '/{id}/update',
                templateUrl: 'App/dist/Views/Messages/update/update.html',
                controller: 'messagesUpdateController',
                data: {
                    pageTitle: 'Update Message',
                    breadcrumbs: ['app.dashboard.home', 'app.messages.inbox']
                }
                //resolve: { data: resolveUpdate }
            });
    }

    runModule.$inject = [];
    function runModule() {
        // ...
    }

    resolveCreate.$inject = [];
    function resolveCreate() {
        return null;
    }

    resolveDataTables.$inject = ['$ocLazyLoad'];
    function resolveDataTables($ocLazyLoad) {
        return $ocLazyLoad.load({
            serie: true,
            files: [
                'App/ColorAdmin/assets/plugins/DataTables/media/css/dataTables.bootstrap.min.css',
                'App/ColorAdmin/assets/plugins/DataTables/extensions/Buttons/css/buttons.bootstrap.min.css',
                'App/ColorAdmin/assets/plugins/DataTables/extensions/Responsive/css/responsive.bootstrap.min.css',
                'App/ColorAdmin/assets/plugins/DataTables/media/js/jquery.dataTables.js',
                'App/ColorAdmin/assets/plugins/DataTables/media/js/dataTables.bootstrap.min.js',
                'App/ColorAdmin/assets/plugins/DataTables/extensions/Buttons/js/dataTables.buttons.min.js',
                'App/ColorAdmin/assets/plugins/DataTables/extensions/Buttons/js/buttons.bootstrap.min.js',
                'App/ColorAdmin/assets/plugins/DataTables/extensions/Buttons/js/buttons.flash.min.js',
                'App/ColorAdmin/assets/plugins/DataTables/extensions/Buttons/js/jszip.min.js',
                'App/ColorAdmin/assets/plugins/DataTables/extensions/Buttons/js/pdfmake.min.js',
                'App/ColorAdmin/assets/plugins/DataTables/extensions/Buttons/js/vfs_fonts.min.js',
                'App/ColorAdmin/assets/plugins/DataTables/extensions/Buttons/js/buttons.html5.min.js',
                'App/ColorAdmin/assets/plugins/DataTables/extensions/Buttons/js/buttons.print.min.js',
                'App/ColorAdmin/assets/plugins/DataTables/extensions/Responsive/js/dataTables.responsive.min.js'
            ]
        });
    }

    resolveJQueryTagIt.$inject = ['$ocLazyLoad'];
    function resolveJQueryTagIt($ocLazyLoad) {
        return $ocLazyLoad.load({
            serie: true,
            files: [
                'App/ColorAdmin/assets/plugins/jquery-tag-it/css/jquery.tagit.css',
                'App/ColorAdmin/assets/plugins/jquery-tag-it/js/tag-it.min.js'
            ]
        });
    }

    resolveNgTagsInput.$inject = ['$ocLazyLoad'];
    function resolveNgTagsInput($ocLazyLoad) {
        return $ocLazyLoad.load({
            serie: true,
            files: [
                'App/dist/js/ng-tags-input.min.js',
                'App/dist/css/ng-tags-input.min.css'
            ]
        });
    }

})();
