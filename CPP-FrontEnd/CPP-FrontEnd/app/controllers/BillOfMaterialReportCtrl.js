angular.module('cpp.controllers').
    controller('BillOfMaterialReportCtrl', ['$scope', '$timeout', '$uibModal', '$rootScope', '$http', 'Program', 'ProgramElement', 'Page', 'Project', 'Trend', 'MaterialCategory', 'SubcontractorType', 'fteposition', 'Organization', 'PhaseCode', 'ProjectClass', 'ProjectClassPhase', 'VersionDetails', 'Category',
        function ($scope, $timeout, $uibModal, $rootScope, $http, Program, ProgramElement, Page, Project, Trend, MaterialCategory, SubcontractorType, fteposition, Organization, PhaseCode, ProjectClass, ProjectClassPhase, VersionDetails, Category) {
            var okToExit = true;
            Page.setTitle('Clients');

            $scope.gridOPtions = {};
            $scope.myExternalScope = $scope;

            console.log('luan test');
            var now = new Date();
            var aa = now.toString();
            var ee = moment(aa).format("YYYY-MM-DD[T]00:00:00");
            var new_date = (moment(ee).add(10, 'd').format('YYYY-MM-DD[T]00:00:00'));
            var OrganizationID = 75;
            $scope.Statuses = [];
            $scope.Statuses.push({ ID: "Pending", value: "Pending" }, { ID: "Order Placed", value: "Order Placed" }, { ID: "Partially Received", value: "Partially Received" },
                { ID: "Fully Received", value: "Fully Received" });
            Program.lookup().get({ OrganizationID: OrganizationID }, function (response) {
                console.log(response);
                $scope.allProgramList = response.result;
                $scope.filterContract = $scope.allProgramList[0];
                fetchdataforproject($scope.filterContract.ProgramID);

            });

            $scope.filterChangeProject = function () {
                fetchdataforproject($scope.filterContract.ProgramID);
            }

            $scope.filterProjectById = function () {

                PerformOperationOnBillofMaterialgrid($scope.filterProject.ProgramElementID);


            }


            function PerformOperationOnBillofMaterialgrid(ProgramElementID) {

                $http({
                    url: serviceBasePath + 'Request/BillsOfMateralList?ProgramElementID=' + ProgramElementID,
                    method: "GET",
                    headers: { 'Content-Type': 'application/json' }
                }).then(function success(response) {

                    $scope.billofmateriallist = response.data.data;
                    $scope.orgbillofmateriallist = angular.copy($scope.billofmateriallist);
                    gridloadforbillofmaterial();

                }, function error(response) {
                    dhtmlx.alert("Failed to save. Please contact your Administrator.");
                });
            }

            function gridloadforbillofmaterial() {
                angular.forEach($scope.billofmateriallist, function (item, index) {
                   
                    for (var x = 0; x < $scope.Statuses.length; x++) {
                        if (item.Status == $scope.Statuses[x].ID) {
                            item.Status = item.Status;
                        }
                    }
                });

                $scope.gridOptions.data = $scope.billofmateriallist;
            }

            function fetchdataforproject(ProgramID) {
                ProgramElement.lookup().get({ ProgramID: $scope.filterContract.ProgramID }, function (response) {
                    $scope.allProgramElementList = response.result;
                    $scope.filterProject = $scope.allProgramElementList[0];
                    if ($scope.filterProject != undefined) {
                        PerformOperationOnBillofMaterialgrid($scope.filterProject.ProgramElementID);
                    }
                    if ($scope.filterProject == undefined) {
                        $scope.gridOptions.data = [];
                    }
                });


            }

            $scope.cellInputEditableTemplate = '<input ng-class="\'colt\' + col.index" ng-input="COL_FIELD" ng-model="COL_FIELD" ng-blur="updateEntity(row)" />';
            //$scope.cellSelectEditableTemplate = '<select ng-class="\'colt\' + col.index" ng-input="COL_FIELD" ng-model="COL_FIELD" ng-click = "test(COL_FIELD)" ng-options="phase.Code for phase in phaseCodeCollection" />';
            $scope.cellCheckEditTableTemplate = '<input class="c-approval-matrix-check"  type="checkbox" ng-input="COL-FIELD" ng-model="COL_FIELD"/>';
            $scope.cellCheckEditTableTemplateApplicable = '<input class="c-approval-matrix-check"  type="checkbox" ng-input="COL-FIELD" ng-model="COL_FIELD"/>';
            //$scope.cellSelectEditableTemplateProjectType = '<select ng-class="\'colt\' + col.index" ng-input="COL_FIELD" ng-model="COL_FIELD" style="width:120px;" ng-options="id.Type for id in phaseCodeCollection" ng-blur="updateEntity(row)" />';

            $scope.gridOptions = {
                enableColumnMenus: false,
                enableCellEditOnFocus: false,
                enableSorting: false,
                rowHeight: 40,
                width: 700,
                columnDefs: [{
                    field: 'MaterialName',
                    name: 'Material Name',
                    width: 270,
                    enableCellEdit: false,
                    headerCellClass: 'text-c'
                   
                   
                }, {
                    field: 'UnitCostStartDate',
                    name: 'Start Date',  //Manasi
                    width: 160,
                    enableCellEdit: false,
                    type: 'date',
                    cellFilter: 'date:"MM/dd/yyyy"',
                    headerCellClass: 'text-c'
                 
                    },
                    {
                        field: 'UnitCostEndDate',
                        name: 'End Date',  //Manasi
                        width: 160,
                        enableCellEdit: false,
                        type: 'date',
                        cellFilter: 'date:"MM/dd/yyyy"',
                        headerCellClass: 'text-c'

                    },
                    {
                    field: 'UnitQuantity',
                    name: 'Unit Quantity',
                    width: 170,
                        enableCellEdit: false,
                        headerCellClass: 'text-c',
                    cellClass: 'c-col-Num',
                    
                    },
                    {
                    field: 'Make_Available_by',
                    //name:'SubCategory Id',
                    name: 'Make Available by',
                        width: 200,
                        headerCellClass: 'text-c',
                        enableCellEdit: false,
                        type: 'date',
                        cellFilter: 'date:"MM/dd/yyyy"',
                        cellClass: function (grid, row, col, rowRenderIndex, colRenderIndex) {
                            if (grid.getCellValue(row, col) >= ee && grid.getCellValue(row, col) < new_date) {
                                return 'red';
                            }
                        }
                    },
                    {
                        field: 'Status',
                        name: 'Status',
                        editableCellTemplate: 'ui-grid/dropdownEditor',
                        editDropdownIdLabel: 'ID',
                        editDropdownValueLabel: 'value',
                        editDropdownOptionsArray: $scope.Statuses,
                        cellFilter: 'customFilter:this',
                        width: 150,
                        headerCellClass: 'text-c'
                    },
                    {
                        field: 'Note',
                        //name:'SubCategory Id',
                        name: 'Note',
                        width: 350,
                        headerCellClass: 'text-c'
                    }
               
              
                ]
            }

            $scope.SubmitBillsOfMaterial = function () {
              
                var listToSave = [];
                isnotefilled = false;
                angular.forEach($scope.billofmateriallist, function (value, key, obj) {
                    isChanged = true;
                    if (value.Status == "Partially Received") {
                        if (value.Note == null) {
                            dhtmlx.alert('Note is Required Field when Status is Partially Received');
                            isnotefilled = true;
                            return;
                        }
                    }
                    angular.forEach($scope.orgbillofmateriallist, function (orgItem) {
                        if (value.Id === orgItem.Id &&
                            value.Status === orgItem.Status &&
                                value.Note === orgItem.Note
                          ) {
                            isChanged = false;
                            okToExit = true;
                        }

                    });
                    if (isChanged == true) {
                        var dataObj = {
                            Operation: '2',
                            Id: value.Id,
                            Note: value.Note,
                            Status: value.Status
                        }
                    }
                    else {
                        var dataObj = {
                            Operation: '4',
                            Id: value.Id,
                            Note: value.Note,
                            Status: value.Status
                        }
                    }
                    listToSave.push(dataObj)
                    okToExit = true;
                });

                if (isnotefilled) {
                    return;
                }

                $http({
                    url: serviceBasePath + 'Response/BillsOfMateralList',
                    method: "POST",
                    data: JSON.stringify(listToSave),
                    headers: { 'Content-Type': 'application/json' }
                }).then(function success(response) {
                    if (response.data.result) {
                        dhtmlx.alert(response.data.result);
                    }
                    else {
                        dhtmlx.alert('No changes to be saved.');
                    }

                }, function error(response) {
                    dhtmlx.alert("Failed to save. Please contact your Administrator.");
                });
               
                }
            $('.modal-backdrop').hide();

        }
    ]);