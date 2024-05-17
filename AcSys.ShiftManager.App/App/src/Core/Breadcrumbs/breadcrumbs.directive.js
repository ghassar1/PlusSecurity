(function () {
    'use strict';

    angular.module('app.core')
        .directive('breadcrumbs', breadcrumbsDirective);

    function breadcrumbsDirective() {

        function link(scope, element, attrs) {
            //console.log('Element: ', element, attrs);

            var vm = scope;
            vm.click = click;

            // copy attribute values to directive scope
            vm.textProperty = attrs.textproperty;
            vm.hrefProperty = attrs.hrefproperty;
            //console.debug(vm);

            function click() {

            }
        }

        return {
            //restrict: 'AEC',
            link: link,
            //replace: true,
            //transclude: true,
            options: '=',
            scope: {
                list: '=list',
                hrefProperty: '@hrefProperty',
                textProperty: '@textProperty'
            },
            template: [
                '<ol ng-if="list && list.length > 0" class="breadcrumb pull-right">',
                    '<li ng-repeat="bc in list">',
                        '<a ng-if="bc[hrefProperty] || bc.href" href="{{bc[hrefProperty] || bc.href}}">{{bc[textProperty] || bc.text}}</a>',
                        '<span ng-if="!(bc[hrefProperty] || bc.href)">{{bc.text}}</span>',
                    '</li>',
                '</ol>'
            ].join('')
        };
    }
})();
