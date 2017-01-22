(function () {
    'use strict';
    function StockEntradasSalidasController($scope, toaster,$uibModal, StockServices, ArticulosServices) {
        
        $scope.informe = {};
        $scope.bodegas = [];
        $scope.bodega = {};
        $scope.articulo = {};
        $scope.articulos = [];
        $scope.annos = [];

        $scope.formDisable = false
        $scope.tableVisible = false;
        $scope.loading = false;

        function generarInforme() {
            $scope.form.$submitted = true;
            if ($scope.form.$valid) {
                $scope.loading = true;
                $scope.tableVisible = true;
                StockServices.GetEntradasSalidas($scope.articulo.selected, $scope.bodega,$scope.anno)
                .then(function (data) {
                    if (data != "") {
                        $scope.informe = data;
                        $scope.formDisable = true;
                        toaster.pop('success', "Generado", 'Informe generado correctamente');
                        $scope.tableVisible = true;
                        
                    } else {
                        toaster.pop('info', "Sin información", 'No se ha encontrado información para el artículo.');
                        $scope.tableVisible = false;
                    }
                    $scope.loading = false;
                })
                .catch(function (err) {
                    toaster.pop('error', "Error al obtener información", 'Ocurrio un error al obtener la información, error: ' + err);
                    $scope.tableVisible = false;
                    $scope.loading = false;
                });
            }
        }
        function refreshArticulo(search) {
            ArticulosServices.getByCodigoDescripcion(search)
               .then(function (data) {
                   $scope.articulos = data;
                   if ($scope.articulos == null) {
                       toaster.pop('alert', "Sin resultados", 'El artículo no se ha encontrado, modifique y vuelva a intentar');
                   }
               })
               .catch(function (err) {
                   toaster.pop('error', "Error al obtener información", 'Ocurrio un error al obtener los articulos, error: ' + err);

               });
        };
        function limpiarArticuloSelected() {
            $scope.articulo = {};
        }
        function verDetalles(mes) {
            var conMovimientos = false;
            var saldo = 0;
            switch (mes) {
                case 1:
                    if ($scope.informe.enero[0] > 0 || $scope.informe.enero[1] > 0) {
                        conMovimientos = true;
                        saldo = $scope.informe.stockInicial;
                    }
                    break;
                case 2:
                    if ($scope.informe.febrero[0] > 0 || $scope.informe.febrero[1] > 0) {
                        conMovimientos = true;
                        saldo = $scope.informe.enero[2];
                    }
                    break;
                case 3:
                    if ($scope.informe.marzo[0] > 0 || $scope.informe.marzo[1] > 0 ) {
                        conMovimientos = true;
                        saldo = $scope.informe.febrero[2];
                    }
                    break;
                case 4:
                    if ($scope.informe.abril[0] > 0 || $scope.informe.abril[1] > 0) {
                        conMovimientos = true;
                        saldo = $scope.informe.marzo[2];
                    }
                    break;
                case 5:
                    if ($scope.informe.mayo[0] > 0 || $scope.informe.mayo[1] > 0) {
                        conMovimientos = true;
                        saldo = $scope.informe.abril[2];
                    }
                    break;
                case 6:
                    if ($scope.informe.junio[0] > 0 || $scope.informe.junio[1] > 0) {
                        conMovimientos = true;
                        saldo = $scope.informe.mayo[2];
                    }
                    break;
                case 7:
                    if ($scope.informe.julio[0] > 0 || $scope.informe.julio[1] > 0) {
                        conMovimientos = true;
                        saldo = $scope.informe.junio[2];
                    }
                    break;
                case 8:
                    if ($scope.informe.agosto[0] > 0 || $scope.informe.agosto[1] > 0) {
                        conMovimientos = true;
                        saldo = $scope.informe.julio[2];
                    }
                    break;
                case 9:
                    if ($scope.informe.septiembre[0] > 0 || $scope.informe.septiembre[1] > 0) {
                        conMovimientos = true;
                        saldo = $scope.informe.agosto[2];
                    }
                    break;
                case 10:
                    if ($scope.informe.octubre[0] > 0 || $scope.informe.octubre[1] > 0) {
                        conMovimientos = true;
                        saldo = $scope.informe.septiembre[2];
                    }
                    break;
                case 11:
                    if ($scope.informe.noviembre[0] > 0 || $scope.informe.noviembre[1] > 0) {
                        conMovimientos = true;
                        saldo = $scope.informe.octubre[2];
                    }
                    break;
                case 12:
                    if ($scope.informe.diciembre[0] > 0 || $scope.informe.diciembre[1] > 0) {
                        conMovimientos = true;
                        saldo = $scope.informe.noviembre[2];
                    }
                    break;
            }
            if (conMovimientos) {
                StockServices.GetDetallesEntradasSalidas($scope.articulo.selected, $scope.bodega, $scope.anno, mes, saldo)
                .then(function (data) {
                    // Abrir modal
                    $uibModal.open({
                        animation: $scope.animationsEnabled,
                        templateUrl: 'detalle-entradassalidas.html',
                        controller: 'ModalDetalleEntradaSalidaController',
                        size: 'lg',
                        resolve: {
                            detalles: function () {
                                return data;
                            },
                            articulo: function () {
                                return $scope.articulo.selected;
                            },
                            saldoInicial: function () {
                                return saldo;
                            }
                        }
                    });
                })
                .catch(function (err) {
                    toaster.pop('error', "Error al obtener Información", 'Ocurrio un error al obtener los detalles de movientos');
                });
            }
        }
        function informeReset() {
            $scope.informe = {
                enero: [0, 0, 0],
                febrero: [0, 0, 0],
                marzo: [0, 0, 0],
                abril: [0, 0, 0],
                mayo: [0, 0, 0],
                junio: [0, 0, 0],
                julio: [0, 0, 0],
                agosto: [0, 0, 0],
                septiembre: [0, 0, 0],
                octubre: [0, 0, 0],
                noviembre: [0, 0, 0],
                diciembre: [0, 0, 0],
            };
        }
        function clear() {
            $scope.limpiarArticuloSelected();
            $scope.informeReset();
            $scope.tableVisible = false;
            $scope.formDisable = false;
        }
        function init() {
            StockServices.getBodegas()
                .then(function (data) {
                    $scope.bodegas = data;
                })
                .catch(function (err) {
                    toaster.pop('error', "Error al obtener bodegas", 'Ocurrio un error al obtener las bodegas, error: ' + err);
                });
            //TODO: Obtener años desde que se cre la empresa y cargar combo de años
            $scope.annos = [{
                id: 2016
            }];
            $scope.informeReset();
        }

        $scope.generarInforme = generarInforme;
        $scope.refreshArticulo = refreshArticulo;
        $scope.limpiarArticuloSelected = limpiarArticuloSelected;
        $scope.verDetalles = verDetalles;
        $scope.clear = clear;
        $scope.informeReset = informeReset;
        $scope.init = init;
       
        $scope.init();
    }

    angular
	.module('app')
	.controller('StockEntradasSalidasController', StockEntradasSalidasController);

    StockEntradasSalidasController.$inject = ['$scope', 'toaster', '$uibModal', 'StockServices', 'ArticulosServices'];

})();

(function () {
    'use strict';
    function ModalDetalleEntradaSalidaController($scope, $log, $uibModalInstance, detalles, articulo, saldoInicial) {
        $scope.informe = angular.copy(detalles);
        $scope.articulo = articulo;
        $scope.saldo = saldoInicial;
       
        $scope.title = 'Detalle Tarjeta de Existencia';
        $scope.mensaje = '';        
        
        $scope.cancelar = cancelar;
        
        function cancelar() {
            $uibModalInstance.dismiss('cancel');
        }        
    }

    angular
	.module('app')
	.controller('ModalDetalleEntradaSalidaController', ModalDetalleEntradaSalidaController);

    ModalDetalleEntradaSalidaController.$inject = ['$scope', '$log', '$uibModalInstance', 'detalles', 'articulo', 'saldoInicial'];

})();