(function () {
    'use strict';

    angular.module('app.core')
        .factory('logger', logger);

    logger.$inject = ['_'];

    function logger(_) {

        return function ($delegate) {

            return {
                log: _log,
                info: _info,
                error: _error,
                warn: _warn,
                debug: _debug,
                table: _table
            };

            // get the type of the object
            function getClass(object) {

                // http://tobyho.com/2011/01/28/checking-types-in-javascript/
                // http://stackoverflow.com/questions/8834126/how-to-efficiently-check-if-variable-is-array-or-object-in-nodejs-v8
                // http://stackoverflow.com/questions/2265922/how-to-check-if-an-object-is-not-an-array
                // http://stackoverflow.com/questions/4775722/check-if-object-is-array
                // https://juhukinners.wordpress.com/
                return Object.prototype.toString.call(object).slice(8, -1);
            }

            function _log(obj) {

                for (var index in arguments) {

                    var data = arguments[index];

                    // type checking code should check a value to be an array before checking it to be an object because every array is an object but not all objects are arrays
                    //if (getClass(data) === 'Array') {
                    if (_.isArray(data)) {

                        if (_.find(data, isObject)) {
                            _table(data);
                        } else {
                            $delegate.log(data);
                            //_table(_.pairs(data));
                        }
                    } else if (_.isObject(data)) {
                        $delegate.log(data);
                        //_table(data);
                        //_table(_.pairs(data));
                    } else {
                        $delegate.log(data);
                    }
                }
            }

            function isObject(el) {
                return _.isObject(el);
            }

            function _info(data) {
                $delegate.info(data);
            }

            function _error(data) {
                //$delegate.error(data);
                console.log(data);
                console.error(data);
            }

            function _warn(data) {
                $delegate.warn(data);
            }

            function _debug(data) {
                $delegate.debug(data);
            }

            function _table(data, fieldFilter) {

                // if console.table() method is supported.
                if (console.table) {
                    console.table(data, fieldFilter);
                }
                else {
                    //$delegate.warn('console.table() not supported.');
                    $delegate.log(JSON.stringify(data));
                }
            }
                                    
            //return {
            //    log: function (data) {

            //        $delegate.info(typeof data);
            //        //if (typeof data === 'string') {}
            //        if (typeof data === 'object') {

            //            $delegate.info('Show data in table!');
            //            table(data);
            //        }
            //        else {
            //            // input is something else
            //        }
            //        $delegate.log(data);
            //    },

            //    info: function (data) {
            //        $delegate.info(data);
            //    },

            //    error: function (data) {
            //        $delegate.error(data);
            //    },

            //    warn: function (data) {
            //        $delegate.warn(data);
            //    },

            //    debug: function (data) {
            //        $delegate.debug(data);
            //    },

            //    table: function (data, fieldFilter) {

            //        //var isChrome = navigator.userAgent.toLowerCase().indexOf('chrome') > -1;
            //        //var isChrome = !!window.chrome;
            //        //if (isChrome)
            //        if (console.table)
            //            console.table(data, fieldFilter);
            //        else {
            //            $delegate.error('console.table() not supported.');
            //        }
            //    }
            //};

        };
    }
})();
