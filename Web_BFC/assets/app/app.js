var meses = [
              "enero",
              "febrero",
              "marzo",
              "abril",
              "mayo",
              "junio",
              "julio",
              "agosto",
              "septiembre",
              "octubre",
              "noviembre",
              "diciembre"
];


/**************** CONFIGURACION */
//var URL_API = 'http://localhost:41713/'; OLD
//var URL_API = 'http://localhost:55862/';
var URL_API = 'http://localhost:49391/';
/*if (miIP == '186.67.10.220') {
    URL_API = 'http://192.168.0.105/';
}else{
    URL_API='http://localhost:49391/';
}
//var URL_API = 'http://localhost:12345/';
/**************** CONFIGURACION FIN */
function isEmpty(value) {
    return angular.isUndefined(value) || value === '' || value === null || value !== value;
}
(function () {
    'use strict';    
    angular
        .module('app',
	    [
        //'ngTouch',
        'LocalStorageModule',
        'ui.router',
        'ngSanitize',
        'ngAnimate',
        'ngMessages',
        'ui.bootstrap',
        'ui.bootstrap.datetimepicker',
        'toaster',

        'ui.select',
        'ui.grid',
        'ui.grid.resizeColumns',
        'ui.grid.exporter',
        'ui.grid.pagination',
        //'ui.grid.edit',
        'pascalprecht.translate',
        'cgBusy',// verificar si se usa o no
        'angular-button-spinner',
        'cfp.loadingBar',
        'angular-jwt',
        'ui.grid.grouping'
	    ]
	)

    .constant('ngAuthSettings', {
        apiServiceBaseUri: URL_API,
        clientId: 'ngAuthApp'
    })
    .config(function ($locationProvider, $urlRouterProvider, $stateProvider, $httpProvider, $translateProvider) {
        //$translateProvider.translations('es', { APP_HEADLINE:  'Großartige AngularJS App'});

        $translateProvider.preferredLanguage('es');
        $locationProvider.html5Mode(false);
        $urlRouterProvider.otherwise("/login");

        $stateProvider
            .state('articulos', {
                url: "/articulos",
                views: {
                    "content": { templateUrl: "assets/app/articulo/articuloList.html" },
                    "menu": { templateUrl: "assets/app/menu/menu.html" }
                }
            })
            .state('consultacobros', {
                url: "/consultacobros",
                views: {
                    "content": { templateUrl: "assets/app/cobros/consultacobros.html" },
                    "menu": { templateUrl: "assets/app/menu/menu.html" }
                }
            })
            .state('cobroservicios', {
                url: "/cobroservicios",
                views: {
                    "content": { templateUrl: "assets/app/cobros/cobroservicios.html" },
                    "menu": { templateUrl: "assets/app/menu/menu.html" }
                }
            })
            .state('cobroalmacenamiento', {
                url: "/cobroalmacenamiento",
                views: {
                    "content": { templateUrl: "assets/app/cobros/cobroalmacenamiento.html" },
                    "menu": { templateUrl: "assets/app/menu/menu.html" }
                }
            })
            .state('mantenedoruf', {
                url: "/mantenedoruf",
                views: {
                    "content": { templateUrl: "assets/app/cobros/mantenedoruf.html" },
                    "menu": { templateUrl: "assets/app/menu/menu.html" }
                }
            })
            .state('mantenedorcobros', {
                url: "/mantenedorcobros",
                views: {
                    "content": { templateUrl: "assets/app/cobros/mantenedorcobros.html" },
                    "menu": { templateUrl: "assets/app/menu/menu.html" }
                }
            })
            .state('inicio', {
                url: "/inicio",
                views: {
                    "content": { templateUrl: "assets/app/inicio/inicio.html" },
                    "menu": { templateUrl: "assets/app/menu/menu.html" }
                }
            })
            .state('login', {
                url: "/login",
                views: {
                    "content": { templateUrl: "assets/app/login/login.html" },
                    "menu": { templateUrl: "assets/app/menu/menu.html" }
                }
            })
            .state('stockconsulta', {
                url: "/stockconsulta",
                views: {
                    "content": { templateUrl: "assets/app/stock/stockconsulta.html" },
                    "menu": { templateUrl: "assets/app/menu/menu.html" }
                }
            })
            .state('stockentradassalidas', {
                url: "/stockentradassalidas",
                views: {
                    "content": { templateUrl: "assets/app/stock/stockentsal.html" },
                    "menu": { templateUrl: "assets/app/menu/menu.html" }
                }
            })
            .state('stocklistadoentradassalidas', {
                url: "/stocklistadoentradassalidas",
                views: {
                    "content": { templateUrl: "assets/app/stock/stocklistadoentradassalidas.html" },
                    "menu": { templateUrl: "assets/app/menu/menu.html" }
                }
            })
            .state('stockmovimiento', {
                url: "/stockmovimiento",
                views: {
                    "content": { templateUrl: "assets/app/stock/stockmovimiento.html" },
                    "menu": { templateUrl: "assets/app/menu/menu.html" }
                }
            })
            .state('usuarios', {
                url: "/usuarios",
                views: {
                    "content": { templateUrl: "assets/app/usuario/usuario.html" },
                    "menu": { templateUrl: "assets/app/menu/menu.html" }
                }
            })
            .state('no-enrutar', {
                url: "/no-enrutar",
                template: "<p>No enrutar</p>"
            })
            .state('logout', {
                url: "/no-logout",
                templateUrl: "assets/app/logout/logout.html"
            });
        //if (!$httpProvider.defaults.headers.get) $httpProvider.defaults.headers.get = {};

        //$httpProvider.defaults.headers.get['Cache-Control'] = 'no-cache';
        //$httpProvider.defaults.headers.get['Pragma'] = 'no-cache';
        $httpProvider.interceptors.push('authInterceptorService');
    })
    .run(['$state', '$rootScope', 'authService', '$log', 'cfpLoadingBar', '$timeout', 'i18nService',
        function ($state, $rootScope, authService, $log, cfpLoadingBar, $timeout, i18nService) {
            i18nService.setCurrentLang('es');
            authService.fillAuthData();

            $rootScope.$on('$stateChangeStart',
                function (event, toState, toParams, fromState, fromParams) {
                    if (toState.name == 'no-enrutar') {
                        event.preventDefault();
                    } else {
                        cfpLoadingBar.start();
                    }


                })
            $rootScope.$on('$stateChangeSuccess', function () {

                $timeout(function () {
                    cfpLoadingBar.complete();

                }, 250);
            })
        }])
    .directive('onlyNumbers', function () {
        return {
            restrict: 'A',
            link: function (scope, elm, attrs, ctrl) {
                elm.on('keydown', function (event) {
                    //var tecla = String.fromCharCode(event.which).toLowerCase();
                    if (event.which == 190 || event.which == 110) {
                        if (elm[0].value.length == 0) {
                            return false;
                        } else if (elm[0].value.indexOf('.') > -1) {
                            return false;
                        }
                    } else if (event.which == 64 || event.which == 16) {
                        // to allow numbers  
                        return false;
                    } else if (event.which >= 48 && event.which <= 57) {
                        // to allow numbers  
                        return true;
                    } else if (event.which >= 96 && event.which <= 105) {
                        // to allow numpad number  
                        return true;
                    } else if ([8, 9, 13, 27, 37, 38, 39, 40].indexOf(event.which) > -1) {
                        // to allow backspace, Tab, enter, escape, arrows  
                        return true;
                    } else {
                        event.preventDefault();
                        // to stop others  
                        return false;
                    }
                });
            }
        }
    })
    .directive('onlyInterger', function () {
        return {
            restrict: 'A',
            link: function (scope, elm, attrs, ctrl) {
                elm.on('keydown', function (event) {
                    //var tecla = String.fromCharCode(event.which).toLowerCase();
                    //if (tecla == "¾") {
                    //    if (elm[0].value.length == 0) {
                    //        return false;
                    //    } else if (elm[0].value.indexOf('.') > -1) {
                    //        return false;
                    //    }
                    //} else
                    if (event.which == 64 || event.which == 16) {
                        // to allow numbers  
                        return false;
                    } else if (event.which >= 48 && event.which <= 57) {
                        // to allow numbers  
                        return true;
                    } else if (event.which >= 96 && event.which <= 105) {
                        // to allow numpad number  
                        return true;
                    } else if ([8, 9, 13, 27, 37, 38, 39, 40].indexOf(event.which) > -1) {
                        // to allow backspace, Tab, enter, escape, arrows  
                        return true;
                    } else {
                        event.preventDefault();
                        // to stop others  
                        return false;
                    }
                });
            }
        }
    })
    .directive('ngMin', function () {
        return {
            restrict: 'A',
            require: 'ngModel',
            link: function (scope, elem, attr, ctrl) {
                scope.$watch(attr.ngMin, function () {
                    ctrl.$setViewValue(ctrl.$viewValue);
                });
                var minValidator = function (value) {
                    var min = Number(attr.ngMin || 0) || 0;
                    if (!isEmpty(value) && value < min) {
                        ctrl.$setValidity('ngMin', false);
                        return undefined;
                    } else {
                        ctrl.$setValidity('ngMin', true);
                        return value;
                    }
                };

                ctrl.$parsers.push(minValidator);
                ctrl.$formatters.push(minValidator);
            }
        };
    })
    .filter('mapDocumentoExistencia', function () {
        var DocHash = {
            38: 'Ingreso',
            42: 'Salida'
        };

        return function (input) {
            var result;
            var match;
            if (!input) {
                return '';
            } else if (result = DocHash[input]) {
                return result;
            } else if ((match = input.match(/(.+)( \(\d+\))/)) && (result = DocHash[match[1]])) {
                return result + match[2];
            } else {
                return input;
            }
        };
    });
    
})();