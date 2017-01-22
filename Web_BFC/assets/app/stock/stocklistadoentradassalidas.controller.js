(function () {
    'use strict';
    function StockListadoEntradasSalidasController($scope, toaster, StockServices, ArticulosServices, uiGridGroupingConstants, $filter, authService, EmpresaServices) {
        moment.locale('es');
        moment().format('L');
        $scope.informe = {};
        $scope.bodegas = [];
        $scope.bodega = {};
        $scope.articulo = {};
        $scope.articulos = [];
        $scope.fchHasta = new Date();
        $scope.fchDesde = new Date($scope.fchHasta.getFullYear(), $scope.fchHasta.getMonth(), 1);
        $scope.informe = {
            desde: '',
            hasta: '',
            mensaje:''
        };
        $scope.empresa = {
            id: authService.authentication.empresa,
            nombre :''
        };
        $scope.formDisable = false
        $scope.tableVisible = false;
        $scope.loading = false;

        $scope.gridOptions = {
            paginationPageSizes: [25, 50, 100],
            paginationPageSize: 25,
            //enableFiltering: true,
            treeRowHeaderAlwaysVisible: false,
            columnDefs: [
              {
                  field: 'DESCRIPCION', displayName: 'Artículo', width: 330, grouping: { groupPriority: 0 },
                  //sort: { priority: 0, direction: 'asc' },
                  cellTemplate: '<div><div ng-if="!col.grouping || col.grouping.groupPriority === undefined || col.grouping.groupPriority === null || ( row.groupHeader && col.grouping.groupPriority === row.treeLevel )" class="ui-grid-cell-contents" title="TOOLTIP">{{COL_FIELD CUSTOM_FILTERS}}</div></div>'
                  , enableColumnMenu: false, enableSorting: false, enableFiltering: true
              },
              //{ field: 'AR_DESART', displayName: 'Descripción', grouping: { groupPriority: 1 }, sort: { priority: 1, direction: 'asc' } },
              { field: 'EE_FECEMI', displayName: 'Fecha', cellFilter: 'date:\'dd-MM-yyyy\'', width: 90, cellClass: 'text-center', headerCellClass: 'text-center', enableColumnMenu: false, enableSorting: false },
              { field: 'EE_CODBOD', displayName: 'Bodega', width: 80, cellClass: 'text-center', headerCellClass: 'text-center', enableColumnMenu: false, enableSorting: false },
              { field: 'EE_TIPDOC', displayName: 'Movimiento', cellFilter: 'mapDocumentoExistencia', width: 100, cellClass: 'text-center', headerCellClass: 'text-center', enableColumnMenu: false, enableSorting: false },
              { field: 'EE_NUMDOC', displayName: 'N° Mov.', width: 100, cellClass: 'text-center', headerCellClass: 'text-center', enableColumnMenu: false, enableSorting: false },
              //{ field: 'EE_TIPDOC', displayName: 'Documento', cellFilter: 'mapDocumentoExistencia', width: 100, cellClass: 'text-center', headerCellClass: 'text-center', enableColumnMenu: false, enableSorting: false },
              //{ field: 'EE_NUMDOC', displayName: 'N° Doc.', width: 100, cellClass: 'text-center', headerCellClass: 'text-center', enableColumnMenu: false, enableSorting: false },
              { field: 'SALDO', displayName: 'Inicial', width: 120, treeAggregationType: uiGridGroupingConstants.aggregation.MAX, groupingSuppressAggregationText: true, cellTemplate: '<div><span style="padding:15px;">{{grid.appScope.GetSaldoInicial(grid, row)}}</span></div>', headerCellClass: 'text-center', enableColumnMenu: false, enableSorting: false },
              { field: 'entradas', displayName: 'Entradas', width: 120, type: 'number', treeAggregationType: uiGridGroupingConstants.aggregation.SUM, headerCellClass: 'text-center', enableColumnMenu: false, enableSorting: false },
              { field: 'salidas', displayName: 'Salidas', width: 120, type: 'number', treeAggregationType: uiGridGroupingConstants.aggregation.SUM, headerCellClass: 'text-center', enableColumnMenu: false, enableSorting: false },
              { name: 'Saldo Final', field: 'saldo', displayName: 'Saldos', width: 120, type: 'number', treeAggregationType: uiGridGroupingConstants.aggregation.MAX, groupingSuppressAggregationText: true, cellTemplate: '<div><span style="padding:15px;">{{grid.appScope.GetSaldoFinal(grid, row)}}</span></div>', headerCellClass: 'text-center', enableColumnMenu: false, enableSorting: false }
            ],
            enableGridMenu: false,
            /*exporterCsvFilename: 'ListadoDetalleEntradasSalidas.csv',
            exporterPdfFilename: 'ListadoDetalleEntradasSalidas.pdf',
            exporterPdfDefaultStyle: { fontSize: 9 },
            exporterPdfTableStyle: { margin: [30, 30, 30, 30] },
            exporterPdfTableHeaderStyle: { fontSize: 10, bold: true, italics: true, color: 'black' },
            exporterPdfHeader: { text: "Consulta de Stock", style: 'headerStyle' },
            exporterPdfFooter: function (currentPage, pageCount) {
                return { text: currentPage.toString() + ' de ' + pageCount.toString(), style: 'footerStyle' };
            },
            exporterPdfCustomFormatter: function (docDefinition) {
                docDefinition.styles.headerStyle = { fontSize: 22, bold: true };
                docDefinition.styles.footerStyle = { fontSize: 10, bold: true };
                return docDefinition;
            },
            exporterPdfOrientation: 'portrait',
            exporterPdfPageSize: 'LETTER',
            exporterPdfMaxGridWidth: 500,
            exporterCsvLinkElement: angular.element(document.querySelectorAll(".custom-csv-link-location")),
            gridMenuCustomItems: [
            //{
            //    title: "Exportar vista a Excel",
            //    action: function ($event) {
            //        $scope.exportViewExcel();
            //    }
            //    //,
            //    //icon: "glyphicon glyphicon-download"
            //},
            {
                title: "Exportar todo a Excel",
                action: function ($event) {
                    $scope.exportAllExcel();
                }//,
                //icon: "glyphicon glyphicon-download"
            },
            ],
            */
            onRegisterApi: function (gridApi) {
                $scope.gridApi = gridApi;
            }
        };

        $scope.GetSaldoInicial = function (grid, row, col) {
            if (row.groupHeader) {
                var entity = row.treeNode.children[0].row.entity;
                return "total: " + entity.SALDO;
                //return entity.SALDO;
            }
            else {
                //return row.entity.user.name;
                return "";
            }
        }
        $scope.GetSaldoFinal = function (grid, row, col) {
            if (row.groupHeader) {
                //var entity = row.treeNode.children[0].row.entity;
                var nombres = [];
                for (var o in row.entity) {
                    nombres.push(row.entity[o]);
                }
                //var r = row.entity[nombres[0]].value + row.entity[nombres[1]].value - row.entity[nombres[2]].value;
                var r = nombres[1].value + nombres[2].value - nombres[3].value;
                return "total: " + r;
            }
            else {
                return row.entity.subSaldo;
            }            
        }
        
        ////////$scope.GetSaldoFinal = function (grid, row) {
            
        //////    if (row.groupHeader) {
        //////        //if (col.displayName == 'Saldo Final') {
        //////        //            // SALDO INICIAL                     // TOTAL ENTRADAS                      // TOTAL SALIDAS
        //////        //    return row.treeNode.aggregations[1].value + row.treeNode.aggregations[2].value - row.treeNode.aggregations[3].value;
        //////        //}
        //////        return row.treeNode.aggregations[1].value + row.treeNode.aggregations[2].value - row.treeNode.aggregations[3].value;
        //////        //var entity = row.treeNode.children[0].row.entity;
                
        //////        //return "total: " + entity.SALDO;
        //////    }
        //////    else {
        //////        //if (col.displayName == 'Saldo Final') {
        //////            var entity = row.entity;

        //////            if (entity.subSaldo == null || isNaN(entity.subSaldo)) {
        //////                //grid.appScope.codart = entity.LE_CODART;
        //////                entity.subSaldo = entity.SALDO + entity.entradas - entity.salidas;
        //////            } else {
        //////                entity.subSaldo = entity.subSaldo + entity.entradas - entity.salidas;
        //////            }
        //////            return entity.subSaldo;
        //////        //}
        //////            //if (grid.appScope.codart != entity.LE_CODART) {
        //////        //    grid.appScope.subSaldo = entity.SALDO;
        //////        //}
        //////        //    entity.subSaldo = grid.appScope.subSaldo + entity.entradas - entity.salidas;
                
        //////        //return grid.appScope.subSaldo;
        //////        //var entity = row.entity;
        //////        //if (grid.codart == null) {
        //////        //    grid.codart = entity.LE_CODART;
        //////        //} else if (grid.codart != entity.LE_CODART) {
        //////        //    grid.subSaldo = entity.SALDO;
        //////        //}
        //////        //grid.subSaldo = grid.subSaldo + entity.entradas - entity.salidas;

                
        //////    }
        //////    return "";
        //////}
        function exportViewExcel() {

        }
        function exportAllExcel() {
            var columns = [];
            toaster.pop('info','','Generando exportacion, espere un momento');
            var sTable = '<table class="table" id="texport"><thead>';
            sTable += '<tr><th colspan="5" style="text-align:center">Empresa:</th><th>' + $scope.empresa.nombre + '</th><th>&nbsp;</th><th>&nbsp;</th><th>' + $filter('date')(new Date(),'dd-MM-yyyy hh:mm') + '</th></tr>';
            sTable += '<tr><th colspan="9" style="text-align:center">&nbsp;</th></tr>';
            sTable += '<tr><th colspan="5" style="text-align:center">&nbsp;</th><th>Listado de Entradas y Salidas</th></tr>';
            sTable += '<tr><th colspan="5" style="text-align:center">&nbsp;</th><th>Desde:</th><th>' + $filter('date')($scope.fchDesde, 'dd-MM-yyyy') + '</th><th>Hasta:</th><th>' + $filter('date')($scope.fchHasta, 'dd-MM-yyyy') + '</th></tr>';
            sTable += '<tr><th colspan="9" style="text-align:center">&nbsp;</th></tr><tr bgcolor="#cecece">';
            sTable += '<th>Código</th>';// codigo
            sTable += '<th>Descripción</th>';// descripcion
            sTable += '<th>Fecha</th>';// fecha
            sTable += '<th>Bodega</th>';// bodega
            sTable += '<th>Moviemiento</th>';// Movimiento
            sTable += '<th>N° Mov.</th>';// N° mov.
            sTable += '<th>Entradas</th>';// entradas
            sTable += '<th>Salidas</th>';// salidas
            sTable += '<th>Saldo</th>';// saldo

            sTable += '</tr></thead><tbody><tr><td></td></tr>';
            var totalEntradas = 0;
            var totalSalidas = 0;
            var totalSaldo = 0;
            var cont = 0;
            var ultimoArticulo = '';
            var cantidadRepeticiones = 0;
            for (var i = 0; i < $scope.gridOptions.data.length; i++) {
                var item = $scope.gridOptions.data[i];

                if (ultimoArticulo == '') {
                    ultimoArticulo = item["LE_CODART"];
                    sTable += '<tr>';
                    sTable += '<td>&nbsp;</td>';
                    sTable += '<td>Saldo Inicial</td>';
                    sTable += '<td>&nbsp;</td>';
                    sTable += '<td>&nbsp;</td>';
                    sTable += '<td>&nbsp;</td>';
                    sTable += '<td>&nbsp;</td>';
                    sTable += '<td>&nbsp;</td>';
                    sTable += '<td>&nbsp;</td>';
                    sTable += '<td>'+ item["SALDO"] +'</td>';
                    sTable += '</tr>';
                    //Si es distinto escribe linea de resumen totales y luego linea en blanco
                }
                if (ultimoArticulo != item["LE_CODART"]) {
                    sTable += '<tr style="border-top: solid 1px black">';
                    sTable += '<td>&nbsp;</td>';
                    sTable += '<td>&nbsp;</td>';
                    sTable += '<td>&nbsp;</td>';
                    sTable += '<td>&nbsp;</td>';
                    sTable += '<td>&nbsp;</td>';
                    sTable += '<td>Total</td>';
                    sTable += '<td>' + totalEntradas + '</td>';
                    sTable += '<td>' + totalSalidas + '</td>';
                    sTable += '<td>' + totalSaldo + '</td>';
                    sTable += '</tr>';
                    totalEntradas = 0;
                    totalSalidas = 0;
                    totalSaldo = 0;
                    ultimoArticulo = item["LE_CODART"];
                    // Linea en Blanco
                    sTable += '<tr><td>&nbsp;</td></tr>';

                    // Linea Stock Inicial
                    sTable += '<tr>';
                    sTable += '<td>&nbsp;</td>';
                    sTable += '<td>Saldo Inicial</td>';
                    sTable += '<td>&nbsp;</td>';
                    sTable += '<td>&nbsp;</td>';
                    sTable += '<td>&nbsp;</td>';
                    sTable += '<td>&nbsp;</td>';
                    sTable += '<td>&nbsp;</td>';
                    sTable += '<td>&nbsp;</td>';
                    sTable += '<td>' + item["SALDO"] + '</td>';
                    sTable += '</tr>';

                    ///****/*/*//*
                    sTable += '<tr>';
                    sTable += '<td>' + item["LE_CODART"] + '</td>';
                    sTable += '<td>' + item["AR_DESART"] + '</td>';
                    sTable += '<td>' + $filter('date')(item["EE_FECEMI"], 'dd-MM-yyyy') + '</td>';
                    sTable += '<td>' + item["EE_CODBOD"] + '</td>';
                    sTable += '<td>' + (item["EE_TIPDOC"] == 38 ? 'Entrada' : 'Salida') + '</td>';
                    sTable += '<td>' + item["EE_NUMDOC"] + '</td>';
                    sTable += '<td>' + item["entradas"] + '</td>';
                    sTable += '<td>' + item["salidas"] + '</td>';
                    sTable += '<td>' + item["subSaldo"] + '</td>';
                    sTable += '</tr>';
                    totalEntradas = totalEntradas + Number(item["entradas"]);
                    totalSalidas = totalSalidas + Number(item["salidas"]);
                    totalSaldo = Number(item["subSaldo"]);
                } else if (ultimoArticulo == item["LE_CODART"]) {
                    sTable += '<tr>';
                    sTable += '<td>' + item["LE_CODART"] + '</td>';
                    sTable += '<td>' + item["AR_DESART"] + '</td>';
                    sTable += '<td>' + $filter('date')(item["EE_FECEMI"], 'dd-MM-yyyy') + '</td>';
                    sTable += '<td>' + item["EE_CODBOD"] + '</td>';
                    sTable += '<td>' + (item["EE_TIPDOC"] == 38 ? 'Entrada' : 'Salida') + '</td>';
                    sTable += '<td>' + item["EE_NUMDOC"] + '</td>';
                    sTable += '<td>' + item["entradas"] + '</td>';
                    sTable += '<td>' + item["salidas"] + '</td>';
                    sTable += '<td>' + item["subSaldo"] + '</td>';
                    sTable += '</tr>';
                    totalEntradas = totalEntradas + Number(item["entradas"]);
                    totalSalidas = totalSalidas + Number(item["salidas"]);
                    totalSaldo = Number(item["subSaldo"]);
                }
            }
            //ultima linea
            sTable += '<tr>';
            sTable += '<td>&nbsp;</td>';
            sTable += '<td>&nbsp;</td>';
            sTable += '<td>&nbsp;</td>';
            sTable += '<td>&nbsp;</td>';
            sTable += '<td>&nbsp;</td>';
            sTable += '<td>Total</td>';
            sTable += '<td>' + totalEntradas + '</td>';
            sTable += '<td>' + totalSalidas + '</td>';
            sTable += '<td>' + totalSaldo + '</td>';
            sTable += '</tr>';
            sTable += '</tbody></table>';
            
            $("#tabla-export").html(sTable);
            $('#texport').tableExport({type:'excel',escape:'false',tableName:'Listado'});
            $("#tabla-export").html('');
        }

        

        $scope.cantidadAcumulada = 0;
        $scope.articuloAgrupado = '';
        function generarInforme() {
            $scope.form.$submitted = true;
            if ($scope.form.$valid) {
                $scope.loading = true;
                $scope.tableVisible = true;
                $scope.cantidadAcumulada = 0;
                $scope.articuloAgrupado = '';
                StockServices.GetListadoEntradasSalidas($scope.articulo.selected, $scope.fchDesde, $scope.fchHasta)
                .then(function (data) {
                    if (data != "") {
                        var datos = JSON.parse(data);
                        var arti = undefined;
                        $scope.informe.desde = $filter('date')($scope.fchDesde, 'dd-MM-yyyy');
                        $scope.informe.hasta = $filter('date')($scope.fchHasta, 'dd-MM-yyyy');
                        $scope.informe.mensaje = 'Listado de Stock: ' + ($scope.articulo.selected != null
                            && $scope.articulo.selected.descripcion != null
                            && $scope.articulo.descripcion != '' ? 'artículo: ' +
                            $scope.articulo.selected.descripcion : 'Todos los artículos') + ', desde ' + $scope.informe.desde + ' hasta ' + $scope.informe.hasta;
                        for (var i in datos) {
                            datos[i].entradas = datos[i].EE_TIPDOC == 38 ? datos[i].LE_CANART : 0;
                            datos[i].salidas = datos[i].EE_TIPDOC == 42 ? datos[i].LE_CANART : 0;
                            // SI ES PRIMERA LINEA
                            if ($scope.articuloAgrupado == '') {
                                $scope.articuloAgrupado = datos[i].LE_CODART;
                                $scope.cantidadAcumulada = datos[i].SALDO + datos[i].entradas - datos[i].salidas;
                                datos[i].subSaldo = $scope.cantidadAcumulada;

                                // CONTINUA CON EL MISMO ARTICULO
                            } else if ($scope.articuloAgrupado == datos[i].LE_CODART) {
                                $scope.cantidadAcumulada = $scope.cantidadAcumulada + datos[i].entradas - datos[i].salidas;
                                datos[i].subSaldo = $scope.cantidadAcumulada;
                                // ES UN NUEVO ARTICULO
                            } else if ($scope.articuloAgrupado != datos[i].LE_CANART && $scope.articuloAgrupado != '') {
                                $scope.articuloAgrupado = datos[i].LE_CODART;
                                $scope.cantidadAcumulada = 0;
                                $scope.cantidadAcumulada = datos[i].SALDO + datos[i].entradas - datos[i].salidas;
                                datos[i].subSaldo = $scope.cantidadAcumulada;
                            }                            
                        }
                        $scope.gridOptions.data = datos;
                        //$scope.formDisable = true;
                        toaster.pop('success', "Generado", 'Informe generado correctamente');
                        $scope.tableVisible = true;

                    } else {
                        toaster.pop('info', "Sin información", 'No se ha encontrado información para el periodo seleccionado.');
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
                    if ($scope.informe.marzo[0] > 0 || $scope.informe.marzo[1] > 0) {
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
            //$scope.formDisable = false;
            $scope.informe = {
                desde: '',
                hasta: '',
                mensaje: ''
            };
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

            EmpresaServices.get()
            .then(function (data) {
                $scope.empresas = data;
                if ($scope.empresas != null) {

                    angular.forEach($scope.empresas, function (value, key) {
                        if (value.id == $scope.empresa.id) {
                            $scope.empresa = value;
                        }
                    });
                }
            })
            .catch(function (err) {
                toaster.pop('error', "Error cambio de empresa", 'Ocurrio un error al cambiar de empresa, error: ' + err);
            });
        }

        $scope.generarInforme = generarInforme;
        $scope.refreshArticulo = refreshArticulo;
        $scope.limpiarArticuloSelected = limpiarArticuloSelected;
        $scope.verDetalles = verDetalles;
        $scope.clear = clear;
        $scope.informeReset = informeReset;
        $scope.exportViewExcel = exportViewExcel;
        $scope.exportAllExcel = exportAllExcel;
        $scope.init = init;

        $scope.init();
    }

    angular
	.module('app')
	.controller('StockListadoEntradasSalidasController', StockListadoEntradasSalidasController);

    StockListadoEntradasSalidasController.$inject = ['$scope', 'toaster', 'StockServices', 'ArticulosServices', 'uiGridGroupingConstants', '$filter', 'authService', 'EmpresaServices'];

})();

