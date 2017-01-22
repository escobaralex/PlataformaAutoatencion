(function () {
    'use strict';
    function CobroAlmacenamientoController($scope, CobrosServices, toaster) {
        $scope.meses = meses;
        $scope.anos = [];
        $scope.mesSelected = 0;
        $scope.anoSelected = 0;
        $scope.formDisabled = false;
        $scope.showReport = false;

        $scope.init = init;
        $scope.GetCobroAlmacenamiento = GetCobroAlmacenamiento;
        $scope.clear = clear;

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
        }

        function GetCobroAlmacenamiento() {
            $scope.formDisabled = true;
            $scope.loading = true;
            CobrosServices.GetCobroAlmacenamiento($scope.mesSelected, $scope.anoSelected)
            .then(function (data) {
                $scope.results = data;
                $scope.loading = false;
                $scope.showReport = true;
            })
            .catch(function (err) {
                toaster.pop('error', "Error al obtener información", 'Ocurrio un error al obtener cobros de almacenamiento');
                $scope.formDisabled = false;
                $scope.loading = false;
            });
        }
        function clear() {
            $scope.formDisabled = false;
            $scope.showReport = false;
        }

        $scope.init();
    }

    angular
	.module('app')
	.controller('CobroAlmacenamientoController', CobroAlmacenamientoController);

    CobroAlmacenamientoController.$inject = ['$scope', 'CobrosServices', 'toaster'];

})();