(function () {
    'use strict';
    function loginController($scope, $state, authService, ngAuthSettings, toaster) {
        $scope.loading = false;
        $scope.loginData = {
            userName: "cmunro@bevfoodcenter.cl",
            password: "bfc_2016.",
            //userName: "admin@bevfoodcenter.cl",
            //password: "Super4dm",
            useRefreshTokens: false
        };

        $scope.login = function () {
            $scope.loading = true;
            $scope.form.$submitted = true;
            if ($scope.form.$valid) {
                authService.login($scope.loginData).then(function (response) {
                    $state.go('inicio');
                },
                 function (err) {
                     toaster.pop('error', "Error al ingresar", (err !== undefined && err.error_description !== undefined ? err.error_description : JSON.stringify(err || '')) );
                     $scope.loading = false;
                 });
            } else {
                toaster.pop('error', "Error", "Verifique que se han llenado correctamente todos los campos.");
                $scope.loading = false;
            }
        };
    }

    angular
	.module('app')
	.controller('loginController', loginController);

    loginController.$inject = ['$scope', '$state', 'authService', 'ngAuthSettings', 'toaster'];

})();