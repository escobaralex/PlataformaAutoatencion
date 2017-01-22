(function () {
    'use strict';
    function UsuariosController($scope, $log, toaster, $uibModal, i18nService, UsuariosServices, EmpresaServices) {
        i18nService.setCurrentLang('es');
        $scope.animationsEnabled = true;
        $scope.empresas = [];
        $scope.roles = [];
        //$scope.formasCobros = [];
        $scope.usuario = {};

        $scope.gridVisible = false;
        $scope.loading = false;
        
        function addUser(grid, row) {
            $log.info('addUser');

            var modalInstanceAdd = $uibModal.open({
                animation: $scope.animationsEnabled,
                templateUrl: 'usuario-detalle.html',
                controller: 'ModalNewUptUsuario',
//                size: '400',
                resolve: {
                    usuario: function () {
                        return $scope.usuario;
                    },
                    operacion: function () {
                        return "agregar";
                    },
                    empresas: function () {
                        return $scope.empresas;
                    },
                    roles: function () {
                        return $scope.roles;
                    }
                }
            });

            modalInstanceAdd.result.then(function (usuarioCreado) {
                UsuariosServices.ObtenerAll()
               .then(function (data) {
                   $scope.gridOptions.data = data;
               })
               .catch(function (err) {
                   toaster.pop('error', "Error al obtener usuarios", 'Ocurrio un error al obtener los usuarios, error: ' + err);
               });
                toaster.pop('success', "Usuario Creado", 'El usuario ' + usuarioCreado.email + ' fue creado correctamente.');
            }, function () {
                $log.info('Modal dismissed at: ' + new Date());
            });
        }
        function verUsuario(grid, row) {

            var modalInstance = $uibModal.open({
                animation: $scope.animationsEnabled,
                templateUrl: 'usuario-detalle.html',
                controller: 'ModalNewUptUsuario',
                //                size: '400',
                resolve: {
                    usuario: function () {
                        return row.entity;
                    },
                    operacion: function () {
                        return "ver";
                    },
                    empresas: function () {
                        return $scope.empresas;
                    },
                    roles: function () {
                        return $scope.roles;
                    }
                }
            });

            //modalInstance.result.then(function (selectedItem) {
            //    $scope.selected = selectedItem;
            //}, function () {
            //    $log.info('Modal dismissed at: ' + new Date());
            //});
        }
        function editarUsuario(grid, row) {

            var modalInstanceEditar = $uibModal.open({
                animation: $scope.animationsEnabled,
                templateUrl: 'usuario-detalle.html',
                controller: 'ModalNewUptUsuario',
                //                size: '400',
                resolve: {
                    usuario: function () {
                        return row.entity;
                    },
                    operacion: function () {
                        return "editar";
                    },
                    empresas: function () {
                        return $scope.empresas;
                    },
                    roles: function () {
                        return $scope.roles;
                    }
                }
            });

            modalInstanceEditar.result.then(function (usuario) {
                UsuariosServices.ObtenerAll()
               .then(function (data) {
                   $scope.gridOptions.data = data;
               })
               .catch(function (err) {
                   toaster.pop('error', "Error al obtener usuarios", 'Ocurrio un error al obtener los usuarios, error: ' + err);
               });
                toaster.pop('success', "Usuario Editado", 'El usuario ' + usuario.email + ' fue modificado correctamente.');
            }, function () {
                $log.info('Modal dismissed at: ' + new Date());
            });
        }
        function eliminarUsuario(grid, row) {
            var modalInstanceEliminar = $uibModal.open({
                animation: $scope.animationsEnabled,
                templateUrl: 'usuario-eliminar.html',
                controller: 'ModalEliminarUsuario',
                //                size: '400',
                resolve: {
                    usuario: function () {
                        return row.entity;
                    }
                }
            });
            modalInstanceEliminar.result.then(function (usuarioEliminado) {
                
                UsuariosServices.Remover(usuarioEliminado)
               .then(function (data) {
                   // Luego de eliminar Recarga los usuarios
                   $scope.gridOptions.data = data;                   
                   toaster.pop('success', "Eliminar usuario", 'Usuario eliminado correctamente');
               })
               .catch(function (err) {
                   toaster.pop('error', "Error al obtener usuarios", 'Ocurrio un error al obtener los usuarios, error: ' + err);
               });
            }, function () {
                $log.info('Modal dismissed at: ' + new Date());
            });
        }
        
        // INICIO UI-GRID
        $scope.gridOptions = {
            paginationPageSizes: [10, 50, 100],
            paginationPageSize: 10,
            
            columnDefs: [
              {
                  field: 'nombres', displayName: 'Nombre', width: 250,
                  cellTemplate: '<div class="ui-grid-cell-contents">{{row.entity.nombres}} {{row.entity.apellidos}}</div>'
            },
              { field: 'email', displayName: 'Email' }, 
              { field: 'rol.name', displayName: 'Rol', width: 150 },
              { field: 'empresa.nombre', displayName: 'Empresa', width: 200 },
              {
                  field: 'id', name: 'Acciones', width: 270, cellTemplate: '<div class="ui-grid-cell-contents">' +
                      // Boton VER
                  '<button type="button" class="btn btn-xs btn-info" style="width:80px"' +
                  'ng-click="grid.appScope.verUsuario(grid, row)"><i class="fa fa-eye"></i>&nbsp;&nbsp;Ver</button>' +
                      // Boton EDITAR
                  '&nbsp;&nbsp;<button type="button" class="btn btn-xs btn-primary"  style="width:80px"' +
                  'ng-click="grid.appScope.editarUsuario(grid, row)"><i class="fa fa-edit"></i>&nbsp;&nbsp;Editar</button>' +
                      // Boton ELIMINAR
                  '&nbsp;&nbsp;<button type="button" class="btn btn-xs btn-danger"  style="width:80px"' +
                  'ng-click="grid.appScope.eliminarUsuario(grid, row)"><i class="fa fa-trash-o"></i>&nbsp;&nbsp;Eliminar</button></div>'
              }              
            ],

            onRegisterApi: function (gridApi) {
                $scope.gridApi = gridApi;
            }
        };

        function init() {
            
            UsuariosServices.ObtenerAll()
               .then(function (data) {
                   $scope.gridOptions.data = data;
                   $scope.gridVisible = true;
               })
               .catch(function (err) {
                   toaster.pop('error', "Error al obtener usuarios", 'Ocurrio un error al obtener los usuarios, error: ' + err);
               });
            EmpresaServices.get()
                .then(function (data) {
                    $scope.empresas = data;
                })
                .catch(function (err) {
                    toaster.pop('error', "Error al obtener información", 'Ocurrio un error al obtener los usuarios, error: ' + err);
                });
            UsuariosServices.GetRoles()
                .then(function (data) {
                    $scope.roles = data;
                })
               .catch(function (err) {
                   toaster.pop('error', "Error al obtener información", 'Ocurrio un error al obtener los roles');
               });
            //UsuariosServices.GetFormasCobro()
            //    .then(function (data) {
            //        $scope.formasCobros = data;
            //    })
            //    .catch(function (err) {
            //        toaster.pop('error', "Error al obtener información", 'Ocurrio un error al obtener formas de cobro');
            //    });

        }

        $scope.init = init;
        $scope.addUser = addUser;
        $scope.verUsuario = verUsuario;
        $scope.editarUsuario = editarUsuario;
        $scope.eliminarUsuario = eliminarUsuario;
        $scope.init();
    }

    angular
	.module('app')
	.controller('UsuariosController', UsuariosController);

    UsuariosController.$inject = ['$scope', '$log', 'toaster', '$uibModal', 'i18nService', 'UsuariosServices', 'EmpresaServices'];

})();

(function () {
    function ModalNewUptUsuario($scope,UsuariosServices,$uibModalInstance, usuario, operacion, empresas, roles) {
        $scope.usuario = angular.copy(usuario);
        $scope.empresas = empresas;
        $scope.roles = roles;
        $scope.aceptarVisible = true;
        $scope.disabled = false;
        $scope.title = '';
        $scope.mensaje = '';

        $scope.aceptar = aceptar;
        $scope.cancelar = cancelar;
        $scope.setSelect = setSelect;
        $scope.loadUsuarioDefault = loadUsuarioDefault;
        $scope.reqContr = false;
        // Reemplazo el submited del form por no tener acceso a el
        $scope.formCrtl = false;
        

        if (operacion == 'ver') {
            $scope.disabled = true;
            $scope.aceptarVisible = false;
            $scope.title = 'Ver usuario';
        } else if (operacion == 'agregar') {
            $scope.title = 'Crear un nuevo usuario';
            $scope.loadUsuarioDefault();
            $scope.reqContr = true;
        } else {
            $scope.title = 'Editar usuario';
        }

        function aceptar(form1, form2) {
            $scope.mensaje = '';
            if (operacion == 'ver') {
                alert('Operación Invalida');
            } else {
                // VALIDACIONES
                if (form1 && form2) {
                    if (operacion == "agregar") {
                        UsuariosServices.Create($scope.usuario)
                        .then(function (data) {
                            $uibModalInstance.close(data);
                        })
                        .catch(function (err) {
                            $scope.mensaje = 'Ocurrio un error al intentar crear el usuario, error: ' + err.exceptionMensaje;
                        });
                    } else if (operacion == "editar") {
                        UsuariosServices.Update($scope.usuario)
                        .then(function (data) {
                            $uibModalInstance.close(data);
                        })
                        .catch(function (err) {
                            $scope.mensaje = 'Ocurrio un error al intentar editar el usuario, error: ' + err.exceptionMensaje;
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
        function loadUsuarioDefault() {
            $scope.usuario = {                
                "nombres": "",
                "apellidos": "",
                "email": "",
                "telefono": "",
                "isActivo": true,
                "celular": "",
                "direccion": ""
            };
        }
        function setSelect() {
            //$scope.form
           
        }
        $scope.setSelect();
    }

    angular
	.module('app')
	.controller('ModalNewUptUsuario', ModalNewUptUsuario);

    ModalNewUptUsuario.$inject = ['$scope', 'UsuariosServices', '$uibModalInstance', 'usuario', 'operacion', 'empresas', 'roles'];

})();

(function () {
    function ModalEliminarUsuario($scope, $uibModalInstance, usuario) {
        $scope.usuario = usuario;
        $scope.aceptar = aceptar;
        $scope.cancelar = cancelar;

        function aceptar() {
            $uibModalInstance.close($scope.usuario);
        }
        function cancelar() {
            $uibModalInstance.dismiss('cancel');
        }
    }

    angular
	.module('app')
	.controller('ModalEliminarUsuario', ModalEliminarUsuario);

    ModalEliminarUsuario.$inject = ['$scope', '$uibModalInstance', 'usuario'];

})();