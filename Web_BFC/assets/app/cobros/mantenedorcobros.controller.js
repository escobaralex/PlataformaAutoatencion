(function () {
    'use strict';
    function MantenedorCobrosController($scope, $log, toaster, CobrosServices) {
        // fields
        $scope.valoresCobro = {
            metroCuadrado: 0,
            metroCubico: 0,
            posicionPalet: 0,
            valorDefault: 'metroCubico'
        };

        // functions
        $scope.save = save;

        // fields de control de interfaz
        $scope.isGuardar = false;
        $scope.isEditando = false;

        function save() {
            $scope.isGuardar = true;
            if ($scope.metroCuadrado == '')
                $scope.metroCuadrado = 0;
            if ($scope.metroCubico == '')
                $scope.metroCubico = 0;
            if ($scope.posicionPalet == '')
                $scope.posicionPalet = 0;
            // Validaciones del formulario 
            if ($scope.isEditando) {
                CobrosServices.UpdateCobrosCliente($scope.valoresCobro)
                    .then(function (data) {
                        toaster.pop('success', "", 'Valores actualizados correctamente');
                        $scope.isEditando = true;
                        $scope.isGuardar = false;
                    })
                    .catch(function (err) {
                        toaster.pop('error', "", 'Ocurrio un error al intentar actualizar los valores de cobro para la empresa');
                    });
            } else {
                CobrosServices.SaveCobrosCliente($scope.valoresCobro)
                    .then(function (data) {
                        toaster.pop('success', "", 'Valores guardados correctamente');
                        $scope.isEditando = true;
                        $scope.isGuardar = false;
                    })
                    .catch(function (err) {
                        toaster.pop('error', "", 'Ocurrio un error al intentar guardar los valores de cobro para la empresa');
                    });
            }
        }

        function init() {
            CobrosServices.GetValoresCobros()
                .then(function (data) {
                    if (data == null) {
                        toaster.pop('info', "", 'No existen los valores de cobros para esta empresa');
                    } else {
                        $scope.valoresCobro = data;
                        $scope.isEditando = true;
                    }
                })
                .catch(function (err) {
                    toaster.pop('error', "Error al obtener empresas", 'Ocurrio un error al obtener empresas, error: ' + err);
                });
        }

        $scope.init = init;

        $scope.init();
    }

    angular
	.module('app')
	.controller('MantenedorCobrosController', MantenedorCobrosController);

    MantenedorCobrosController.$inject = ['$scope', '$log', 'toaster', 'CobrosServices'];

})();
