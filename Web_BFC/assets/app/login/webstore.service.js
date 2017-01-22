(function () {
    'use strict';
    function WebStorageService($window) {
        function get(store, key) {
            switch (store) {
                case "ls":
                    return $window.JSON.parse($window.localStorage.getItem(key));
                    break;
                case "ss":
                    return $window.JSON.parse($window.sessionStorage.getItem(key));
                    break;                
            }
        }
        function set(store, key, value) {
            switch (store) {
                case "ls":
                    $window.localStorage.setItem(key, $window.JSON.stringify(value));
                    break;
                case "ss":
                    $window.sessionStorage.setItem(key, $window.JSON.stringify(value));
                    break;
            }
        }
        function remove(store, key) {
            switch (store) {
                case "ls":
                    $window.localStorage.removeItem(key);
                    break;
                case "ss":
                    $window.sessionStorage.removeItem(key);
                    break;
            }
        }

        this.get = get;
        this.set = set;
        this.remove = remove;
    }
    angular
      .module('app')
      .service('WebStorageService', WebStorageService);
    
    WebStorageService.$inject = ['$window'];
})();