(function () {

    function authService($http, $q, WebStorageService, ngAuthSettings, jwtHelper, $log) {
        var serviceBase = ngAuthSettings.apiServiceBaseUri;
        var authServiceFactory = {};

        var _authentication = {
            isAuth: false,
            userName: "",
            useRefreshTokens: false,
            rol: "",
            usuario: "",
            empresa: "",
            idEmpresa: ""
        };

        var _externalAuthData = {
            provider: "",
            userName: "",
            externalAccessToken: ""
        };

        var _saveRegistration = function (registration) {

            _logOut();

            return $http.post(serviceBase + 'api/account/register', registration).then(function (response) {
                return response;
            });

        };

        var _login = function (loginData) {

            var data = "grant_type=password&username=" + loginData.userName + "&password=" + loginData.password;

            if (loginData.useRefreshTokens) {
                data = data + "&client_id=" + ngAuthSettings.clientId;
            }

            var deferred = $q.defer();
            //token solamente
            $http.post(serviceBase + 'oauth/token', data, { headers: { 'Content-Type': 'application/x-www-form-urlencoded' } })
                .success(function (response) {
                
                if (loginData.useRefreshTokens) {
                    WebStorageService.set('ss','authInfo', { token: response.access_token, userName: loginData.userName, refreshToken: response.refresh_token, useRefreshTokens: true });
                }
                else {
                    WebStorageService.set('ss', 'authInfo', { token: response.access_token, userName: loginData.userName, refreshToken: "", useRefreshTokens: false });
                }
                _authentication.isAuth = true;
                _authentication.userName = loginData.userName;
                _authentication.useRefreshTokens = loginData.useRefreshTokens;
                var tokenPayload = jwtHelper.decodeToken(response.access_token);
                if (_authentication.isAuth == true && tokenPayload) {
                    _authentication.rol = tokenPayload.rol;
                    _authentication.usuario = tokenPayload.nombres + ' ' + tokenPayload.apellidos;
                    _authentication.empresa = tokenPayload.empresa;
                    _authentication.idEmpresa = tokenPayload.idEmpresa;
                    
                }
                deferred.resolve(response);

            }).error(function (err, status) {                
                deferred.reject(err);
            });
            
            return deferred.promise;

        };

        var _logOut = function () {
            
            var deferred = $q.defer();
            var uri = URL_API + 'api/accounts/logout'
            $http({
                url: uri,
                method: "GET"
            })
            .success(function (data, status, headers, config) {
                $log.info('Sesión cerrada correctamente');                
                WebStorageService.remove('ss', 'authInfo');
                _authentication.isAuth = false;
                _authentication.userName = "";
                _authentication.useRefreshTokens = false;
                _authentication.rol = "";
                _authentication.usuario = "";
                _authentication.empresa = "";
                _authentication.idEmpresa = "";
                deferred.resolve(data);
            })
            .error(function (err) {
                $log.error('error al intentar cerrar sesion.');
                deferred.reject(err)
            });
            return deferred.promise;
        };

        var _fillAuthData = function () {

            var authData = WebStorageService.get('ss', 'authInfo');
            if (authData) {
                _authentication.isAuth = true;
                _authentication.userName = authData.userName;
                _authentication.useRefreshTokens = authData.useRefreshTokens;
                var tokenPayload = jwtHelper.decodeToken(authData.token);
                if (_authentication.isAuth == true && tokenPayload) {
                    _authentication.rol = tokenPayload.rol;
                    _authentication.usuario = tokenPayload.nombres + ' ' + tokenPayload.apellidos;
                    _authentication.empresa = tokenPayload.empresa;                    
                }
            }
        };

        var _refreshToken = function () {
            var deferred = $q.defer();

            var authData = WebStorageService.get('ss', 'authInfo');

            if (authData) {

                if (authData.useRefreshTokens) {

                    var data = "grant_type=refresh_token&refresh_token=" + authData.refreshToken + "&client_id=" + ngAuthSettings.clientId;

                    WebStorageService.remove('ss', 'authInfo');

                    $http.post(serviceBase + 'token', data, { headers: { 'Content-Type': 'application/x-www-form-urlencoded' } }).success(function (response) {

                        WebStorageService.set('ss', 'authInfo', { token: response.access_token, userName: response.userName, refreshToken: response.refresh_token, useRefreshTokens: true });

                        deferred.resolve(response);

                    }).error(function (err, status) {
                        _logOut();
                        deferred.reject(err);
                    });
                }
            }

            return deferred.promise;
        };

        var _obtainAccessToken = function (externalData) {

            var deferred = $q.defer();

            $http.get(serviceBase + 'api/account/ObtainLocalAccessToken', { params: { provider: externalData.provider, externalAccessToken: externalData.externalAccessToken } }).success(function (response) {

                WebStorageService.set('ss', 'authInfo', { token: response.access_token, userName: response.userName, refreshToken: "", useRefreshTokens: false });

                _authentication.isAuth = true;
                _authentication.userName = response.userName;
                _authentication.useRefreshTokens = false;

                deferred.resolve(response);

            }).error(function (err, status) {
                _logOut();
                deferred.reject(err);
            });

            return deferred.promise;

        };

        var _registerExternal = function (registerExternalData) {

            var deferred = $q.defer();

            $http.post(serviceBase + 'api/account/registerexternal', registerExternalData).success(function (response) {

                WebStorageService.set('ss', 'authInfo', { token: response.access_token, userName: response.userName, refreshToken: "", useRefreshTokens: false });

                _authentication.isAuth = true;
                _authentication.userName = response.userName;
                _authentication.useRefreshTokens = false;

                deferred.resolve(response);

            }).error(function (err, status) {
                _logOut();
                deferred.reject(err);
            });

            return deferred.promise;

        };

        authServiceFactory.saveRegistration = _saveRegistration;
        authServiceFactory.login = _login;
        authServiceFactory.logOut = _logOut;
        authServiceFactory.fillAuthData = _fillAuthData;
        authServiceFactory.authentication = _authentication;
        authServiceFactory.refreshToken = _refreshToken;

        authServiceFactory.obtainAccessToken = _obtainAccessToken;
        authServiceFactory.externalAuthData = _externalAuthData;
        authServiceFactory.registerExternal = _registerExternal;

        return authServiceFactory;
    }

    angular.module('app')
    .factory('authService', authService);

    authService.$inject = ['$http', '$q', 'WebStorageService', 'ngAuthSettings', 'jwtHelper', '$log'];

})();