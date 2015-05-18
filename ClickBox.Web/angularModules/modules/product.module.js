//var productModule = angular.module("productModule", []);

productModule.controller('productController', ['$scope', '$http', '$timeout' , 'Products', function ($scope, $http, $timout, products) {

    $scope.Products = products;

    $scope.cancel = function() {
        $scope.selectedProduct = undefined;
        $scope.newProduct = undefined;
    }

    $scope.saveSelectedProduct = function () {
        console.log("save the selected product", $scope.selectedProduct);
        $scope.selectedProduct = undefined;
    }

    $scope.showProductDetails = function(id) {
        console.log("show details for:", id);
        var selected = _.first($scope.Products, function(p) {
            return p.Id === id;
        });
        $scope.newProduct = undefined;
        $scope.selectedProduct = selected;
    }

    $scope.addNewProduct = function () {
        $scope.selectedProduct = undefined;

        $http.get('/product/NewProductId').
          success(function (data, status, headers, config) {
              $scope.newProduct = {
                  Id: data,
                  Name: undefined,
                  PrivateKey: undefined,
                  PublicKey: undefined
              };
          }).
          error(function (data, status, headers, config) {
              // called asynchronously if an error occurs
              // or server returns response with an error status.
          });
    }

    $scope.saveNewProduct = function() {
        $http.post('/product/create', $scope.newProduct).
          success(function (data, status, headers, config) {
              // this callback will be called asynchronously
              // when the response is available
                console.log(data);
                $scope.newProduct = undefined;
                $scope.selectedProduct = undefined;
                $timout(function() {
                    $scope.Products = data.Products;
                }, 500);

            }).
          error(function (data, status, headers, config) {
              // called asynchronously if an error occurs
              // or server returns response with an error status.
          });
    }

    console.log("The products scope", $scope);
}]);