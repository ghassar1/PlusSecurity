(function () {
    'use strict';

    var app = angular.module('app')
        .controller('appController', appController);

    appController.$inject = ['$scope', '$rootScope', '$state', '$localStorage', 'setting', 'accountService', '_'];
    function appController($scope, $rootScope, $state, $localStorage, setting, accountService, _) {

        $rootScope.$state = $state;
        $rootScope.setting = setting;

        //console.log('Loading gritter using $ocLazyLoad...');
        //$ocLazyLoad.load({
        //    serie: true,
        //    files: [
        //        'App/ColorAdmin/assets/plugins/gritter/css/jquery.gritter.css',
        //        'App/ColorAdmin/assets/plugins/gritter/js/jquery.gritter.min.js'
        //    ]
        //});

        //console.debug(setting.layout.appName);
    }
})();
