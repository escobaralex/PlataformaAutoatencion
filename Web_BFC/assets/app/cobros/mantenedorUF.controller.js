(function () {
    'use strict';
    function MantenedorUFController($scope, $log, toaster, CobrosServices) {
        // Propiedades
        $scope.meses = meses;
        $scope.anos = [];
        $scope.mesSelected = 0;
        $scope.anoSelected = 0;
        $scope.mensaje = "Guardar";
        $scope.isEditando = false;

        // Propiedades relacionadas a la vista
        $scope.formDisabled = false;

        // Funciones
        $scope.getValorMes = getValorMes;
        $scope.save = save;        
      
        function save() {
            if ($scope.valor == "" || $scope.valor == 0) {
                toaster.pop('error', "", 'El valor de la UF debe ser mayor a 0');
            }
            else if ($scope.mesSelected != "" && $scope.anoSelected != "" && $scope.valor !== undefined && $scope.valor != '') {
                $scope.formDisabled = true;
                $scope.mensaje = "Guardando...";
                if ($scope.isEditando) {
                    CobrosServices.UpdValorUFMes($scope.anoSelected, $scope.mesSelected, $scope.valor)
                        .then(function (data) {
                            $scope.formDisabled = false;                            
                            toaster.pop('success', '', 'Valor UF actualizado para el periodo ' + $scope.mesSelected + '/' + $scope.anoSelected);
                            $scope.mensaje = "Guardar";
                        })
                        .catch(function (err) {
                            $scope.formDisabled = false;
                            $scope.mensaje = "Guardar";
                            toaster.pop('error', "Error", 'Ocurrio un error al actualizar el valor de la UF para el periodo seleccionado');
                        });
                } else {
                    CobrosServices.SaveValorUFMes($scope.anoSelected, $scope.mesSelected, $scope.valor)
                    .then(function (data) {
                        $scope.formDisabled = false;                        
                        toaster.pop('success', '', 'Valor UF guardado para el periodo ' + $scope.mesSelected + '/' + $scope.anoSelected);
                        $scope.mensaje = "Guardar";
                    })
                    .catch(function (err) {
                        $scope.formDisabled = false;
                        $scope.mensaje = "Guardar";
                        toaster.pop('error', "Error", 'Ocurrio un error al guardar el valor de la UF para el periodo seleccionado');
                    });
                }
            }
        }

        function getValorMes() {
            if ($scope.mesSelected != 0 && $scope.anoSelected > 0) {
                $scope.formDisabled = true;
                $scope.mensaje = "Obteniendo valor...";
                CobrosServices.GetValorUFMes($scope.mesSelected, $scope.anoSelected)
                   .then(function (data) {
                       $scope.formDisabled = false;
                       if (data == null || data == '' || data.length == 0) {
                           toaster.pop('info', '', 'No existe valor UF para el periodo ' + $scope.mesSelected + '/' + $scope.anoSelected);
                           $scope.isEditando = false;
                           $scope.valor = '';
                       } else {
                           $scope.valor = data[0].valor;
                           toaster.pop('success', '', 'Valor obtenido para el periodo ' + $scope.mesSelected + '/' + $scope.anoSelected);
                           $scope.isEditando = true;
                       }
                       $scope.mensaje = "Guardar";
                   })
                   .catch(function (err) {
                       $scope.mensaje = "Guardar";
                       $scope.formDisabled = false;
                       $scope.isEditando = false;
                       $scope.valor = '';
                       toaster.pop('error', "Error al obtener información", 'Ocurrio un error al obtener el valor para el periodo seleccionado');
                   });
            }
        }

        function init() {
            var currentYear = new Date().getFullYear();
            var maxAno = 0;
            if (currentYear != 2016) {
                for (var i = 2016; i < currentYear; i++) {
                    $scope.anos.push(i);
                    maxAno++;
                }
                $scope.anoSelected = $scope.anos[maxAno];
            } else {
                $scope.anos.push(2016);
                $scope.anoSelected = $scope.anos[0];
            }
            var currentMonth = new Date().getMonth();
            $scope.mesSelected = $scope.meses[currentMonth];
            $scope.getValorMes();
        }

        $scope.init = init;

        $scope.init();
    }

    angular
	.module('app')
	.controller('MantenedorUFController', MantenedorUFController);

    MantenedorUFController.$inject = ['$scope', '$log', 'toaster', 'CobrosServices'];

})();
