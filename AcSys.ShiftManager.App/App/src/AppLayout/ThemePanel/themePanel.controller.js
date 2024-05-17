(function () {
    'use strict';

    angular.module('app')
        .controller('themePanelController', themePanelController);

    /* -------------------------------
       7.0 CONTROLLER - Theme Panel
    ------------------------------- */

    themePanelController.$inject = ['$scope', '$rootScope', '$state'];

    function themePanelController($scope, $rootScope, $state) {

        App.initThemePanel();
    }

})();
