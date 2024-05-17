(function () {
    'use strict';

    angular.module('app.core')
        .run(setup);

    setup.$inject = ['$rootScope'];
    function setup($rootScope) {
        var breadcrumbs = new Breadcrumbs();
        $rootScope.breadcrumbs = breadcrumbs;
    }

    function Breadcrumbs() {

        var me = this;
        me.list = [];
        me.add = add;
        me.set = set;
        me.clear = clear;

        function set(param) {
            me.clear();
            return me.add(param);
        }

        function add(param) {

            if (!me.list) me.clear();

            if (!param) {
                console.error(param);
                throw new Error('Invalid breadcrumbs param.');
            }

            if (typeof param === 'string') {
                me.list.push({ text: param, href: '' });
            } else if (Array.isArray(param)) {
                param.forEach(function (el) {
                    me.add(el);
                });
            } else if (typeof param === 'object') {
                
                if (!param || !param.text) {
                    console.error(param);
                    throw new Error('Invalid breadcrumbs param.');
                }

                //var updated = false;
                //for (var index in me.list) {
                //    if (me.list[index].text === param.text) {
                //        me.list[index].href = param.href;
                //        updated = true;
                //    }
                //}

                //if (!updated) me.list.push(param);
                me.list.push(param);

            } else {
                console.error(param);
                throw new Error('Invalid breadcrumbs param: ' + typeof param);
            }

            return me;
        }

        function clear() {
            me.list = [];
            return me;
        }
    }

})();
