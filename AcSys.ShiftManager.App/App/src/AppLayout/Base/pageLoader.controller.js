(function () {
    'use strict';

    var app = angular.module('app')
        .controller('pageLoaderController', pageLoaderController);

    /* -------------------------------
    6.0 CONTROLLER - Page Loader
    ------------------------------- */

    pageLoaderController.$inject = ['$scope', '$rootScope', '$state'];

    function pageLoaderController($scope, $rootScope, $state) {
        App.initPageLoad();
    }

})();
