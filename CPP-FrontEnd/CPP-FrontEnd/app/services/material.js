'use strict';

angular.module('cpp.services').
    factory('Material', function ($resource) {
        return $resource(serviceBasePath + "Request/Material");
    }).

    factory('getMaterialHistory', function ($resource) {

        return $resource(serviceBasePath + "MaterialHistory/getMaterialHistory");  

    }).


    factory('UpdateMaterial', function ($resource) {
        return $resource(serviceBasePath + "Response/Material");
    });