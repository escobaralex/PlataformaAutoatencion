(function () {
    'use strict';
    function CobrosServices($http, $q, $log) {
        function getAll() {
            var deferred = $q.defer();
            var uri = URL_API + 'api/Cobros/GetAll'
            $http({
                url: uri,
                method: "GET"
            })
            .success(function (data, status, headers, config) {
                $log.info('Articulos obtenidos correctamente');
                deferred.resolve(data);
            })
            .error(function (err) {
                $log.error('error al obtener menu');
                deferred.reject(err)
            });

            return deferred.promise;
        };
        function getValorUFMes(mes,ano) {
            var deferred = $q.defer();
            var uri = URL_API + 'api/Cobros/GetValorUFMes'
            var data = {
                mes : mes,
                ano : ano
            };
            switch (data.mes) {
                case "enero":
                    data.mes = 1;
                    break;
                case "febrero":
                    data.mes = 2;
                    break;
                case "marzo":
                    data.mes = 3;
                    break;
                case "abril":
                    data.mes = 4;
                    break;
                case "mayo":
                    data.mes = 5;
                    break;
                case "junio":
                    data.mes = 6;
                    break;
                case "julio":
                    data.mes = 7;
                    break;
                case "agosto":
                    data.mes = 8;
                    break;
                case "septiembre":
                    data.mes = 9;
                    break;
                case "octubre":
                    data.mes = 10;
                    break;
                case "noviembre":
                    data.mes = 11;
                    break;
                case "diciembre":
                    data.mes = 12;
                    break;
            }
            $http.post(uri,data)
            .success(function (data, status, headers, config) {
                $log.info('Articulos obtenidos correctamente');
                deferred.resolve(data);
            })
            .error(function (err) {
                $log.error('error al obtener Valor UF');
                deferred.reject(err)
            });

            return deferred.promise;
        }
        function SaveValorUFMes(ano, mes, valor) {
            var deferred = $q.defer();
            var uri = URL_API + 'api/Cobros/SaveUF'
            var ValorUF = {
                mes: mes,
                ano: ano,
                valor:valor
            };
            switch (ValorUF.mes) {
                case "enero":
                    ValorUF.mes = 1;
                    break;
                case "febrero":
                    ValorUF.mes = 2;
                    break;
                case "marzo":
                    ValorUF.mes = 3;
                    break;
                case "abril":
                    ValorUF.mes = 4;
                    break;
                case "mayo":
                    ValorUF.mes = 5;
                    break;
                case "junio":
                    ValorUF.mes = 6;
                    break;
                case "julio":
                    ValorUF.mes = 7;
                    break;
                case "agosto":
                    ValorUF.mes = 8;
                    break;
                case "septiembre":
                    ValorUF.mes = 9;
                    break;
                case "octubre":
                    ValorUF.mes = 10;
                    break;
                case "noviembre":
                    ValorUF.mes = 11;
                    break;
                case "diciembre":
                    ValorUF.mes = 12;
                    break;
            }
            $http.post(uri, ValorUF)
            .success(function (data, status, headers, config) {
                $log.info('Valor UF guardado correctamente');
                deferred.resolve(data);
            })
            .error(function (err) {
                $log.error('error al guardar Valor UF');
                deferred.reject(err)
            });

            return deferred.promise;
        }
        function UpdValorUFMes(ano, mes, valor) {
            var deferred = $q.defer();
            var uri = URL_API + 'api/Cobros/UpdValorUFMes'
            var ValorUF = {
                mes: mes,
                ano: ano,
                valor: valor
            };
            switch (ValorUF.mes) {
                case "enero":
                    ValorUF.mes = 1;
                    break;
                case "febrero":
                    ValorUF.mes = 2;
                    break;
                case "marzo":
                    ValorUF.mes = 3;
                    break;
                case "abril":
                    ValorUF.mes = 4;
                    break;
                case "mayo":
                    ValorUF.mes = 5;
                    break;
                case "junio":
                    ValorUF.mes = 6;
                    break;
                case "julio":
                    ValorUF.mes = 7;
                    break;
                case "agosto":
                    ValorUF.mes = 8;
                    break;
                case "septiembre":
                    ValorUF.mes = 9;
                    break;
                case "octubre":
                    ValorUF.mes = 10;
                    break;
                case "noviembre":
                    ValorUF.mes = 11;
                    break;
                case "diciembre":
                    ValorUF.mes = 12;
                    break;
            }
            $http.post(uri, ValorUF)
            .success(function (data, status, headers, config) {
                $log.info('Valor UF actualizado correctamente');
                deferred.resolve(data);
            })
            .error(function (err) {
                $log.error('error al actualizar Valor UF');
                deferred.reject(err)
            });

            return deferred.promise;
        }
        /* Obtiene los cobros de la empresa del usuario */
        function GetValoresCobrosEmpresa() {
            var deferred = $q.defer();
            var uri = URL_API + 'api/Cobros/GetValoresCobrosEmpresa'
                     
            $http.get(uri)
            .success(function (data, status, headers, config) {
                $log.info('Valores cobros obtenidos correctamente');
                deferred.resolve(data);
            })
            .error(function (err) {
                $log.error('error al obtener valores cobros');
                deferred.reject(err)
            });

            return deferred.promise;
        }
        function SaveCobrosCliente(valor) {
            var deferred = $q.defer();
            var uri = URL_API + 'api/Cobros/SaveValoresCobrosEmpresa'
            
            $http.post(uri, valor)
            .success(function (data, status, headers, config) {
                $log.info('Valores guardados correctamente');
                deferred.resolve(data);
            })
            .error(function (err) {
                $log.error('error al guardar Valores de cobro.');
                deferred.reject(err)
            });

            return deferred.promise;
        }
        function UpdateCobrosCliente(valor) {
            var deferred = $q.defer();
            var uri = URL_API + 'api/Cobros/UpdateValoresCobrosEmpresa'

            $http.post(uri, valor)
            .success(function (data, status, headers, config) {
                $log.info('Articulos obtenidos correctamente');
                deferred.resolve(data);
            })
            .error(function (err) {
                $log.error('error al obtener Valor UF');
                deferred.reject(err)
            });

            return deferred.promise;
        }

        this.GetValorUFMes = getValorUFMes;
        this.GetAll = getAll;
        this.GetValoresCobros = GetValoresCobrosEmpresa;
        this.SaveCobrosCliente = SaveCobrosCliente;
        this.UpdateCobrosCliente = UpdateCobrosCliente;
        this.SaveValorUFMes = SaveValorUFMes;
        this.UpdValorUFMes = UpdValorUFMes;

        // COBROS ALMACENAMIENTO
        function GetCobroAlmacenamiento(mes, ano) {
            var deferred = $q.defer();
            var uri = URL_API + 'api/Cobros/GetCobroAlmacenamiento'
            
            switch (mes) {
                case "enero":
                    mes = '01';
                    break;
                case "febrero":
                    mes = '02';
                    break;
                case "marzo":
                    mes = '03';
                    break;
                case "abril":
                    mes = '04';
                    break;
                case "mayo":
                    mes = '05';
                    break;
                case "junio":
                    mes = '06';
                    break;
                case "julio":
                    mes = '07';
                    break;
                case "agosto":
                    mes = '08';
                    break;
                case "septiembre":
                    mes = '09';
                    break;
                case "octubre":
                    mes = '10';
                    break;
                case "noviembre":
                    mes = '11';
                    break;
                case "diciembre":
                    mes = '12';
                    break;
            }
            $http.post(uri, {mes : mes, ano : ano})
            .success(function (data, status, headers, config) {
                $log.info('Service Cobros GetCobroAlmacenamiento obtenido correctamente');
                deferred.resolve(data);
            })
            .error(function (err) {
                $log.error('Error Service Cobros GetCobroAlmacenamiento');
                deferred.reject(err)
            });

            return deferred.promise;
        }
        this.GetCobroAlmacenamiento = GetCobroAlmacenamiento;
    }

    angular
	.module('app')
	.service('CobrosServices', CobrosServices);

    CobrosServices.$inject = ['$http', '$q', '$log'];

})();