(function () {
    'use strict';
    function EmpresaServices($http, $q, $log) {
        function get() {
            var deferred = $q.defer();
            var uri = URL_API + 'api/empresa/get'
            $http({
                url: uri,
                method: "GET"
            })
            .success(function (data, status, headers, config) {
                $log.info('Empresas obtenidas correctamente');
                deferred.resolve(data);
            })
            .error(function (err) {
                $log.error('error al obtener Empresas');
                deferred.reject(err)
            });

            return deferred.promise;
        };
        function changeEmpresa(id) {
            var deferred = $q.defer();
            var uri = URL_API + 'api/empresa/change'
            var data = { id: id };

            var filter = { id: id };
            $http.post(uri,filter)
            .success(function (data, status, headers, config) {
                $log.info('Empresa cambiada correctamente');
                deferred.resolve(data);
            })
            .error(function (err) {
                $log.error('error al cambiar de empresa');
                deferred.reject(err)
            });

            return deferred.promise;
        };
        function GetDocumentosReferencia() {
            var deferred = $q.defer();
            var uri = URL_API + 'api/empresa/GetDocumentosReferencia'
           
            $http.get(uri)
            .success(function (data, status, headers, config) {
                $log.info('Documentos de referencia obtenidos correctamente');
                deferred.resolve(data);
            })
            .error(function (err) {
                $log.error('error al obtener listado de documentos de referencia');
                deferred.reject(err)
            });

            return deferred.promise;
        }

        this.get = get;
        this.changeEmpresa = changeEmpresa;
        this.GetDocumentosReferencia = GetDocumentosReferencia;
    }

    angular
	.module('app')
	.service('EmpresaServices', EmpresaServices);

    EmpresaServices.$inject = ['$http', '$q', '$log'];

})();