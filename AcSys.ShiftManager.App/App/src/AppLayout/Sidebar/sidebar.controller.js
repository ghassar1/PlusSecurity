(function () {
    'use strict';

    angular.module('app')
        .controller('sidebarController', sidebarController);

    /* -------------------------------
       2.0 CONTROLLER - Sidebar
    ------------------------------- */

    sidebarController.$inject = ['$scope', '$rootScope', '$state'];

    function sidebarController($scope, $rootScope, $state) {

        App.initSidebar();

        //var vm = $scope;
        //vm.Menu = [];
        //vm.updateMenu = updateMenu;
        //vm.updateMenu();

        //$rootScope.$on('update_menu', updateMenu);

        //function updateMenu() {
        //    $rootScope.Navigation.Build();
        //    vm.Menu = $rootScope.Navigation.Menu;
        //}
    }

})();
