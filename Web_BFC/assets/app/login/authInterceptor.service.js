(function () {

    function authInterceptorService($q, $injector, $location, WebStorageService) {
        var authInterceptorServiceFactory = {};

        var _request = function (config) {

            config.headers = config.headers || {};

            var authData = WebStorageService.get('ss','authInfo');
            if (authData) {
                config.headers.Authorization = 'Bearer ' + authData.token;
            }
            return config;
        }

        var _responseError = function (rejection) {
            if (rejection.status === 401) {
                //var authData = WebStorageService.get('ss', 'authInfo');

                //if (authData) {
                //    if (authData.useRefreshTokens) {
                //        $location.path('/refresh');
                //        return $q.reject(rejection);
                //    }
                //}

                var authService = $injector.get('authService');
                authService.logOut();
                $location.path('/login');
            }
            return $q.reject(rejection);
        }

        authInterceptorServiceFactory.request = _request;
        authInterceptorServiceFactory.responseError = _responseError;

        return authInterceptorServiceFactory;
    }

    angular.module('app')
    .factory('authInterceptorService', authInterceptorService);

    authInterceptorService.$inject = ['$q', '$injector', '$location', 'WebStorageService'];

})();