purchaseOrderModule.controller('purchaseOrderController', ['$scope', '$http', '$timeout', 'ViewModel',
    function ($scope, $http, $timeout, viewModel) {

        $scope.viewModel = viewModel;
        

    }
]);

purchaseOrderModule.controller('purchaseOrderContentController', ['$scope', '$http', '$timeout', 'ViewModel',
    function ($scope, $http, $timeout, viewModel) {

        $scope.viewModel = viewModel;
        $scope.viewModel.Product.Image = "../../Content/images/pagemaker.png";

    }
]);