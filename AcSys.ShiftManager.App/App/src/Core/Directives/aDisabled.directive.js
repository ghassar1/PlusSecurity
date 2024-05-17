(function () {
    'use strict';

    // Directive for disabling anchor tags based on condition.
    // When condition is true: adds disabled class to anchor, disables ng-click and removes href attribute.
    // <a a-disabled='1 === 1' ng-click='actionAction()'></a>
    // http://stackoverflow.com/a/25391043/3423802
    // http://stackoverflow.com/questions/23425254/enable-disable-anchor-tags-using-angularjs
    angular.module('app.core')
        .directive('aDisabled', aDisabled);

    function aDisabled() {
        return {
            compile: compile
        };
    }

    function compile(tElement, tAttrs, transclude) {
        //Disable ngClick
        tAttrs.ngClick = '!(' + tAttrs.aDisabled + ') && (' + tAttrs.ngClick + ')';

        //return a link function
        return function (scope, iElement, iAttrs) {

            //Toggle 'disabled' to class when aDisabled becomes true
            scope.$watch(iAttrs.aDisabled, function (newValue) {
                if (newValue !== undefined) {
                    iElement.toggleClass('disabled', newValue);
                    //iElement.attr('href', newValue ? '' : 'javascript:;');
                    if (newValue) {
                        //iElement.removeAttr('href');
                        //iElement.attr('title', 'Last Page');
                        iElement.css('cursor', 'not-allowed');
                        iElement.css('pointer-events', 'none');
                    }
                    else {
                        //iElement.attr('href', 'javascript:;');
                        //iElement.attr('title', 'Next Page');
                        iElement.css('cursor', 'pointer');
                        iElement.css('pointer-events', 'all');
                    }
                }
            });

            //Disable href on click
            iElement.on('click', function (e) {
                if (scope.$eval(iAttrs.aDisabled)) {
                    e.preventDefault();
                }
            });
        };
    }

})();
