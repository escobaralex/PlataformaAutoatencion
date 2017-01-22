(function () {
    'use strict';
    angular
        .module('app').filter('propsFilter', function () {
        return function (items, props) {
            var out = [];

            if (angular.isArray(items)) {
                items.forEach(function (item) {
                    var itemMatches = false;

                    var keys = Object.keys(props);
                    for (var i = 0; i < keys.length; i++) {
                        var prop = keys[i];
                        var text = props[prop].toLowerCase();
                        if (item[prop].toString().toLowerCase().indexOf(text) !== -1) {
                            itemMatches = true;
                            break;
                        }
                    }

                    if (itemMatches) {
                        out.push(item);
                    }
                });
            } else {
                // Let the output be the input untouched
                out = items;
            }

            return out;
        }
    });

    function stockConsulta($scope, toaster, i18nService, StockServices, ArticulosServices, uiGridConstants) {
        i18nService.setCurrentLang('es');
        $scope.bodegas = [];
        $scope.bodega = {};
        $scope.articulo = {};
        $scope.articulos = [];
        $scope.informeHasta = '';
        
        $scope.selectIsDisable = false;
        $scope.gridVisible = false;
        $scope.loading = false;
        $scope.exportAllExcel = exportAllExcel;
        $scope.exportViewExcel = exportViewExcel;


        function selectDisable() {
            $scope.selectIsDisable = true;
        };

        function selectEnabled() {
            $scope.selectIsDisable = false;
        };

        function generarInforme() {
            $scope.loading = true;
            StockServices.get($scope.articulo.selected, $scope.bodega)
            .then(function (data) {
                $scope.gridOptions.data = data;
                toaster.pop('success', "Generado", 'Informe generado correctamente'); 
                $scope.gridVisible = true;
                $scope.loading = false;
            })
            .catch(function (err) {
                toaster.pop('error', "Error al obtener información", 'Ocurrio un error al obtener la información, error: ' + err);
                $scope.gridVisible = false;
                $scope.loading = false;
            });            
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
        function init() {
            StockServices.getBodegas()
                .then(function (data) {
                    $scope.bodegas = data;
                })
                .catch(function (err) {
                    toaster.pop('error', "Error al obtener bodegas", 'Ocurrio un error al obtener las bodegas, error: ' + err);
                });          
        }

        $scope.generarInforme = generarInforme;
        $scope.selectEnabled = selectEnabled;
        $scope.selectDisable = selectDisable;        
        $scope.refreshArticulo = refreshArticulo;
        $scope.limpiarArticuloSelected = limpiarArticuloSelected;
        $scope.init = init;
       
        // INICIO UI-GRID
        $scope.gridOptions = {
            paginationPageSizes: [25, 50, 100],
            paginationPageSize: 25,
            //enableFiltering: true,
            columnDefs: [
              { field: 'codigo', displayName: 'Código', width: 200},
              { field: 'descripcion', displayName: 'Descripción' },
              { field: 'unidadDeMedida', displayName: 'Unidad de Medida', width: 200 },
              {
                  field: 'stock', displayName: 'Stock', width: 200, type: 'number'
                  //filters: [
                  //  {
                  //      condition: uiGridConstants.filter.GREATER_THAN,
                  //      placeholder: 'greater than'
                  //  },
                  //  {
                  //      condition: uiGridConstants.filter.LESS_THAN,
                  //      placeholder: 'less than'
                  //  }
                  //] },
                  //{ name: 'Stock', displayName: 'Edit', cellTemplate: '<button id="editBtn" type="button" class="btn-small" ng-click="edit(row.entity)" >Edit</button> ' }
              }],
            enableGridMenu: true,
            exporterCsvFilename: 'consultaStock.csv',
            exporterPdfFilename: 'consultaStock.pdf',
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
            onRegisterApi: function (gridApi) {
                $scope.gridApi = gridApi;
            }
        };
        function exportViewExcel() {

        }
        function exportAllExcel() {
            alasql.promise('SELECT codigo AS CODIGO, descripcion AS DESCRIPCION, unidadDeMedida AS MEDIDA, stock AS STOCK INTO XLSX("Consulta_Stock.xlsx",{headers:true}) FROM ?', [$scope.gridOptions.data])
            .then(function (res) {
                console.log(res); // output depends on mydata.xls
            }).catch(function (err) {
                console.log('Does the file exists? there was an error:', err);
            });
        }

        // INICIO DATE-PICKER
        $scope.dateOptions = {
            formatYear: 'yy',
            startingDay: 1
        };
        $scope.popup1 = {
            opened: false
        };

        $scope.open1 = function () {
            $scope.popup1.opened = true;
        };

        $scope.formats = ['dd-MMMM-yyyy', 'yyyy/MM/dd', 'dd.MM.yyyy', 'shortDate'];
        $scope.format = $scope.formats[0];
        $scope.altInputFormats = ['M!/d!/yyyy'];
        $scope.today = function () {
            $scope.dt = new Date();
        };
        $scope.today();

        $scope.clear = function () {
            $scope.dt = null;
        };

        // Disable weekend selection
        $scope.disabled = function (date, mode) {
            return mode === 'day' && (date.getDay() === 0 || date.getDay() === 6);
        };

        $scope.toggleMin = function () {
            $scope.minDate = $scope.minDate ? null : new Date();
        };

        $scope.toggleMin();
        $scope.maxDate = new Date(2050, 12, 31);
                
        var tomorrow = new Date();
        tomorrow.setDate(tomorrow.getDate() + 1);
        var afterTomorrow = new Date();
        afterTomorrow.setDate(tomorrow.getDate() + 1);
        $scope.events =
          [
            {
                date: tomorrow,
                status: 'full'
            },
            {
                date: afterTomorrow,
                status: 'partially'
            }
          ];

        $scope.getDayClass = function (date, mode) {
            if (mode === 'day') {
                var dayToCheck = new Date(date).setHours(0, 0, 0, 0);

                for (var i = 0; i < $scope.events.length; i++) {
                    var currentDay = new Date($scope.events[i].date).setHours(0, 0, 0, 0);

                    if (dayToCheck === currentDay) {
                        return $scope.events[i].status;
                    }
                }
            }
        }

        // FIN DATE-PICKER
        $scope.init();
    }

    angular
	.module('app')
	.controller('stockConsulta', stockConsulta);

    stockConsulta.$inject = ['$scope', 'toaster', 'i18nService', 'StockServices', 'ArticulosServices', 'uiGridConstants'];

})();