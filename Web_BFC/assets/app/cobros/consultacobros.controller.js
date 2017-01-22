(function () {
    'use strict';
    function ConsultaCobrosController($scope, $log, toaster, $uibModal, i18nService, uiGridConstants, CobrosServices) {
        i18nService.setCurrentLang('es');
        // INICIO UI-GRID
        $scope.gridOptions = {
            enableFiltering: true,
            onRegisterApi: function (gridApi) {
                $scope.gridApi = gridApi;
            },
            paginationPageSizes: [10, 50, 100],
            paginationPageSize: 10,

            columnDefs: [
              { field: 'codigo', displayName: 'Código', width: 250 },
              { field: 'descripcion', displayName: 'Descripcion' },
              { field: 'unidadDeMedida', displayName: 'U. de Medida', width: 140, enableFiltering: false },
              //{ field: 'cajaPalet', displayName: 'Caja/Pallet', width: 200 },
              {
                  field: 'id', name: 'Acciones', width: 270, enableFiltering: false, cellTemplate: '<div class="ui-grid-cell-contents">' +
                      // Boton VER
                  '<button type="button" class="btn btn-xs btn-info" style="width:80px"' +
                  'ng-click="grid.appScope.verArticulo(grid, row)"><i class="fa fa-eye"></i>&nbsp;&nbsp;Ver</button>' +
                      // Boton EDITAR
                  '&nbsp;&nbsp;<button type="button" class="btn btn-xs btn-primary"  style="width:80px"' +
                  'ng-click="grid.appScope.editarArticulo(grid, row)"><i class="fa fa-edit"></i>&nbsp;&nbsp;Editar</button>' +
                      // Boton ELIMINAR
                  '&nbsp;&nbsp;<button type="button" class="btn btn-xs btn-danger"  style="width:80px"' +
                  'ng-click="grid.appScope.eliminarArticulo(grid, row)"><i class="fa fa-trash-o"></i>&nbsp;&nbsp;Eliminar</button></div>'
              }
            ]
        };

        function init() {
            CobrosServices.GetAll()
               .then(function (data) {
                   $scope.gridOptions.data = data;
               })
               .catch(function (err) {
                   toaster.pop('error', "Error al obtener información", 'Ocurrio un error al obtener los cobros, error: ' + (JSON.stringify(err) || ''));
               });
        }

        $scope.init = init;

        $scope.init();
    }

    angular
	.module('app')
	.controller('ConsultaCobrosController', ConsultaCobrosController);

    ConsultaCobrosController.$inject = ['$scope', '$log', 'toaster', '$uibModal', 'i18nService', 'uiGridConstants', 'CobrosServices'];

})();

(function () {
    'use strict';
    function ModalDetalleCobro($scope, $log, CobrosServices, $uibModalInstance, articulo, operacion, unidadesDeMedida) {
        $scope.articulo = angular.copy(articulo);
        $scope.unidadesDeMedida = unidadesDeMedida;

        $scope.aceptarVisible = true;
        $scope.disabled = false;
        $scope.title = '';
        $scope.mensaje = '';
        $scope.articulo.metrosCubicos = 0;
        $scope.ActualizaMtrCubicos = ActualizaMtrCubicos;
        $scope.aceptar = aceptar;
        $scope.cancelar = cancelar;
        $scope.loadArticuloDefault = loadArticuloDefault;
        $scope.reqContr = false;
        // Reemplazo el submited del form por no tener acceso a el
        $scope.formCrtl = false;


        if (operacion == 'ver') {
            $scope.disabled = true;
            $scope.aceptarVisible = false;
            $scope.title = 'Ver artículo';

        } else if (operacion == 'agregar') {
            // A partir del codigo en string de la U.medida pasar a objeto
            angular.forEach($scope.unidadesDeMedida, function (value, key) {
                if (value.codigo == $scope.articulo.unidadDeMedida) {
                    $scope.articulo.unidadDeMedida = value;
                }
            });
            $scope.title = 'Crear un nuevo artículo';
            $scope.loadArticuloDefault();
            $scope.reqContr = true;
        } else {
            // A partir del codigo en string de la U.medida pasar a objeto
            angular.forEach($scope.unidadesDeMedida, function (value, key) {
                if (value.codigo == $scope.articulo.unidadDeMedida.trim()) {
                    $scope.articulo.unidadDeMedida = value;
                }
            });
            $scope.codigoDisabled = true;
            $scope.title = 'Editar artículo';
        }

        $scope.$watch('articulo.largo', function () {
            $log.debug('Largo modificado');
            $scope.ActualizaMtrCubicos();
        }, true);
        $scope.$watch('articulo.alto', function () {
            $log.debug('alto modificado');
            $scope.ActualizaMtrCubicos();
        }, true);
        $scope.$watch('articulo.ancho', function () {
            $log.debug('ancho modificado');
            $scope.ActualizaMtrCubicos();
        }, true);

        function aceptar(form) {
            $scope.mensaje = '';
            if (operacion == 'ver') {
                alert('Operación Invalida');
            } else {
                // VALIDACIONES
                if (form) {
                    var copyUsuario = angular.copy($scope.articulo);
                    copyUsuario.unidadDeMedida = $scope.articulo.unidadDeMedida.codigo;

                    if (operacion == "agregar") {
                        ArticulosServices.Create(copyUsuario)
                        .then(function (data) {
                            $uibModalInstance.close(data);
                        })
                        .catch(function (err) {
                            $scope.mensaje = 'Ocurrio un error al intentar crear el articulo, error: ' + (JSON.stringify(err) || '');
                        });
                    } else if (operacion == "editar") {
                        ArticulosServices.Update(copyUsuario)
                        .then(function (data) {
                            $uibModalInstance.close(data);
                        })
                        .catch(function (err) {
                            $scope.mensaje = 'Ocurrio un error al intentar editar el articulo, error: ' + (JSON.stringify(err) || '');
                        });
                    }
                } else {
                    $scope.formCrtl = true;
                }
            }
        }
        function cancelar() {
            $uibModalInstance.dismiss('cancel');
        }
        function loadArticuloDefault() {
            $scope.articulo = {
                "codigo": "",
                "descripcion": "",
                "unidadDeMedida": undefined,
                "uniXcaja": "",
                "cXpallet": "",
                "largo": "",
                "alto": "",
                "ancho": "",
                "metrosCubicos": 0,
                "isActivo": true
            };
        }
        function ActualizaMtrCubicos() {
            $scope.articulo.metrosCubicos = ($scope.articulo.largo * $scope.articulo.ancho * $scope.articulo.alto * 0.000001).toFixed(5);
        }
    }

    angular
	.module('app')
	.controller('ModalDetalleCobro', ModalDetalleCobro);

    ModalDetalleCobro.$inject = ['$scope', '$log', 'CobrosServices', '$uibModalInstance', 'articulo', 'operacion', 'unidadesDeMedida'];

})();