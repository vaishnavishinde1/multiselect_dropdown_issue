'use strict';

angular.module('cpp.services').
    factory('AdminApproval', function ($resource) {
        return $resource(serviceBasePath + "Request/adminApproval");
    }).
    factory('ApprovalPresidentAndHistory', function ($resource) {
        return $resource(serviceBasePath + "adminApprovalPresident/getApprovalPresidentAndHistory");
    }).
    factory('UpdateAdminApproval', function ($resource) {
        return $resource(serviceBasePath + "Response/adminApproval");
    });
