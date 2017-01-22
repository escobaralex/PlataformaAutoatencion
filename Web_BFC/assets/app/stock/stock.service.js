(function () {
    'use strict';
    function StockServices($http, $q, $log) {

        function get(selected, bodega) {
            var deferred = $q.defer();
            var uri = URL_API + 'api/Stock/Post';

            var articulo = {};
            if (selected !== undefined && selected != null) {
                articulo.codigo =  selected.codigo;
            }
            if (bodega == null) {
                bodega = {};
            }
            
            var filter = { articulo: articulo, bodega: bodega };
            $http.post(uri, filter)
           .success(function (data, status, headers, config) {
               $log.info('Informacion de stock obtenida correctamente');
               deferred.resolve(data);
           })
           .error(function (err) {
               $log.error('error al obtener informacion de stock');
               deferred.reject(err)
           });

            return deferred.promise;
        };
        
        function GetEntradasSalidas(selected, bodega,anno) {
            var deferred = $q.defer();
            var uri = URL_API + 'api/Stock/GetEntradasSalidas';

            var articulo = {};
            if (selected !== undefined && selected != null) {
                articulo.codigo = selected.codigo;
            }
            if (bodega == null) {
                bodega = {};
            }

            var filter = { articulo: articulo, bodega: bodega, anno: anno.id };
            $http.post(uri, filter)
           .success(function (data, status, headers, config) {
               $log.info('Informacion de stock obtenida correctamente');
               deferred.resolve(data);
           })
           .error(function (err) {
               $log.error('error al obtener informacion de stock');
               deferred.reject(err)
           });

            return deferred.promise;
        };

        function GetDetallesEntradasSalidas(selected, bodega, anno, mes,saldo) {
            var deferred = $q.defer();
            var uri = URL_API + 'api/Stock/GetDetallesEntradasSalidas';

            var articulo = {};
            if (selected !== undefined && selected != null) {
                articulo.codigo = selected.codigo;
            }
            if (bodega == null) {
                bodega = {
                    codigo:0
                };
            }
	    if (saldo == null) {
                saldo = 0;
            }

            var filter = { articulo: articulo, bodega: bodega, anno: anno.id ,mes:mes,saldo:saldo};
            $http.post(uri, filter)
           .success(function (data, status, headers, config) {
               $log.info('Informacion de stock obtenida correctamente');
               deferred.resolve(data);
           })
           .error(function (err) {
               $log.error('error al obtener informacion de stock');
               deferred.reject(err)
           });

            return deferred.promise;
        };

        function GetListadoEntradasSalidas(selected, desde, hasta) {
            var deferred = $q.defer();
            var uri = URL_API + 'api/Stock/GetListadoEntradasSalidas';

            var articulo = {};
            if (selected !== undefined && selected != null) {
                articulo.codigo = selected.codigo;
            }

            var filter = { articulo: articulo, desde: desde, hasta: hasta };
            $http.post(uri, filter)
           .success(function (data, status, headers, config) {
               $log.info('Informacion Listado de entradas y salidas de stock obtenida correctamente');
               deferred.resolve(data);
           })
           .error(function (err) {
               $log.error('error al obtener informacion Listado de entradas y salidas de stock');
               deferred.reject(err)
           });

            return deferred.promise;
        };

        function getBodegas() {
            var deferred = $q.defer();
            var uri = URL_API + 'api/Bodega/Get';
            
            $http.get(uri, {})
           .success(function (data, status, headers, config) {
               $log.info('Bodegas obtenidas correctamente');
               deferred.resolve(data);
           })
           .error(function (err) {
               $log.error('error al obtener bodegas');
               deferred.reject(err)
           });

            return deferred.promise;
        };

        function CreateMovimientoExistencia(movimiento) {
            var deferred = $q.defer();
            var uri = URL_API + 'api/Stock/CreateMovimientoExistencia';

            $http.post(uri, movimiento)
           .success(function (data, status, headers, config) {
               $log.info('Movimiento de existencia registrado correctamente');
               deferred.resolve(data);
           })
           .error(function (err) {
               $log.error('error al registrar movimiento de existencia');
               deferred.reject(err)
           });

            return deferred.promise;
        }
        
        function UpdateMovimientoExistencia(movimiento) {
            var deferred = $q.defer();
            var uri = URL_API + 'api/Stock/UpdateMovimientoExistencia';

            $http.post(uri, movimiento)
           .success(function (data, status, headers, config) {
               $log.info('Movimiento de existencia actualizado correctamente');
               deferred.resolve(data);
           })
           .error(function (err) {
               $log.error('error al actualizar movimiento de existencia');
               deferred.reject(err)
           });

            return deferred.promise;
        }

        function GetDetalleMovimientoExistencia(movimiento){
            var deferred = $q.defer();
            var uri = URL_API + 'api/Stock/GetDetalleMovimientoExistencia';

            $http.post(uri,movimiento)
           .success(function (data, status, headers, config) {
               $log.info('Movimiento de existencia obtenidos correctamente');
               deferred.resolve(data);
           })
           .error(function (err) {
               $log.error('error al intentar obtener movimiento de existencia');
               deferred.reject(err)
           });

            return deferred.promise;
        }

        function GetMovimientoExistencia(movimiento) {
            var deferred = $q.defer();
            var uri = URL_API + 'api/Stock/GetMovimientoExistencia';

            $http.post(uri, movimiento)
           .success(function (data, status, headers, config) {
               $log.info('Movimiento de existencia obtenidos correctamente');
               deferred.resolve(data);
           })
           .error(function (err) {
               $log.error('error al intentar obtener movimiento de existencia');
               deferred.reject(err)
           });

            return deferred.promise;
        }

        function GetAllMovimientoExistencia() {
            var deferred = $q.defer();
            var uri = URL_API + 'api/Stock/GetAllMovimientoExistencia';

            $http.get(uri)
           .success(function (data, status, headers, config) {
               $log.info('Movimientos de existencia obtenidos correctamente');
               deferred.resolve(data);
           })
           .error(function (err) {
               $log.error('error al intentar obtener movimientos de existencias');
               deferred.reject(err)
           });

            return deferred.promise;
        }

        function DeleteMovimientoExistencia(movimiento) {
            var deferred = $q.defer();
            var uri = URL_API + 'api/Stock/DeleteMovimientoExistencia';

            $http.post(uri, movimiento)
           .success(function (data, status, headers, config) {
               $log.info('Movimiento de existencia eliminado correctamente');
               deferred.resolve(data);
           })
           .error(function (err) {
               $log.error('error al eliminar movimiento de existencia');
               deferred.reject(err)
           });

            return deferred.promise;
        }

        this.get = get;
        this.GetEntradasSalidas = GetEntradasSalidas;
        this.GetDetallesEntradasSalidas = GetDetallesEntradasSalidas;
        this.GetListadoEntradasSalidas = GetListadoEntradasSalidas;
        this.getBodegas = getBodegas;
        this.CreateMovimientoExistencia = CreateMovimientoExistencia;
        this.GetMovimientoExistencia = GetMovimientoExistencia;
        this.GetDetalleMovimientoExistencia = GetDetalleMovimientoExistencia;
        this.GetAllMovimientoExistencia = GetAllMovimientoExistencia;
        this.DeleteMovimientoExistencia = DeleteMovimientoExistencia;
        this.UpdateMovimientoExistencia = UpdateMovimientoExistencia;
    }

    angular
	.module('app')
	.service('StockServices', StockServices);

    StockServices.$inject = ['$http', '$q', '$log'];

})();