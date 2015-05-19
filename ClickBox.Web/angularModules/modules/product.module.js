//var productModule = angular.module("productModule", []);

productModule.controller('productController', ['$scope', '$http', '$timeout' , 'Products', function ($scope, $http, $timout, products) {

    $scope.Products = products;

    $scope.cancel = function() {
        $scope.selectedProduct = undefined;
        $scope.newProduct = undefined;
    }

    $scope.saveSelectedProduct = function () {
        console.log("save the selected product", $scope.selectedProduct);
        $http.post('/product/edit', $scope.selectedProduct).
          success(function (data, status, headers, config) {
              console.log('save by api',data);
              
              $timout(function () {
                  $scope.selectedProduct = undefined;
              }, 500);

          }).
          error(function (data, status, headers, config) {
          });
    }

    $scope.showProductDetails = function(product) {
        console.log("show details for:", product);
        var selected = _.filter($scope.Products, function(p) {
            return p.Id == product.product;
        });
        $scope.newProduct = undefined;
        $scope.selectedProduct = selected[0];
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
          });
    }

    $scope.saveNewProduct = function() {
        $http.post('/product/create', $scope.newProduct).
          success(function (data, status, headers, config) {
                console.log(data);
                $scope.newProduct = undefined;
                $scope.selectedProduct = undefined;
                $timout(function() {
                    $scope.Products = data.Products;
                }, 500);

            }).
          error(function (data, status, headers, config) {
          });
    }

    console.log("The products scope", $scope);
}]);