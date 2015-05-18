//var productModule = angular.module("productModule", []);

productModule.controller('productController', ['$scope', 'Products', function ($scope, products) {

    $scope.Products = products;

    $scope.showProductDetails = function(id) {
        console.log("show details for:", id);
        var selected = _.first($scope.Products, function(p) {
            return p.Id === id;
        });
        $scope.selectedProduct = selected;
    }

    console.log("The products scope", $scope);


}]);