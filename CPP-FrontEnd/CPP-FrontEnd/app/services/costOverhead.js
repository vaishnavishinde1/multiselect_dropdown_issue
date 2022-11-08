'use strict';

angular.module('cpp.services').
    factory('CostOverhead', function ($resource) {
        console.log('test luan');
        return $resource(serviceBasePath + "Request/CostOverhead");
    }).

    factory('getCostOverheadHistory', function ($resource) {

        return $resource(serviceBasePath + "CostOverheadHistory/getCostOverheadHistory");

    }).
    factory('UpdateCostOverhead', function ($resource) {
        return $resource(serviceBasePath + "Response/CostOverhead");
    });