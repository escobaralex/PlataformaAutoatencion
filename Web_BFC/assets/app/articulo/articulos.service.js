(function () {
    'use strict';
    function ArticulosServices($http, $q, $log) {
        
        function get() {            
            var deferred = $q.defer();
            var uri = URL_API + 'api/Articulo/get'
            $http.get(uri, {})
           .success(function (data, status, headers, config) {
               $log.info('Articulos obtenidos correctamente');
               deferred.resolve(data);
           })
           .error(function (err) {
               $log.error('error al intentar obtener los articulos');
               deferred.reject(err)
           });
            return deferred.promise;
        };
        function GetDetalleArticulo(articulo) {            
            var deferred = $q.defer();
            var uri = URL_API + 'api/Articulo/GetDetalleArticulo'
            $http.post(uri, articulo)
           .success(function (data, status, headers, config) {
               $log.info('Articulos obtenidos correctamente');
               deferred.resolve(data, status);
           })
           .error(function (err) {
               $log.error('error al intentar obtener los articulos');
               deferred.reject(err)
           });
            return deferred.promise;
        };
        function getAll() {
            var deferred = $q.defer();
            var uri = URL_API + 'api/Articulo/getAll'
            $http.get(uri, {})
           .success(function (data, status, headers, config) {
               $log.info('Articulos obtenidos correctamente');
               deferred.resolve(data);
           })
           .error(function (err) {
               $log.error('error al intentar obtener los articulos');
               deferred.reject(err)
           });
            return deferred.promise;
        };
        function getUnidadesDeMedida() {
            var deferred = $q.defer();
            var uri = URL_API + 'api/Articulo/GetUnidadesDeMedida'
            $http.get(uri, {})
           .success(function (data, status, headers, config) {
               $log.info('Unidades de Medida obtenidas correctamente');
               deferred.resolve(data);
           })
           .error(function (err) {
               $log.error('error al intentar obtener Unidades de Medida');
               deferred.reject(err)
           });
            return deferred.promise;
        };
        function getByCodigoDescripcion(param) {
            var deferred = $q.defer();
            var uri = URL_API + 'api/Articulo/getByCodigoDescripcion'
            var articulo = { descripcion : param };
            $http.post(uri, articulo)
           .success(function (data, status, headers, config) {
               $log.info('Articulos obtenidos correctamente');
               deferred.resolve(data);
           })
           .error(function (err) {
               $log.error('error al obtener articulos');
               deferred.reject(err)
           });
            return deferred.promise;
        };
        function getById(id) {
            var deferred = $q.defer();
            var uri = URL_API + 'api/Articulos/get'
            $http({
                data: {id : id },
                url: uri,
                method: "GET"
            })
            .success(function (data, status, headers, config) {
                $log.info('Articulos obtenidos desde servidor correctamente');
                deferred.resolve(data);
            })
            .error(function (err) {
                $log.error('error llamada API al obtener articulos');
                deferred.reject(err)
            });

            return deferred.promise;
        };
        
        function Create(articulo) {
            var deferred = $q.defer();
            var uri = URL_API + 'api/Articulo/Create'
            $http.post(uri,articulo)
            .success(function (data, status, headers, config) {
                $log.info('Articulos removido correctamente');
                deferred.resolve(data);
            })
            .error(function (err) {
                $log.error('error al intentar crear el articulo, error: ' + (JSON.stringify(err) || 'no especificado') );
                deferred.reject(err)
            });

            return deferred.promise;
        };
        function Remover(articulo) {
            var deferred = $q.defer();
            var uri = URL_API + 'api/Articulo/Remover'
            $http({
                data: articulo,
                url: uri,
                method: "POST"
            })
            .success(function (data, status, headers, config) {
                $log.info('Articulos removido correctamente');
                deferred.resolve(data);
            })
            .error(function (err) {
                $log.error('error al intentar remover el articulo');
                deferred.reject(err)
            });

            return deferred.promise;
        };
        function Update(articulo) {
            var deferred = $q.defer();
            var uri = URL_API + 'api/Articulo/update'
            $http.post(uri, articulo)
            .success(function (data, status, headers, config) {
                $log.info('Articulo actualizado correctamente');
                deferred.resolve(data);
            })
            .error(function (err) {
                $log.error('error al actualizar articulo');
                deferred.reject(err)
            });

            return deferred.promise;
        };

        function VerificaPoderEliminar(articulo) {
            var deferred = $q.defer();
            var uri = URL_API + 'api/Articulo/VerificarPoderEliminar'
            $http({
                data: articulo,
                url: uri,
                method: "POST"
            })
            .success(function (data, status, headers, config) {
                $log.info('El articulo puede ser eliminado');
                deferred.resolve(data);
            })
            .error(function (err) {
                $log.error('error llamada API al obtener articulos');
                deferred.reject(err)
            });

            return deferred.promise;
        };
        this.get = get;
        this.GetDetalleArticulo = GetDetalleArticulo;
        this.getAll = getAll;
        this.getByCodigoDescripcion = getByCodigoDescripcion;
        this.getById = getById;
        this.Update = Update;
        this.Create = Create;
        this.Remover = Remover;
        this.VerificaPoderEliminar = VerificaPoderEliminar;
        this.getUnidadesDeMedida = getUnidadesDeMedida;
    }

    angular
	.module('app')
	.service('ArticulosServices', ArticulosServices);

    ArticulosServices.$inject = ['$http', '$q', '$log'];

})();
