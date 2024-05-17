(function () {
    'use strict';

    angular.module('app.core')
        .service('AppHttpService', httpServiceFactory);
      
    httpServiceFactory.$inject = ['$rootScope', '$q', '$http', 'setting', '$localStorage', '$log', '_'];
    function httpServiceFactory($rootScope, $q, $http, setting, $localStorage, $log, _) {

        var formUrlEncodedContentType = 'application/x-www-form-urlencoded';

        var service = {
            Get: _Get, //_FakeGet
            Post: _Post,
            Delete: _Delete,
            Put: _Put
        };

        //var fakeService = {
        //    Get: _fakeGet,
        //    Post: _fakePost
        //}

        return service;
        ////return fakeService;

        function _Get(endpoint, reqData, contentType, doNotAppendBaseUrl) {
            return _SendRequest('GET', endpoint, reqData, contentType, doNotAppendBaseUrl);
        }

        function _Post(endpoint, reqData, contentType, doNotAppendBaseUrl) {
            return _SendRequest('POST', endpoint, reqData, contentType, doNotAppendBaseUrl);
        }

        function _Delete(endpoint, reqData, contentType, doNotAppendBaseUrl) {
            return _SendRequest('DELETE', endpoint, reqData, contentType, doNotAppendBaseUrl);
        }

        function _Put(endpoint, reqData, contentType, doNotAppendBaseUrl) {
            return _SendRequest('PUT', endpoint, reqData, contentType, doNotAppendBaseUrl);
        }

        function _PrepareRequestObject(method, url, reqData, contentType, doNotAppendBaseUrl) {

            var req = { headers: {}, method: method };

            req.headers['Content-Type'] = contentType || 'application/json';
            
            req.url = doNotAppendBaseUrl ? url : setting.api.baseUrl + url;

            if (contentType === formUrlEncodedContentType)
                reqData = $.param(reqData);

            if (req.method === 'GET') {// || req.method === 'DELETE') {
                req.params = reqData;
            } else {
                req.data = reqData;
            }

            return req;
        }

        function _SendRequest(method, url, reqData, contentType, doNotAppendBaseUrl) {

            method = method.toUpperCase();

            //var loadingMessage = '';
            //var resourceName = '';
            //if (loadingMessage) {
            //    $rootScope.loadingMessage = loadingMessage;
            //} else if (resourceName) {
            //    var message = '';
            //    if (method === 'GET') {
            //        message = 'Getting';
            //    } else if (method === 'PUT') {
            //        message = 'Updating';
            //    } else if (method === 'POST') {
            //        message = 'Posting';
            //    } else if (method === 'DELETE') {
            //        message = 'Deleting';
            //    }

            //    $rootScope.loadingMessage = message + ' ' + resourceName + '...';
            //};

            var deferred = $q.defer();

            var req = _PrepareRequestObject(method, url, reqData, contentType, doNotAppendBaseUrl);
            $log.log('Request Data: ', req);

            function _successCallback(data) {
                $rootScope.loadingMessage = '';
                deferred.resolve(data.data);
            }

            function _errorCallback(data) {
                $rootScope.loadingMessage = '';
                deferred.reject(data);
            }

            // Methods depreciated. More at: http://stackoverflow.com/a/33531521
            //$rootScope.promise = $http(req)
            //    .success(_successCallback)
            //    .error(_errorCallback);

            $rootScope.promise = $http(req)
                .then(_successCallback, _errorCallback);

            return deferred.promise;
        }

        //var _fakeGet = function () { }
        //var _fakePost = function () { }

        function _FakeGet(method, url, reqData, contentType) {

            console.log('Fake Service ' + url);
            
            var data = [
                { 'Id': '3233-32423-32423-3242-3423', 'Name': 'Org-A', 'SetupCompleted': false },
                { 'Id': '3233-32423-32423-3242-3423', 'Name': 'Org-B', 'SetupCompleted': true },
                { 'Id': '3233-32423-32423-3242-3423', 'Name': 'Org-C', 'SetupCompleted': false },
                { 'Id': '3233-32423-32423-3242-3423', 'Name': 'Org-D', 'SetupCompleted': false },
                { 'Id': '3233-32423-32423-3242-3423', 'Name': 'Org-E', 'SetupCompleted': false },
                { 'Id': '3233-32423-32423-3242-3423', 'Name': 'Org-F', 'SetupCompleted': true },
                { 'Id': '3233-32423-32423-3242-3423', 'Name': 'Org-G', 'SetupCompleted': false }
            ];

            return $q.resolve(data);

        }
    }
})();
