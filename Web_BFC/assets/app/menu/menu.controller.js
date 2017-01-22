(function () {

    function menuController($scope, $log, $location,$state, authService, EmpresaServices, $uibModal, toaster) {
        $scope.visible = false;
        $scope.isAdmin = false;
        $scope.isCliente = false;
        $scope.isOperador = false;
        
        $scope.empresa = {};
        $scope.empresas = [];
        $scope.mostrarBtnCambiar = false;
        $scope.usuario = {};

        function changeEmpresa() {
            if ($scope.isCliente == false) {
                
                var modalInstance = $uibModal.open({
                    animation: $scope.animationsEnabled,
                    templateUrl: 'cambio-empresa.html',
                    controller: 'ModalCambioEmpresaController',
                    //                size: '400',
                    resolve: {
                        empresas: function () {
                            var emp = [];
                            angular.forEach($scope.empresas, function (value, key) {
                                if (value.id != $scope.empresa.id) {
                                    emp.push(value);
                                }
                            });
                            return emp;
                        }
                    }
                });

                modalInstance.result.then(function (selectedItem) {
                    
                    EmpresaServices.changeEmpresa(selectedItem.id)
                    .then(function (data) {
                        toaster.pop('success', 'Cambio de Empresa', 'Cambio de empresa exitoso');
                        $scope.empresa = selectedItem;
                        authService.authentication.empresa = selectedItem.id;
                        $state.go($state.current, {}, { reload: true });
                    })
                    .catch(function (err) {
                        toaster.pop('error', "Error cambio de empresa", 'Ocurrio un error al cambiar de empresa, error: ' + JSON.stringify(err));
                    });

                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
                
            } else {
                toaster.pop('error', "Error cambio de empresa", 'No cuenta con permisos para realizar esta operación');
            }
            
        }
        function init() {
            if ($scope.isCliente == false) {
                EmpresaServices.get()
                   .then(function (data) {
                       $scope.empresas = data;
                       if ($scope.empresas != null) {
                           if ($scope.empresas.length > 1) {
                               $scope.mostrarBtnCambiar = true;
                           }
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
        }
        function logout() {
            authService.logOut()
            .then(function (data) {
                $state.go('login');
            })
            .catch(function (err) {
                toaster.pop('error', "Error", 'Ocurrio un error al intentar cerrar la sessión, error: ' + JSON.stringify(err));
            });           
        }

        $scope.changeEmpresa = changeEmpresa;
        $scope.init = init;
        $scope.logout = logout;
        
        if (authService.authentication.isAuth && $location.$$path != "/login") {
            $scope.visible = true;
            $scope.usuario.nombre = authService.authentication.usuario;
            if (authService.authentication.rol == 'Admin') {
                $scope.isAdmin = true;
                $scope.isCliente = false;
                $scope.isOperador = false;
            } else if (authService.authentication.rol == 'Cliente') {
                $scope.isAdmin = false;
                $scope.isCliente = true;
                $scope.isOperador = false;
            } else if (authService.authentication.rol == 'Operador') {
                $scope.isAdmin = false;
                $scope.isCliente = false;
                $scope.isOperador = true;
            }
            $scope.usuario.email = authService.authentication.userName;
            $scope.usuario.rol = authService.authentication.rol;
            $scope.empresa.id = authService.authentication.empresa;
            $scope.init();
        }
    }

    angular.module('app').controller('menuController', menuController);
    
    menuController.$inject = ['$scope', '$log', '$location', '$state', 'authService', 'EmpresaServices', '$uibModal', 'toaster'];
})();

(function () {
    function ModalCambioEmpresaController($scope, $uibModalInstance, empresas) {
        $scope.empresas = empresas;
        $scope.mostrarError = false;
        $scope.empresa = {};
        $scope.aceptar = aceptar;
        $scope.cancelar = cancelar;

        function aceptar() {
            if ($scope.empresa == null || $scope.empresa == undefined || $scope.empresa.id == undefined) {
                $scope.mostrarError = true;
            } else {
                //('Acepto empresa: ' + $scope.empresa.id + '-' + $scope.empresa.nombre);
                $uibModalInstance.close($scope.empresa);
            }
        }
        function cancelar() {
            $uibModalInstance.dismiss('cancel');
        }
    }

    angular
	.module('app')
	.controller('ModalCambioEmpresaController', ModalCambioEmpresaController);

    ModalCambioEmpresaController.$inject = ['$scope', '$uibModalInstance', 'empresas'];

})();