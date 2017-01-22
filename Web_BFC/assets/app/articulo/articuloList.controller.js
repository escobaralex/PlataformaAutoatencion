(function () {
    'use strict';
    function articuloListController($scope, $log, toaster, $uibModal, i18nService, uiGridConstants, ArticulosServices) {
        $scope.btnDisabled = true;
        i18nService.setCurrentLang('es');
        $scope.animationsEnabled = true;
        $scope.articulos = [];
        $scope.articulo = {};
        $scope.unidadesDeMedida = {};

        $scope.gridVisible = false;
        $scope.loading = false;

        function newModal(op) {
            return $uibModal.open({
                animation: $scope.animationsEnabled,
                templateUrl: 'articulo-detalle.html',
                controller: 'ModalNewUptArticulo',
                //size: '400',
                resolve: {
                    articulo: function () {
                        return $scope.articulo;
                    },
                    operacion: function () {
                        return op;
                    },
                    unidadesDeMedida: function () {
                        return $scope.unidadesDeMedida;
                    },
                    articulos: function () {
                        return $scope.gridOptions.data;
                    }
                }
            });
        }
        function addArticulo(grid, row) {
            $log.info('addArticulo');
            $scope.articulo = {};
            var modalInstanceAdd = $scope.newModal("agregar");

            modalInstanceAdd.result.then(function (ArticuloCreado) {
                ArticulosServices.getAll()
               .then(function (data) {
                   $scope.gridOptions.data = data;
               })
               .catch(function (err) {
                   toaster.pop('error', "Error al obtener articulos", 'Ocurrio un error al obtener los articulos, error: ' + err);
               });
                toaster.pop('success', "Artículo Creado", 'El artículo fue creado correctamente.');
            }, function () {
                $log.info('Modal dismissed at: ' + new Date());
            });
        }
        function verArticulo(grid, row) {
            $scope.articulo = row.entity;

            ArticulosServices.GetDetalleArticulo($scope.articulo)
                .then(function (data,status) {
                    
                    $scope.articulo = data;
                    var modalInstance = newModal("ver");
                    
                })
               .catch(function (err) {
                   toaster.pop('error', "Error al obtener información", 'Ocurrio un error al obtener información detallada del artículo, error: ' + err);
               });
            
        }
        function editarArticulo(grid, row) {
            var modalInstanceEditar = null;
            $scope.articulo = row.entity;

            ArticulosServices.GetDetalleArticulo($scope.articulo)
               .then(function (data, status) {

                   $scope.articulo = data;
                   modalInstanceEditar = $scope.newModal("editar");
                   modalInstanceEditar.result.then(function (resultado) {
                       if (resultado) {
                           ArticulosServices.getAll()
                      .then(function (data) {
                          $scope.gridOptions.data = data;
                      })
                      .catch(function (err) {
                          toaster.pop('error', "Error al obtener información", 'Ocurrio un error al obtener los artículos, error: ' + err);
                      });
                           toaster.pop('success', "Artículo Editado", 'El artículo ' + $scope.articulo.codigo + ' fue modificado correctamente.');
                       } else {
                           toaster.pop('error', "Error al actualizar", 'Ocurrio un error al intentar actualizar el artículos' + err);
                       }
                       
                   }, function () {
                       $log.info('Modal dismissed at: ' + new Date());
                   });

               })
              .catch(function (err) {
                  toaster.pop('error', "Error al obtener información", 'Ocurrio un error al obtener información detallada del artículo, error: ' + err);
              });
        }

        function eliminarArticulo(grid, row) {
            var modalInstanceEliminar = $uibModal.open({
                animation: $scope.animationsEnabled,
                templateUrl: 'articulo-eliminar.html',
                controller: 'ModalEliminarArticulo',
                resolve: {
                    articulo: function () {
                        return row.entity;
                    }
                }
            });
            modalInstanceEliminar.result.then(function (resultado) {
                if (resultado) {
                    toaster.pop('success', "Eliminado", 'Artículo eliminado correctamente');
                  ArticulosServices.getAll()
                      .then(function (data) {
                          $scope.gridOptions.data = data;

                          $scope.btnDisabled = false;
                      })
                      .catch(function (err) {
                          toaster.pop('error', "Error al obtener información", 'Ocurrio un error al obtener los artículos, error: ' + err);
                      });
                } else{
                    toaster.pop('alert', "Alerta", 'El artículo no puede ser eliminado por tener movimientos de inventario');
                }               
            }, function () {
                $log.info('Modal dismissed at: ' + new Date());
            });
        }

        // INICIO UI-GRID
        $scope.gridOptions = {
            enableFiltering: true,
            onRegisterApi: function (gridApi) {
                $scope.gridApi = gridApi;
            },
            paginationPageSizes: [10, 50, 100],
            paginationPageSize: 10,
            
            columnDefs: [
              {field: 'codigo', displayName: 'Código', width: 250  },
              { field: 'descripcion', displayName: 'Descripcion' },
              { field: 'unidadDeMedida', displayName: 'U. de Medida', width: 140, enableFiltering: false },
              //{ field: 'cajaPalet', displayName: 'Caja/Pallet', width: 200 },
              {
                  field: 'id', name: 'Acciones', width: 270,enableFiltering: false, cellTemplate: '<div class="ui-grid-cell-contents">' +
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
            ArticulosServices.getAll()
               .then(function (data) {
                   $scope.gridOptions.data = data;                   
               })
               .catch(function (err) {
                   toaster.pop('error', "Error al obtener información", 'Ocurrio un error al obtener los artículos, error: ' + err);
               });

            ArticulosServices.getUnidadesDeMedida()
              .then(function (data) {
                  $scope.unidadesDeMedida = data;
              })
              .catch(function (err) {
                  toaster.pop('error', "Error al obtener información", 'Ocurrio un error al obtener las Unidades de Medida, error: ' + err);
              });
        }

        $scope.init = init;
        $scope.addArticulo = addArticulo;
        $scope.verArticulo = verArticulo;
        $scope.editarArticulo = editarArticulo;
        $scope.eliminarArticulo = eliminarArticulo;
        $scope.newModal = newModal;
        $scope.init();
    }

    angular
	.module('app')
	.controller('articuloListController', articuloListController);

    articuloListController.$inject = ['$scope', '$log', 'toaster', '$uibModal', 'i18nService', 'uiGridConstants', 'ArticulosServices'];

})();


(function () {
    'use strict';
    function ModalNewUptArticulo($scope, $log, ArticulosServices, $uibModalInstance, articulo, operacion, unidadesDeMedida,articulos) {
        $scope.articulo = angular.copy(articulo);
        $scope.unidadesDeMedida = unidadesDeMedida;
        $scope.isArticuloRepetido = false;
        $scope.aceptarVisible = true;
        $scope.disabled = false;
        $scope.title = '';
        $scope.mensaje = '';
        $scope.articulo.metrosCubicos = 0;
        $scope.ActualizaMtrCubicos = ActualizaMtrCubicos;
        $scope.aceptar = aceptar;
        $scope.cancelar = cancelar;
        $scope.loadArticuloDefault = loadArticuloDefault;
        $scope.validaRepetido = validaRepetido;
        $scope.articulos = articulos;
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
                if (value.codigo == $scope.articulo.unidadDeMedida.toString().trim()) {
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

        function validaRepetido() {
            if (operacion == 'agregar') {
                if ($scope.articulo.codigo !== undefined && $scope.articulo.codigo != '') {
                    $scope.isArticuloRepetido = false;
                    angular.forEach($scope.articulos, function (value, key) {
                        if (value.codigo.toUpperCase() == $scope.articulo.codigo.toUpperCase()) {
                            $scope.isArticuloRepetido = true;
                        }
                    });
                } else {
                    $scope.isArticuloRepetido = false;
                }
            }
        }
    }

    angular
	.module('app')
	.controller('ModalNewUptArticulo', ModalNewUptArticulo);

    ModalNewUptArticulo.$inject = ['$scope', '$log', 'ArticulosServices', '$uibModalInstance', 'articulo', 'operacion', 'unidadesDeMedida', 'articulos'];

})();

(function () {
    'use strict';
    function ModalEliminarArticulo($scope, $uibModalInstance, ArticulosServices, articulo) {
        $scope.articulo = articulo;
        $scope.aceptar = aceptar;
        $scope.cancelar = cancelar;
        function aceptar() {            
            // Verificar que se puede eliminar, sin movimientos de stock
            
            ArticulosServices.VerificaPoderEliminar(articulo)
               .then(function (data) {
                   // Elimina el articulo
                   if (data) {
                       ArticulosServices.Remover(articulo)
                            .then(function (data) {
                                $uibModalInstance.close(true, data);
                            })
                            .catch(function (err) {
                                $uibModalInstance.close(false, err);
                            });
                   } else {
                       $uibModalInstance.close(false, null);
                   }
               })
               .catch(function (err) {
                   $uibModalInstance.close(false, err);
               });
         }
        
        function cancelar() {
            $uibModalInstance.dismiss('cancel');
        }
    }

    angular
	.module('app')
	.controller('ModalEliminarArticulo', ModalEliminarArticulo);

    ModalEliminarArticulo.$inject = ['$scope', '$uibModalInstance', 'ArticulosServices', 'articulo'];

})();