(function () {
    'use strict';
    function inicioController($scope, $rootScope, authService) {
        $scope.usuario = authService.authentication.usuario;
    }

    angular
	.module('app')
	.controller('inicioController', inicioController);

    inicioController.$inject = ['$scope', '$rootScope', 'authService'];

})();