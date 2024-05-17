(function () {
    'use strict';

    var app = angular.module('app')
        .controller('topMenuController', topMenuController);

    /* -------------------------------
       5.0 CONTROLLER - Top Menu
    ------------------------------- */

    topMenuController.$inject = ['$scope', '$rootScope', '$state'];

    function topMenuController($scope, $rootScope, $state) {

        setTimeout(function () {
            App.initTopMenu();
        }, 0);
    }
})();
