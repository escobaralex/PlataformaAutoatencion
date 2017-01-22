(function () {
    'use strict';
    
    function stockMovimientoController($scope, $filter, $timeout, $log, toaster, $uibModal, StockServices, ArticulosServices, $state, EmpresaServices) {
        moment.locale('es');
        moment().format('L');
        $scope.optionMovimiento = [{nombre:"Ingreso",id:1}, {nombre:"Salida",id:2}];
        $scope.docExi = {
            encabezado: {
                fechaEmision: new Date(),
                nroReferencia : ''
            },
            detalles: []            
        };
        moment().format('L');
        $scope.bodegas = [];
        $scope.bodega = {};
        $scope.articulo = {};
        $scope.articulos = [];
        $scope.documentosReferencia = [];
        $scope.frmdisabled = false;
        $scope.selectIsDisable = false;
        $scope.selectIsHide = false;
        $scope.loadingStock = false;
        $scope.loadingGuardar = false;
        $scope.loadingEliminar = false;
        $scope.loadingLimpiar = false;
        $scope.loadingImprimir = false;
        $scope.isEditandoDetalle = false;
        $scope.isIngresandoNew = true;
        $scope.disableAgregar = false;
        $scope.isEditandoMovimiento = false;

        $scope.formDetalleSubmited = false;
        $scope.newDetalle = {
            cantidad: '',
            valorUnitario: '',
            total:0
        };

        $scope.isFocus = false;
                      
        function refreshArticulo(search) {
            ArticulosServices.getByCodigoDescripcion(search)
               .then(function (data) {

                   $scope.articulos = data;
                   if ($scope.articulos == null) {
                       toaster.pop('warning', "Sin resultados", 'El artículo no se ha encontrado, modifique y vuelva a intentar');
                   }
               })
               .catch(function (err) {
                   toaster.pop('error', "Error al obtener información", 'Ocurrio un error al obtener los articulos, error: ' + err);

               });
        };
        function buscarMovimientoExistencia() {
            var modalInstanceEditar = $uibModal.open({
                animation: $scope.animationsEnabled,
                templateUrl: 'buscar-movimiento.html',
                controller: 'ModalBuscarMovimiento',
                size: 'lg',
                resolve: {
                    documentosReferencia: function () {
                        return $scope.documentosReferencia;
                    },
                    bodegas: function () {
                        return $scope.bodegas;
                    }
                }
            });
            modalInstanceEditar.result.then(function (movimiento) {
                angular.forEach($scope.bodegas, function (value, key) {
                    if (value.codigo == movimiento.encabezado.bodega.codigo) {
                        movimiento.encabezado.bodega.descripcion = value.descripcion;
                    }
                });
                if (movimiento.encabezado.referencia != null && movimiento.encabezado.referencia.id > 0) {
                    if (movimiento.encabezado.referencia.codigoDocumento !== undefined) {
                        $scope.docExi.encabezado.referencia = movimiento.encabezado.referencia;
                    } else {
                        angular.forEach($scope.documentosReferencia, function (value, key) {
                            if (value.id == movimiento.encabezado.referencia.id) {
                                movimiento.encabezado.referencia.codigoDocumento = value.codigoDocumento;
                                movimiento.encabezado.referencia.descripcion = value.descripcion;
                                movimiento.encabezado.referencia.id = value.id;
                                //$scope.docExi.encabezado.referencia = movimiento.encabezado.referencia;
                            }
                        });
                    }
                }
                $scope.docExi.encabezado = {
                    "fechaEmision": movimiento.encabezado.fechaEmision,
                    //"movimiento": movimiento.encabezado.movimiento,
                    "observacion": movimiento.encabezado.observacion,
                    "bodega": {
                        "codigo": movimiento.encabezado.bodega.codigo,
                        "descripcion": movimiento.encabezado.bodega.descripcion,
                    },
                    nroDocumento : movimiento.encabezado.nroDocumento
                };
                $scope.docExi.detalles = [];
                $scope.isEditandoMovimiento = true;
                $scope.docExi.encabezado.movimiento = movimiento.encabezado.movimiento;
                if (movimiento.encabezado.referencia != null && movimiento.encabezado.referencia.id>0) {
                    
                    $scope.docExi.encabezado.referencia = {
                        "id": movimiento.encabezado.referencia.id,
                        "codigoDocumento": movimiento.encabezado.referencia.codigoDocumento,
                        "descripcion": movimiento.encabezado.referencia.descripcion
                    };
                    
                    $scope.docExi.encabezado.nroReferencia = movimiento.encabezado.nroReferencia;
                }
                var cont = 1;
                angular.forEach(movimiento.detalles, function (value, key) {
                    var detalle = {
                        "cantidad": value.cantidad,
                        "valorUnitario": value.valorUnitario,
                        "total": Math.round(value.cantidad * value.valorUnitario),
                        "codigo": value.codigo,
                        "descripcion": value.descripcion,
                        "selected": {
                            "codigo": value.codigo,
                            "descripcion": value.descripcion,
                            "unidadDeMedida": null,
                            "uniXcaja": 0,
                            "cXpallet": 0,
                            "largo": 0,
                            "alto": 0,
                            "ancho": 0,
                            "isActivo": false,
                            "stock": null
                        },
                        "correlativo": cont
                    };
                    cont++;
                    $scope.docExi.detalles.push(detalle);

                });

                $scope.isEditandoMovimiento = true;
                $scope.isIngresandoNew = false;
            }, function () {
                $log.info('Modal dismissed at: ' + new Date());
            });
        };
        function obtenerStock() {
            //if ($scope.docExi.encabezado == null || $scope.docExi.encabezado === undefined) {
            //    toaster.pop('warning', "Alerta", 'Debe completar el encabezado del documento');            
            //} 
            //else 
            //if (
            //    $scope.docExi.encabezado.bodega == null || $scope.docExi.encabezado.bodega == null || $scope.docExi.encabezado.bodega === undefined ||
            //    $scope.docExi.encabezado.bodega == null || $scope.docExi.encabezado.bodega == null || $scope.docExi.encabezado.bodega === undefined
            //    )
            if(!$scope.form.$valid){
                $scope.formCrtl = true;
                toaster.pop('warning', "Alerta", 'Debe completar el encabezado del documento');
            } else if ($scope.articulo == null || $scope.articulo == undefined || $scope.articulo.selected == null || $scope.articulo.selected === undefined) {
                toaster.pop('warning', "Alerta", 'Debe seleccionar un artículo');
            }
            else {
                $scope.loadingStock = true;
                StockServices.get($scope.articulo.selected, $scope.docExi.encabezado.bodega)
                .then(function (data) {
                    $scope.newDetalle.stock = data[0].stock;
                    toaster.pop('success', 'Información obtenida', 'Stock del artículo ' + $scope.articulo.selected.codigo + ' obtenido correctamente');
                    $scope.loadingStock = false;
                })
                .catch(function (err) {
                    toaster.pop('error', "Error al obtener información", 'Ocurrio un error al obtener el stock del artículo, error: ' + (JSON.stringify(err) || ''));
                    $scope.loadingStock = false;
                });
            }
        }
        function validaCero($event,campo) {
            switch (campo) {
                case "cantidad":
                    if ($scope.newDetalle.cantidad == 0) {
                        $scope.newDetalle.cantidad = '';
                    }
                    break;
                case "valorUnitario":
                    if ($scope.newDetalle.valorUnitario == 0) {
                        $scope.newDetalle.valorUnitario = '';
                    }
                    break;
            }            
        }

        // Funciones Botones del Detalle
        function detalleBorrar() {
            $scope.docExi.detalles.splice($scope.newDetalle.correlativo-1, 1);
            var corr = 1;
            angular.forEach($scope.docExi.detalles, function (value, key) {
                value.correlativo = corr;
                corr++;
            }, null);
            $scope.detalleLimpiar();
        }
        function detalleLimpiar() {
            $scope.newDetalle = {
                cantidad: '',
                valorUnitario: '',
                total: 0,
                correlativo : null,
                selected : null
            };
            $scope.disableAgregar = false;
            $scope.selectIsDisable = false;
            $scope.isEditandoDetalle = false;
            $scope.articulo.selected = undefined;
            $scope.formDetalleSubmited = false;
            $scope.formDetalle.articuloselected.$touched = false;
            $scope.formDetalle.detcantidad.$touched = false;
            $scope.formDetalle.detvunit.$touched = false;
        }
        function detalleActualizar() {
            $scope.docExi.detalles[$scope.newDetalle.correlativo-1] = $scope.newDetalle;
            $scope.detalleLimpiar();
            $scope.selectIsDisable = true;
        }
        function detalleAgregar() {
            $scope.formCrtl = true;
            if ($scope.form.$valid) {
                $scope.formDetalleSubmited = true;
                if ($scope.formDetalle.$valid) {
                    // Valido articulo ya ingresado
                    var ingresado = false;
                    if ($scope.docExi.detalles.length > 0) {
                        angular.forEach($scope.docExi.detalles, function (value, key) {
                            if ($scope.newDetalle.codigo == value.codigo) {
                                ingresado = true;
                            }
                        }, null);
                    }                    
                    if (!ingresado) {
                        $scope.newDetalle.selected = $scope.articulo.selected;
                        $scope.newDetalle.correlativo = $scope.docExi.detalles.length + 1;
                        $scope.docExi.detalles.push($scope.newDetalle);
                        $scope.detalleLimpiar();
                    } else {
                        toaster.pop('warning', "Alerta", "El artículo ya se encuentra en el detalle del documento");
                    }
                } else {
                    toaster.pop('warning', "Alerta", "Debe ingresar todos los datos del detalle correctamente");
                }
            } else {
                toaster.pop('warning', "Alerta", "Debe ingresar todos los datos del encabezado correctamente");
            }
        }
        function editarFila(idx) {            
            $scope.newDetalle = angular.copy($scope.docExi.detalles[idx]);
            $scope.articulo.selected = $scope.newDetalle.selected;
            $scope.selectIsDisable = true;
            $scope.disableAgregar = true;
            $scope.isEditandoDetalle = true;
        }
        function actualizarDocumento() {
            if ($scope.isEditandoMovimiento) {
                $scope.loadingActualizar = true;
                $scope.loadingBtnMenu = true;
                
                //TODO: VALIDACIONES
                StockServices.UpdateMovimientoExistencia($scope.docExi)
                    .then(function (data) {
                        toaster.pop('success', 'Documento Editado', 'El documento de existencia ha sido editado correctamente');
                        $scope.loadingActualizar = false;
                        $scope.loadingBtnMenu = false;

                        $scope.limpiar();
                    })
                    .catch(function (err) {
                        toaster.pop('error', "Error", 'Ocurrio un error al editar el Documento de Existencia, error: ' + (JSON.stringify(err) || ''));
                        $scope.loadingActualizar = false;
                        $scope.loadingBtnMenu = false;
                    });
            }
            else {
                toaster.pop('warning', "Alert", 'Debe seleccionar un documento a actualizar');
            }
        }
        
        // Funciones Botones del Formulario
        function eliminar() {
            if ($scope.isEditandoMovimiento) {
                $scope.loadingEliminar = true;
                //TODO: VALIDACIONES
                StockServices.DeleteMovimientoExistencia($scope.docExi)
                    .then(function (data) {
                        toaster.pop('success', 'Documento eliminado', 'El documento de existencia ha sido eliminado correctamente');
                        $scope.loadingEliminar = false;
                        $scope.limpiar();
                    })
                    .catch(function (err) {
                        toaster.pop('error', "Error", 'Ocurrio un error al eliminar el Documento de Existencia, error: ' + (JSON.stringify(err) || ''));
                        $scope.loadingEliminar = false;
                    });
            }
            else {
                toaster.pop('warning', "Alert", 'Debe seleccionar un documento a eliminar');
            }
        }

        function guardar() {
            //TODO: VALIDACIONES
            $scope.formCrtl = true;
            if ($scope.form.$valid) {
                if ($scope.docExi.detalles == null || $scope.docExi.detalles === undefined
                    || $scope.docExi.detalles.length == 0) {
                    toaster.pop('warning', "Alerta", 'El documento debe tener al menos un detalle');
                } else {
                    $scope.loadingBtnMenu = true;
                    $scope.loadingGuardar = true;
                    StockServices.CreateMovimientoExistencia($scope.docExi)
                        .then(function (data) {
                            toaster.pop('success', 'Documento creado', 'El documento de existencia ha sido creado, N° ' + data);
                            $scope.loadingBtnMenu = false;
                            $scope.loadingGuardar = false;
                            $state.go($state.current, {}, { reload: true });
                        })
                        .catch(function (err) {
                            toaster.pop('error', "Error", 'Ocurrio un error al crear el Documento de Existencia, error: ' + (JSON.stringify(err) || ''));
                            $scope.loadingBtnMenu = false;
                            $scope.loadingGuardar = false;
                        });
                }
            } else {
                toaster.pop('error', "Error", 'Complete todos los campos obligatorios');
            }
        }

        function imprimir() {
            if ($scope.isEditando) {
            } else {
                toaster.pop('warning', "Alert", 'Debe seleccionar un documento a imprimir');
            }
        }

        function limpiar() {
            $scope.loadingBtnMenu = true;
            $scope.encabezadoLimpiar();
            $scope.detalleLimpiar();
            $scope.loadingBtnMenu = false;
        }
        function encabezadoLimpiar() {
            $scope.docExi = {
                encabezado: {
                    fechaEmision : new Date(),
                    nroReferencia : ""
                },
                detalles: []
            };
            $scope.form.movimiento.$touched = false;
            $scope.form.observacion.$touched = false;
            $scope.form.bodega.$touched = false;
            $scope.isEditandoMovimiento = false;
            $scope.isIngresandoNew = true;
            $scope.formCrtl = false;            
        }

        function init() {
            StockServices.getBodegas()
                .then(function (data) {
                    $scope.bodegas = data;
                })
                .catch(function (err) {
                    toaster.pop('error', "Error al obtener bodegas", 'Ocurrio un error al obtener las bodegas, error: ' + err);
                });
            EmpresaServices.GetDocumentosReferencia()
                .then(function (data) {
                    $scope.documentosReferencia = data;
                })
                .catch(function (err) {
                    toaster.pop('error', "Error al obtener información", 'Ocurrio un error al obtener listado de documentos de referencia, error: ' + err);
                });
        }

        $scope.refreshArticulo = refreshArticulo;
        $scope.buscarMovimientoExistencia = buscarMovimientoExistencia;
        $scope.obtenerStock = obtenerStock;
        $scope.validaCero = validaCero;
        $scope.detalleBorrar = detalleBorrar;
        $scope.detalleLimpiar = detalleLimpiar;
        $scope.encabezadoLimpiar = encabezadoLimpiar;
        $scope.detalleActualizar = detalleActualizar;
        $scope.detalleAgregar = detalleAgregar;
        $scope.editarFila = editarFila;
        $scope.eliminar = eliminar;
        $scope.guardar = guardar;
        $scope.imprimir = imprimir;
        $scope.limpiar = limpiar;
        $scope.actualizarDocumento = actualizarDocumento;
        $scope.$watch('newDetalle', function () {
            $scope.newDetalle.total = ($scope.newDetalle.cantidad * $scope.newDetalle.valorUnitario).toFixed(0);
        }, true);
        $scope.$watch('articulo', function () {
            if ($scope.articulo !== null && $scope.articulo !== undefined) {
                if ($scope.articulo.selected !== null && $scope.articulo.selected !== undefined) {
                    $scope.newDetalle.codigo = $scope.articulo.selected.codigo;
                    $scope.newDetalle.descripcion = $scope.articulo.selected.descripcion;
                }
            }
        }, true);
        $scope.init = init;

        
        // INICIO DATE-PICKER
        $scope.today = function () {
            $scope.dt = new Date();
        };
        $scope.today();

        $scope.clear = function () {
            $scope.dt = null;
        };

        $scope.inlineOptions = {
            customClass: getDayClass,
            minDate: new Date(),
            showWeeks: true
        };

        $scope.dateOptions = {
            dateDisabled: disabled,
            formatYear: 'yy',
            maxDate: new Date(2020, 5, 22),
            minDate: new Date(),
            startingDay: 1
        };

        // Disable weekend selection
        function disabled(data) {
            var date = data.date,
              mode = data.mode;
            return mode === 'day' && (date.getDay() === 0 || date.getDay() === 6);
        }

        $scope.toggleMin = function () {
            $scope.inlineOptions.minDate = $scope.inlineOptions.minDate ? null : new Date();
            $scope.dateOptions.minDate = $scope.inlineOptions.minDate;
        };

        $scope.toggleMin();

        $scope.open1 = function () {
            $scope.popup1.opened = true;
        };

        $scope.open2 = function () {
            $scope.popup2.opened = true;
        };

        $scope.setDate = function (year, month, day) {
            $scope.dt = new Date(year, month, day);
        };

        $scope.formats = ['dd-MMMM-yyyy', 'yyyy/MM/dd', 'dd.MM.yyyy', 'shortDate'];
        $scope.format = $scope.formats[0];
        $scope.altInputFormats = ['M!/d!/yyyy'];

        $scope.popup1 = {
            opened: false
        };

        $scope.popup2 = {
            opened: false
        };

        var tomorrow = new Date();
        tomorrow.setDate(tomorrow.getDate() + 1);
        var afterTomorrow = new Date();
        afterTomorrow.setDate(tomorrow.getDate() + 1);
        $scope.events = [
          {
              date: tomorrow,
              status: 'full'
          },
          {
              date: afterTomorrow,
              status: 'partially'
          }
        ];

        function getDayClass(data) {
            var date = data.date,
              mode = data.mode;
            if (mode === 'day') {
                var dayToCheck = new Date(date).setHours(0, 0, 0, 0);

                for (var i = 0; i < $scope.events.length; i++) {
                    var currentDay = new Date($scope.events[i].date).setHours(0, 0, 0, 0);

                    if (dayToCheck === currentDay) {
                        return $scope.events[i].status;
                    }
                }
            }

            return '';
        }

        // FIN DATE-PICKER
        $scope.init();
    }

    angular
	.module('app')
	.controller('stockMovimientoController', stockMovimientoController);

    stockMovimientoController.$inject = ['$scope', '$filter', '$timeout', '$log', 'toaster', '$uibModal', 'StockServices', 'ArticulosServices', '$state', 'EmpresaServices'];

})();

(function () {
    'use strict';
    function ModalBuscarMovimiento($scope, $timeout, $log, StockServices, $uibModalInstance, documentosReferencia,bodegas) {
        $scope.documentosReferencia = documentosReferencia;
        $scope.bodegas = bodegas;
        $scope.toggleFiltros = false;
        $scope.movimientos = [];
        $scope.encabezado = {
            id: '',
            movimiento: '',
            bodega : {},
            fechaEmision: getPrimerDiaMes(),
            observacion: new Date(),// lo uso para evitar agregar otra propiedad a la clase
            referencia: '',
            nroReferencia: ''
        };
        function getPrimerDiaMes() {
            var hoy = new Date();
             var result = new Date(hoy.getYear()+1900, hoy.getMonth(), 1);
             return result;
        }
        function editar(idx) {
            StockServices.GetDetalleMovimientoExistencia($scope.movimientos[idx].encabezado)
            .then(function (data) {
                $uibModalInstance.close(data);
            })
            .catch(function (err) {
                $scope.mensaje = 'Ocurrio un error al obtener los movimientos de existencia, error: ' + err;
            });
            
        }
        function habilitaFiltro() {
            if ($scope.form.referencia.$viewValue.id > 0) {
                $scope.busquedaPorReferencia = true;
            } else {
                $scope.busquedaPorReferencia = false;
            }
        }
        $scope.aceptarVisible = true;

        $scope.aceptar = aceptar;
        $scope.cancelar = cancelar;
        $scope.editar = editar;
        $scope.habilitaFiltro = habilitaFiltro;

        function aceptar() {
            $scope.mensaje = '';
            StockServices.GetMovimientoExistencia($scope.encabezado)
                .then(function (data) {
                    $scope.movimientos = data;
                })
                .catch(function (err) {
                    $scope.mensaje = 'Ocurrio un error al obtener los movimientos de existencia, error: ' + err;
                });
        }

        function cancelar() {
            $uibModalInstance.dismiss('cancel');
        }
    }

    angular
	.module('app')
	.controller('ModalBuscarMovimiento', ModalBuscarMovimiento);

    ModalBuscarMovimiento.$inject = ['$scope', '$timeout', '$log', 'StockServices', '$uibModalInstance', 'documentosReferencia','bodegas'];

})();