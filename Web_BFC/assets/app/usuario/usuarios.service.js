(function () {
    'use strict';
    function UsuariosServices($http, $q, $log) {
        function ObtenerAll() {
            var deferred = $q.defer();
            var uri = URL_API + 'api/Usuario/ObtenerAll'
            $http({
                url: uri,
                method: "GET"
            })
            .success(function (data, status, headers, config) {
                $log.info('Usuarios obtenidos correctamente');
                deferred.resolve(data);
            })
            .error(function (err) {
                $log.error('error al obtener Usuarios');
                deferred.reject(err)
            });

            return deferred.promise;
        };
        function GetRoles() {
            var deferred = $q.defer();
            var uri = URL_API + 'api/roles'
            $http({
                url: uri,
                method: "GET"
            })
            .success(function (data, status, headers, config) {
                $log.info('Roles obtenidos correctamente');
                deferred.resolve(data);
            })
            .error(function (err) {
                $log.error('error al obtener Roles');
                deferred.reject(err)
            });

            return deferred.promise;
        };
        function GetFormasCobro() {
            var deferred = $q.defer();
            var uri = URL_API + 'api/empresa/formascobro'
            $http({
                url: uri,
                method: "GET"
            })
            .success(function (data, status, headers, config) {
                $log.info('Roles obtenidos correctamente');
                deferred.resolve(data);
            })
            .error(function (err) {
                $log.error('error al obtener Roles');
                deferred.reject(err)
            });

            return deferred.promise;
        };
        function Remover(usuario) {
            var deferred = $q.defer();
            var uri = URL_API + 'api/Usuario/remover'
            $http({
                url: uri,
                method: "POST",
                data:usuario
            })
            .success(function (data, status, headers, config) {
                $log.info('Usuario eliminado y usuarios obtenidos correctamente');
                deferred.resolve(data);
            })
            .error(function (err) {
                $log.error('error al intentar eliminar usuario');
                deferred.reject(err)
            });

            return deferred.promise;
        };
        function Create(usuario) {
            var deferred = $q.defer();
            var uri = URL_API + 'api/Usuario/Create'
            $http({
                url: uri,
                method: "POST",
                data: usuario
            })
            .success(function (data, status, headers, config) {
                $log.info('Usuario creado correctamente');
                deferred.resolve(data);
            })
            .error(function (err) {
                $log.error('error al intentar crear usuario');
                deferred.reject(err)
            });

            return deferred.promise;
        };
        function Update(usuario) {
            var deferred = $q.defer();
            var uri = URL_API + 'api/Usuario/Update'
            $http({
                url: uri,
                method: "POST",
                data: usuario
            })
            .success(function (data, status, headers, config) {
                $log.info('Usuario creado correctamente');
                deferred.resolve(data);
            })
            .error(function (err) {
                $log.error('error al intentar crear usuario');
                deferred.reject(err)
            });

            return deferred.promise;
        };

        this.ObtenerAll = ObtenerAll;
        this.Remover = Remover;
        this.GetRoles = GetRoles;
        this.GetFormasCobro = GetFormasCobro;
        this.Create = Create;
        this.Update = Update;
    }

    angular
	.module('app')
	.service('UsuariosServices', UsuariosServices);

    UsuariosServices.$inject = ['$http', '$q', '$log'];

})();