angular.module('cpp.controllers').
    //adminApproval Controller

    controller('AdminApprovalCtrl', ['$state', '$http', '$uibModal', 'AdminApproval', 'ApprovalPresidentAndHistory', '$rootScope', '$scope', 'Page', 'User', 'ProjectTitle', 'TrendStatus', '$location', 'AllEmployee', 'ProjectClass', '$timeout',
        function ($state, $http, $uibModal, AdminApproval, ApprovalPresidentAndHistory, $rootScope, $scope, Page, User, ProjectTitle, TrendStatus, $location, AllEmployee, ProjectClass, $timeout) {
            Page.setTitle('Trend Approvers');
            ProjectTitle.setTitle('');
            TrendStatus.setStatus('');
            var url = serviceBasePath + "Response/adminApproval";
            var empArrayDept = [];
            var empArrayOper = [];
            var dptArray = []; //Narayan - 03/08/22 
            $scope.rolesArrayList = [];
            $scope.rolesArray = [];
            $scope.example14settings = {
                scrollableHeight: '100px', scrollable: true,
                enableSearch: false
            };
            $scope.listOfDeptManagers = [];
            $scope.listOfOpeManagers = [];


            function getAllApproversGrid() {
                AdminApproval.get({}, function (approvers) {
                    $scope.checkList = [];


                    // Narayan - 03/08/22 - Get all department api call
                    ProjectClass.get({}, function (departments) {
                        $scope.departmentsData = departments.result;
                        console.log(departments.result);

                        angular.forEach($scope.departmentsData, function (department) {
                            department.DepartmentName = department.ProjectClassName;
                        });


                        angular.forEach(departments.result, function (item) {
                            //$scope.positionArray.push({ ID: item.Id, value: item.PositionDescription });
                            //$rootScope.positionArray.push({ ID: item.Id, value: item.PositionDescription });
                            dptArray.push({ ID: item.ProjectClassID, value: item.ProjectClassName });
                        });


                        console.log($scope.departmentsData);


                        console.log(dptArray);
                        console.log(User);

                        User.get({}, function (Users) {
                            var UserList = Users.result;
                            var listOfPresident = [];
                            let deptManagers = [];
                            let opeManagers = [];

                            deptManagers = UserList.filter(user => user.RoleList.includes("Department Manager"));
                            opeManagers = UserList.filter(user => user.RoleList.includes("Operations Manager"));

                            for (u in UserList) {
                                if (UserList[u].RoleList.length > 0) {
                                    for (r in UserList[u].RoleList)
                                        if (UserList[u].RoleList[r] == 'President') {
                                            UserList[u].UserFullName = UserList[u].FirstName + " " + UserList[u].LastName;
                                            listOfPresident.push(UserList[u]);
                                            break;
                                        }
                                }
                            }
                            $scope.presidentList = listOfPresident;

                            $scope.listOfDeptManagers = deptManagers;
                            $scope.listOfOpeManagers = opeManagers;

                            for (p in listOfPresident) {
                                if (listOfPresident[p].EmployeeID == $scope.approvalPresidentData.PresidentID) {
                                    $scope.filterpresident = listOfPresident[p];
                                    break;
                                }
                            }

                            //$('#selectPresident').val(listOfPresident[1].EmployeeID);



                            $scope.approversCollection = approvers.result;
                            //angular.forEach($scope.userCollection, function (user, key) {
                            //    user.status = 'Unchanged';
                            //});
                            console.log(approvers.result);
                            $scope.originalApprovalsCollection = angular.copy(approvers.result);
                            addIndex($scope.approversCollection);

                            angular.forEach($scope.approversCollection, function (item, index) {
                                //item.Cost = toString(item.Cost);
                                item.checkbox = false;
                                $scope.checkList[index + 1] = false;

                                //Find Department
                                for (var i = 0; i < $scope.departmentsData.length; i++) {
                                    if ($scope.departmentsData[i].ProjectClassID == item.DepartmentID) {
                                        item.Department = $scope.departmentsData[i].ProjectClassID;
                                        break;
                                    }
                                }

                                //Find employee for deprtment manager
                                for (var x = 0; x < $scope.listOfDeptManagers.length; x++) {
                                    if ($scope.listOfDeptManagers[x].EmployeeID == item.DeptManagerID) {
                                        item.DepartmentManager = $scope.listOfDeptManagers[x].FirstName + " " + $scope.listOfDeptManagers[x].LastName;
                                        break;
                                    }
                                }

                                //Find employee for operation manager
                                for (var x = 0; x < $scope.listOfOpeManagers.length; x++) {
                                    if ($scope.listOfOpeManagers[x].EmployeeID == item.OpeManagerID) {
                                        item.OperationsManager = $scope.listOfOpeManagers[x].FirstName + " " + $scope.listOfOpeManagers[x].LastName;
                                        break;
                                    }
                                }


                                let deptWiseDeptManager = [];
                                let deptWiseOpeManager = [];
                                let deptManagerDD = [];
                                let opeManagerDD = [];

                                deptWiseDeptManager = $scope.listOfDeptManagers.filter(user => user.DepartmentID == item.DepartmentID);
                                deptWiseOpeManager = $scope.listOfOpeManagers.filter(user => user.DepartmentID == item.DepartmentID);

                                angular.forEach(deptWiseDeptManager, function (user) {
                                    deptManagerDD.push({ ID: user.EmployeeID, Value: user.FirstName + " " + user.LastName });
                                });
                                angular.forEach(deptWiseOpeManager, function (user) {
                                    opeManagerDD.push({ ID: user.EmployeeID, Value: user.FirstName + " " + user.LastName });
                                });

                                item.departmentManagerOption = deptManagerDD;
                                item.operationManagerOption = opeManagerDD;


                                item.DeptManagerID = item.DeptManagerID;
                                item.OpeManagerID = item.OpeManagerID;

                            });
                            console.log($scope.approvalMatrixCollection);
                            $scope.gridOptions.data = $scope.approversCollection;

                        });

                    });

                });
            }

            function getPresidentAndHistory() {
                ApprovalPresidentAndHistory.get({}, function (data) {
                    $scope.approvalPresidentData = data.presidentData;
                    $scope.approversHistory = data.approvalHistory;
                });
            }

            getPresidentAndHistory();
            getAllApproversGrid();


            if (!$('body').children('#ApprovalHistoryModal').attr('id')) {
                $('#ApprovalHistoryModal').appendTo('body');
            }
            $('.main-content-view').find('#ApprovalHistoryModal').remove();


            $scope.showHistory = function () {
                //alert("hey!");
                getPresidentAndHistory();

                var gridTableHistoryData = $('#tblApprovalHistoryGrid tbody');
                var historyData = $scope.approversHistory;
                gridTableHistoryData.empty();
                
                for (var a = 0; a < historyData.length; a++) {
                    gridTableHistoryData.append('<tr class="contact-row" id="' + historyData[a].Id + '">' +
                        '<td style=" overflow: hidden; text-overflow: ellipsis; white-space: nowrap;"' +
                        '><a>' + (a + 1) + '</a></td> ' +
                        '<td style=" overflow: hidden; text-overflow: ellipsis; white-space: nowrap;"' +
                        '>' + historyData[a].Department + '</td > ' +
                        '<td style=" overflow: hidden; text-overflow: ellipsis; white-space: nowrap;"' +
                        '>' + historyData[a].DepartmentManager + '</td > ' +
                        '<td style=" overflow: hidden; text-overflow: ellipsis; white-space: nowrap;"' +
                        '>' + historyData[a].OperationManager + '</td > ' +
                        '<td style=" overflow: hidden; text-overflow: ellipsis; white-space: nowrap;"' +
                        '>' + historyData[a].UpdatedBy + '</td>' +
                        '<td style=" overflow: hidden; text-overflow: ellipsis; white-space: nowrap;"' +
                        '>' + moment(historyData[a].FromDate).format('MM/DD/YYYY') + '</td>' +
                        '<td style=" overflow: hidden; text-overflow: ellipsis; white-space: nowrap;"' +
                        '>' + moment(historyData[a].ToDate).format('MM/DD/YYYY') + '</td>' +
                        '<tr > ');
                }

                $('#ApprovalHistoryModal').modal({ show: true, backdrop: 'static' });
            }

            //$scope.closeApprovalModal = function () {
            $('#closeApprovalHistoryModal').unbind('click').on('click', function () {

                $("#ApprovalHistoryModal").modal('toggle');

            });
            

            var addIndex = function (data) {
                var i = 1;
                angular.forEach(data, function (value, key, obj) {
                    value.displayId = i;
                    i = i + 1;
                    if (value.Schedule === "0001-01-01T00:00:00") {
                        value.Schedule = "";
                    }
                });
            }
            //$scope.cellInputEditableTemplate = '<input ng-class="\'colt\' + col.index" ng-input="COL_FIELD" ng-model="COL_FIELD" ng-blur="updateEntity(row)" />';
            $scope.mySelections = [];
            $scope.test = function (test) {
                console.log(test);
            }
            $scope.cellInputEditableTemplate = '<input ng-class="\'colt\' + col.index" ng-input="COL_FIELD" ng-model="COL_FIELD" ng-blur="updateEntity(row)" />';
            /* $scope.cellSelectEditableTemplate = '<select ng-class="\'colt\' + col.index" ui-grid-edit-dropdown ng-input="COL_FIELD" ng-change="checkRole(COL_FIELD)" ng-model="COL_FIELD" ng-options="role.Role for role in RoleCollection" />';*/
            $scope.cellSelectEditableTemplate = '<select ng-class=\"\'colt\' + col.uid\"  ui-grid-edit-dropdown ng-model=\"MODEL_COL_FIELD\" ng-options=\"field[editDropdownIdLabel] as field[editDropdownValueLabel] CUSTOM_FILTERS for field in editDropdownOptionsArray\"></select>';
            $scope.cellCheckEditTableTemplate = '<input class="c-approval-matrix-check"  type="checkbox" ng-input="COL-FIELD" ng-model="COL_FIELD"/>';
            $scope.addRow = function () {
                var x = Math.max.apply(Math, $scope.approversCollection.map(function (o) {

                    return o.displayId;
                }))

                if (x < 0) {
                    console.log(x);
                    x = 0;
                }

                $scope.checkList[++x] = false;
                $scope.approversCollection.splice(x, 0, {
                    displayId: x,
                    Department: '',
                    DepartmentManager: '',
                    OperationsManager: '',
                    checkbox: false,
                    new: true
                });

                console.log($scope.approversCollections);
                $scope.gridApi.core.clearAllFilters();//Nivedita-T on 17/11/2021
                $timeout(function () {

                    $scope.gridApi.core.scrollTo($scope.gridOptions.data[$scope.gridOptions.data.length - 1], $scope.gridOptions.columnDefs[0]);
                }, 1);
            }

            $scope.gridOptions = {
                enableColumnMenus: false,
                enableCellEditOnFocus: true,
                enableFiltering: true,
                enableAutoFitColumns: false,
                rowHeight: 40,
                columnDefs: [
                    {
                        field: 'displayId',
                        name: 'ID',
                        enableCellEdit: false,
                        width: 50,
                        cellClass: 'c-col-Num' //Manasi

                    },

                    {
                        field: 'Department',
                        name: 'Department*',
                        editableCellTemplate: 'ui-grid/dropdownEditor',
                        //editDropdownValueLabel: 'ProjectClassName',
                        //editDropdownIdLabel: 'ProjectClassName',
                        editDropdownIdLabel: 'ID',
                        editDropdownValueLabel: 'value',
                        editDropdownOptionsArray: dptArray,
                        //editDropDownChange: 'test',
                        /* enableCellEditOnFocus: true,
                         editableCellTemplate: $scope.cellSelectEditableTemplate,*/
                        cellFilter: 'customFilter:this',
                        cellClass: 'c-col-Num',
                        width: 330

                    },

                    {
                        field: 'DepartmentManager',
                        name: 'Department Manager*',
                        editableCellTemplate: 'ui-grid/dropdownEditor',
                        editDropdownIdLabel: 'ID',
                        editDropdownValueLabel: 'Value',
                        //editDropdownOptionsArray: dptArray,
                        editDropdownRowEntityOptionsArrayPath: 'departmentManagerOption',
                        //editDropDownChange: 'test',
                        /* enableCellEditOnFocus: true,
                         editableCellTemplate: $scope.cellSelectEditableTemplate,*/
                        cellFilter: 'customFilter:this',
                        cellClass: 'c-col-Num',
                        width: 300

                    },

                    {
                        field: 'OperationsManager',
                        name: 'Operations Manager*',
                        editableCellTemplate: 'ui-grid/dropdownEditor',
                        editDropdownIdLabel: 'ID',
                        editDropdownValueLabel: 'Value',
                        //editDropdownOptionsArray: dptArray,
                        editDropdownRowEntityOptionsArrayPath: 'operationManagerOption',
                        //editDropDownChange: 'test',
                        /* enableCellEditOnFocus: true,
                         editableCellTemplate: $scope.cellSelectEditableTemplate,*/
                        cellFilter: 'customFilter:this',
                        cellClass: 'c-col-Num',
                        width: 300

                    },

                    {
                        field: 'checkBox',
                        displayName: '',
                        enableCellEdit: false,
                        enableFiltering: false,
                        width: 50,
                        cellTemplate: '<input id="checkboxId[row.entity.displayId]" ng-model="checkList[row.entity.displayId]" type="checkbox" class = "c-col-check" ng-click="grid.appScope.check(row,col)" style="text-align: center;vertical-align: middle;">'

                    }

                ]
            }
            $scope.gridOptions.onRegisterApi = function (gridApi) {
                $scope.gridApi = gridApi;

                $scope.gridApi.edit.on.beginCellEdit($scope, function (rowEntity, colDef) {
                    $('div.ui-grid-cell form').find('input').css('height', '40px');
                    $('div.ui-grid-cell form').find('select').css('height', '40px');
                    $('div.ui-grid-cell form').css('margin', '0px');
                    //$('div.ui-grid-cell form').find('ui-select').putCursorAtEnd();

                    //$('div.ui-grid-cell form').find('input').putCursorAtEnd();
                    //$('div.ui-grid-cell form').find('select').putCursorAtEnd();
                    $('div.ui-grid-cell form').find('select').focus();
                    //    .on("focus", function () { // could be on any event
                    //    //$('div.ui-grid-cell form').find('input').putCursorAtEnd();
                    //    //console.log($('div.ui-grid-cell form').find('input').putCursorAtEnd());
                    //});
                });

                gridApi.edit.on.afterCellEdit($scope, function (rowEntity, colDef, newValue, oldValue) {
                    if (colDef.name === "Department*") {
                        let deptWiseDeptManager = [];
                        let deptWiseOpeManager = [];
                        let deptManagerDD = [];
                        let opeManagerDD = [];

                        deptWiseDeptManager = $scope.listOfDeptManagers.filter(user => user.DepartmentID == newValue);
                        deptWiseOpeManager = $scope.listOfOpeManagers.filter(user => user.DepartmentID == newValue);

                        angular.forEach(deptWiseDeptManager, function (user) {
                            deptManagerDD.push({ ID: user.EmployeeID, Value: user.FirstName + " " + user.LastName });
                        });
                        angular.forEach(deptWiseOpeManager, function (user) {
                            opeManagerDD.push({ ID: user.EmployeeID, Value: user.FirstName + " " + user.LastName });
                        });

                        rowEntity.departmentManagerOption = deptManagerDD;
                        rowEntity.operationManagerOption = opeManagerDD;
                    }
                    if (colDef.name === "Department Manager*") {
                        //colDef.editDropdownOptionsArray = rowEntity.departmentManagerOption;
                        angular.forEach(rowEntity.departmentManagerOption, function (opt) {
                            if (opt.ID == newValue) {
                                rowEntity.DepartmentManager = opt.Value;
                                rowEntity.DeptManagerID = opt.ID;
                                return false;
                            }
                        });
                    } else if (colDef.name === "Operations Manager*") {
                        //colDef.editDropdownOptionsArray = rowEntity.operationManagerOption;
                        angular.forEach(rowEntity.operationManagerOption, function (opt) {
                            if (opt.ID == newValue) {
                                rowEntity.OperationsManager = opt.Value;
                                rowEntity.OpeManagerID = opt.ID;
                                return false;
                            }
                        });

                    }
                });
            };

            $scope.cellClicked = function (row, col) {
                // do with that row and col what you want
                alert('hey!');
            }

            jQuery.fn.putCursorAtEnd = function () {
                console.log(this);
                return this.each(function () {
                    //alert('we in there');
                    // Cache references
                    var $el = $(this),
                        el = this;

                    // Only focus if input isn't already
                    if (!$el.is(":focus")) {
                        $el.focus();
                    }

                    // If this function exists... (IE 9+)
                    if (el.setSelectionRange) {

                        // Double the length because Opera is inconsistent about whether a carriage return is one character or two.
                        var ghettoLengthFix = 9999; //luan custom
                        var len = ghettoLengthFix * 2;

                        // Timeout seems to be required for Blink
                        setTimeout(function () {
                            el.setSelectionRange(len, len);
                            console.log('set to end');
                        }, 1);

                    } else {
                        $el.focus();
                        // As a fallback, replace the contents with itself
                        // Doesn't work in Chrome, but Chrome supports setSelectionRange
                        $el.val($el.val());

                    }

                    // Scroll to the bottom, in case we're in a tall textarea
                    // (Necessary for Firefox and Chrome)
                    this.scrollTop = 999999;

                });

            };

            $scope.check = function (row, col) {
                if (row.entity.checkbox == false) {
                    row.entity.checkbox = true;
                    //row.config.enableRowSelection = true;
                } else {
                    row.entity.checkbox = false;
                }
            }
            $scope.clicked = function (row, col) {
                $scope.orgRow = row;
                $scope.col = col;
                // $scope.row = row.entity;
            }
            $scope.save = function () {
                $("#save_Material").attr("disabled", true);
                var isReload = false;
                //
                var isReload = false;
                var isChanged = true;
                var isFilled = true;
                var listToSave = [];
                debugger;
                console.log($scope.approversCollection, $scope.originalApprovalsCollection);
                angular.forEach($scope.approversCollection, function (approver, key) {
                    console.log(approver);
                    //Find employee id for a user
                    //for (var x = 0; x < $scope.employeeCollection.length; x++) {
                    //    if ($scope.employeeCollection[x].ID == user.EmployeeName) {
                    //        user.EmployeeID = $scope.employeeCollection[x].ID;
                    //        user.EmployeeName = $scope.employeeCollection[x].Name;
                    //        break;
                    //    }
                    //}

                    ////debugger;
                    //$scope.roleId = [];

                    // for (var x = 0; x < $scope.RoleCollection.length; x++) {

                    //     //angular.forEach(user.Role, function (item) { 
                    //         if (user.Role.includes($scope.RoleCollection[x].Role)) {
                    //             //$scope.roleId.push($scope.RoleCollection[x].Id);
                    //             //user.RoleList.push($scope.RoleCollection[x].Id);

                    //         //break;
                    //     }

                    //// });
                    // }
                    //$scope.lstRole = $scope.RoleCollection;

                    //if (user.new === true) {
                    //    angular.forEach($scope.lstRole, function (item) {

                    //        item.isSelected = false;
                    //        if (user.RoleList != undefined) {    //vaishnavi 10-06-2022
                    //            if (user.RoleList.includes(item.Role)) {
                    //                item.isSelected = true;
                    //            }
                    //        }

                    //    });
                    //}
                    //else {
                    //    //angular.forEach(user.lstUserRole, function (item) {
                    //    //    item.isSelected = false;
                    //    //    if (user.Role.includes(item.Role)) {
                    //    //        item.isSelected = true;
                    //    //    }
                    //    //});

                    //    angular.forEach(user.lstUserRole, function (item) {
                    //        item.isSelected = false;
                    //        if (user.RoleList.includes(item.Role)) {
                    //            item.isSelected = true;
                    //        }
                    //    });
                    //}
                    //for (var x = 0; x < $scope.employeeCollection.length; x++) {
                    //    if ($scope.employeeCollection[x].Name == user.EmployeeName) {
                    //        user.EmployeeID = $scope.employeeCollection[x].ID;
                    //    }
                    //}

                    //Find Department id for user
                    //for (var i = 0; i < $scope.departmentsData.length; i++) {
                    //    if ($scope.departmentsData[i].ProjectClassID == approver.DepartmentName) {
                    //        approver.DepartmentID = $scope.departmentsData[i].ProjectClassID;
                    //        approver.DepartmentName = $scope.departmentsData[i].DepartmentName;
                    //        break;
                    //    }
                    //}

                    approver.DepartmentID = approver.Department;
                    approver.DeptManagerID = approver.DeptManagerID;
                    approver.OpeManagerID = approver.OpeManagerID;


                    //Detech change
                    isChanged = true;
                    angular.forEach($scope.originalApprovalsCollection, function (orgApprover) {

                        if (approver.ID === orgApprover.ID &&
                            approver.DepartmentID === orgApprover.DepartmentID &&
                            approver.DeptManagerID === orgApprover.DeptManagerID &&
                            approver.OpeManagerID === orgApprover.OpeManagerID) {
                            isChanged = false;
                        }


                        //angular.forEach(user.lstUserRole, function (item) {
                        //    item.isSelected = false;
                        //    if (user.RoleList.includes(item.Role)) {
                        //        item.isSelected = true;
                        //    }
                        //});
                    });

                    if (isChanged && (approver.DepartmentID == "" || approver.DepartmentID == null
                        || approver.DeptManagerID == "" || approver.DeptManagerID == null
                        || approver.OpeManagerID == "" || approver.OpeManagerID == null)) {
                        $("#save_Material").attr("disabled", false);
                        dhtmlx.alert({
                            text: "Department, Department Manager, Operations Manager cannot be empty (Row " + approver.displayId + ")",
                            width: "300px"
                        });
                        isFilled = false;
                        return;
                    }

                    if (isFilled == false) {
                        return;
                    }
                    if (approver.new === true) {
                        var dataObj = {
                            Operation: '1',
                            DepartmentID: approver.DepartmentID,
                            DeptManagerID: approver.DeptManagerID,
                            OpeManagerID: approver.OpeManagerID
                        }
                        debugger;
                        listToSave.push(dataObj);
                    }
                    else {

                        if (isChanged) {
                            debugger;
                            isChanged = true;

                            var dataObj = {
                                Operation: '2',
                                ID: approver.ID,
                                DepartmentID: approver.DepartmentID,
                                DeptManagerID: approver.DeptManagerID,
                                OpeManagerID: approver.OpeManagerID
                            }

                            debugger;
                            listToSave.push(dataObj);


                        } else {
                            //delete

                        }
                    }
                });

                angular.forEach($scope.listToDelete, function (item) {
                    debugger;
                    listToSave.push(item);
                });

                if ($scope.filterpresident.EmployeeID != $scope.approvalPresidentData.PresidentID) {
                    var URL = serviceBasePath + "adminApprovalPresident/savePresident";
                    $("#save_Material").attr("disabled", true);
                    var obj = {
                        Operation: '2',
                        ID: $scope.approvalPresidentData.ID,
                        PresidentID: $scope.filterpresident.EmployeeID
                    }

                    $http({
                        url: URL,
                        method: "POST",
                        data: JSON.stringify(obj),
                        headers: { 'Content-Type': 'application/json' }
                    }).then(function success(response) {
                        $("#save_Material").attr("disabled", false);
                        response.data.result.replace(/[\r]/g, '\n');

                        if (response.data.result) {
                            $("#save_Material").attr("disabled", false);
                            dhtmlx.alert(response.data.result);
                        } else {
                            $("#save_Material").attr("disabled", false);
                            dhtmlx.alert('No changes to be saved.');
                        }

                        $state.reload();
                    });
                }

                if (isFilled == false) {
                    return;
                } else {
                    debugger;
                    console.log(listToSave)
                    $("#save_Material").attr("disabled", true);

                    $http({
                        url: url,
                        method: "POST",
                        data: JSON.stringify(listToSave),
                        headers: { 'Content-Type': 'application/json' }
                    }).then(function success(response) {
                        $("#save_Material").attr("disabled", false);
                        response.data.result.replace(/[\r]/g, '\n');

                        if (response.data.result) {
                            $("#save_Material").attr("disabled", false);
                            dhtmlx.alert(response.data.result);
                        } else {
                            $("#save_Material").attr("disabled", false);
                            dhtmlx.alert('No changes to be saved.');
                        }

                        $state.reload();

                        getAllApproversGrid();

                        //Luan here - 
                        //User.get({}, function (userData) {
                        //    $scope.checkList = [];
                        //    $scope.approversCollection = userData.result;
                        //    $scope.originalApprovalsCollection = angular.copy(userData.result);
                        //    addIndex($scope.approversCollection);
                        //    angular.forEach($scope.approversCollection, function (item, index) {
                        //        item.checkbox = false;
                        //        $scope.checkList[index + 1] = false;

                        //        //Find employee name for a user
                        //        for (var x = 0; x < $scope.employeeCollection.length; x++) {
                        //            if ($scope.employeeCollection[x].ID == item.EmployeeID) {
                        //                item.EmployeeName = $scope.employeeCollection[x].Name;
                        //                //$scope.userCollection.EmployeeID = item.EmployeeID;
                        //                //$scope.userCollection.EmployeeName = item.EmployeeName;
                        //            }
                        //        }

                        //        //Find Department
                        //        for (var i = 0; i < $scope.departmentsData.length; i++) {
                        //            if ($scope.departmentsData[i].ProjectClassID == item.DepartmentID) {
                        //                item.DepartmentName = $scope.departmentsData[i].DepartmentName;
                        //            }
                        //        }


                        //        //Find role for a user

                        //        //for (var x = 0; x < $scope.roleCollection.length; x++) {
                        //        //    debugger;
                        //        //    if ($scope.roleCollection[x].RoleId == item.Role) {
                        //        //        item.Role = $scope.roleCollection[x].Role;
                        //        //        //$scope.userCollection.EmployeeID = item.EmployeeID;
                        //        //        //$scope.userCollection.EmployeeName = item.EmployeeName;
                        //        //    }
                        //        //}


                        //        //Find required password change
                        //        if (item.PasswordChangeRequired) {
                        //            item.PasswordChangeRequiredName = "True";
                        //        } else {
                        //            item.PasswordChangeRequiredName = "False";
                        //        }
                        //    });
                        //    $scope.gridOptions.data = $scope.approversCollection;

                        //    console.log($scope.approversCollection);
                        //});

                    }, function error(response) {
                        $("#save_Material").attr("disabled", false);
                        dhtmlx.alert("Failed to save. Information missing!");
                        console.log("Failed to save");
                    });
                }




            }
            $scope.delete = function () {
                var isChecked = false;
                var unSavedChanges = false;
                var listToSave = [];
                var newList = [];
                var selectedRow = false;
                $scope.listToDelete = [];
                angular.forEach($scope.approversCollection, function (item) {
                    if (item.checkbox == true) {
                        selectedRow = true;
                        if (item.new === true) {
                            unSavedChanges = true;
                            newList.push(item);
                        }
                        else {
                            isChecked = true;
                            var dataObj = {
                                Operation: '3',
                                ID: approver.ID,
                                DepartmentID: approver.DepartmentID,
                                DeptManagerID: approver.DeptManagerID,
                                OpeManagerID: approver.OpeManagerID

                            }
                            listToSave.push(dataObj);
                            $scope.listToDelete.push(dataObj);
                            //dhtmlx.alert("Record Deleted");
                        }
                    }
                });
                if (!selectedRow) {
                    dhtmlx.alert("Please select a record to delete.");
                }
                if (newList.length != 0) {
                    for (var i = 0; i < newList.length; i++) {
                        var ind = -1;
                        angular.forEach($scope.approversCollection, function (item, index) {
                            if (item.displayId == newList[i].displayId) {
                                item.checkbox = false;

                                ind = index;
                            }
                        });
                        if (ind != -1) {
                            $scope.checkList.splice(newList[i].displayId, 1);
                            $scope.approversCollection.splice(ind, 1);
                        }
                    }

                }
                if (listToSave.length != 0) {

                    for (var i = 0; i < listToSave.length; i++) {
                        var ind = -1;
                        angular.forEach($scope.approversCollection, function (item, index) {
                            if (item.displayId == listToSave[i].displayId) {
                                item.checkbox = false;

                                ind = index;
                            }
                        });
                        if (ind != -1) {
                            $scope.checkList.splice(listToSave[i].displayId, 1);
                            $scope.approversCollection.splice(ind, 1);
                        }
                    }
                }

            }

            //$scope.setUser = function(u){
            //    $scope.userItem = u;
            //}
            //
            //$scope.newUser = function(){
            //
            //    newOrEdit = 'new';
            //    var scope = $rootScope.$new();
            //    $scope.userItem = null;
            //    scope.params = {userItem : $scope.userItem, newOrEdit:newOrEdit}
            //    $rootScope.modalInstance = $uibModal.open({
            //        scope: scope,
            //        controller: "UserModalCtrl",
            //        templateUrl : "app/views/modal/users_modal.html",
            //        size : "md"
            //    } );
            //    $rootScope.modalInstance.result.then(function(response){
            //        User.get({},function(Users){
            //            $scope.userCollection = Users.result;
            //            console.log($scope.userCollection);
            //        })
            //    });
            //}
            //$scope.editUser = function(){
            //
            //    newOrEdit = 'edit';
            //    var scope = $rootScope.$new();
            //    scope.params = {userItem : $scope.userItem, newOrEdit : newOrEdit}
            //    $rootScope.modalInstance = $uibModal.open({
            //       scope:scope,
            //        templateUrl : "app/views/modal/users_modal.html",
            //        size : 'md',
            //        controller : 'UserModalCtrl'
            //    });
            //
            //    $rootScope.modalInstance.result.then(function(response){
            //        User.get({},function(Users){
            //            $scope.userCollection = Users.result;
            //            console.log($scope.userCollection);
            //        })
            //    });
            //}
            //$scope.deleteUser = function(){
            //    var scope = $rootScope.$new();
            //    $scope.confirm = "";
            //    scope.params={confirm:$scope.confirm};
            //    $rootScope.modalInstance = $uibModal.open({
            //        scope:scope,
            //        templateUrl :'app/views/Modal/confirmation_dialog.html',
            //        controller : 'ConfirmationCtrl',
            //        size : 'sm'
            //    });
            //    $rootScope.modalInstance.result.then(function(data){
            //        if(scope.params.confirm ==='yes'){
            //            var dataObj = {
            //                'Operation': '3',
            //                'FullName' : "",
            //                'UserID' : $scope.userItem.UserID,
            //                'AccessControlList' : "",
            //                'LoginPassword' : "",
            //                'Email': $scope.userItem.Email
            //
            //            }
            //
            //            var index = $scope.userCollection.indexOf($scope.userItem);
            //            $http.post(url,dataObj).then(function(response){
            //                if(response.data.result==="Success"){
            //                    //refresh on delete
            //                    User.get({},function(Users){
            //                        $scope.userCollection = Users.result;
            //                        console.log($scope.userCollection);
            //                    })
            //                    if(index !== -1){
            //                        $scope.userCollection.splice(index,1);
            //                        $scope.userItem = null;
            //                    }
            //
            //                }
            //                else{
            //                    alert("Delete Failed");
            //                }
            //
            //            });
            //        }
            //    });
            //}

            $scope.checkForChanges = function () {
                var unSavedChanges = false;
                var originalCollection = $scope.originalApprovalsCollection;
                var currentCollection = $scope.approversCollection;

                if (currentCollection.length != originalCollection.length) {
                    unSavedChanges = true;
                    return unSavedChanges;
                } else {
                    angular.forEach(currentCollection, function (currentObject) {
                        for (var i = 0, len = originalCollection.length; i < len; i++) {
                            if (unSavedChanges) {
                                return unSavedChanges; // no need to look through the rest of the original array
                            }
                            if (originalCollection[i].Id == currentObject.Id) {
                                var originalObject = originalCollection[i];
                                // compare relevant data
                                if (originalObject.UserID !== currentObject.UserID ||
                                    originalObject.FirstName !== currentObject.FirstName ||
                                    originalObject.LastName !== currentObject.LastName ||
                                    originalObject.Email !== currentObject.Email ||
                                    originalObject.EmployeeID !== currentObject.EmployeeID ||
                                    originalObject.DepartmentID !== currentObject.DepartmentID ||
                                    originalObject.Role !== currentObject.Role) {
                                    // alert if a change has not been saved
                                    //alert("unsaved change on line" + currentCollection.displayId);
                                    unSavedChanges = true;
                                    return unSavedChanges;
                                }
                                break; //no need to check any further, go to next object in new collection
                            }
                        }
                    });
                }
                return unSavedChanges;
            };

            onRouteChangeOff = $scope.$on('$locationChangeStart', function (event) {
                var newUrl = $location.path();
                if (!$scope.checkForChanges()) return;    //luan here
                $scope.confirm = "";
                var scope = $rootScope.$new();
                scope.params = { confirm: $scope.confirm };
                $rootScope.modalInstance = $uibModal.open({
                    scope: scope,
                    templateUrl: 'app/views/Modal/exit_confirmation_modal.html',
                    controller: 'exitConfirmation',
                    size: 'md',
                    backdrop: true
                });
                $rootScope.modalInstance.result.then(function (data) {
                    console.log(scope.params.confirm);
                    if (scope.params.confirm === "exit") {
                        onRouteChangeOff();
                        $location.path(newUrl);
                    }
                    else if (scope.params.confirm === "save") {
                        isTest = true;
                        $scope.save();
                        //    onRouteChangeOff();
                        //    $location.path(newUrl);
                    }
                    else if (scope.params.confirm === "back") {
                        //do nothing
                    }
                });
                event.preventDefault();
            });

        }])
    .directive('uiSelectWrap', uiSelectWrap)
    ;
//.angular.module('app', ['ui.grid', 'ui.grid.edit', 'ui.select'])
uiSelectWrap.$inject = ['$document', 'uiGridEditConstants']; //uiGridEditConstants
function uiSelectWrap($document, uiGridEditConstants) {
    return function link($scope, $elm, $attr) {


        //$scope.$parent.col.grid.options.rowHeight=100;
        $document.on('click', docClick);
        //var target = $(event.target);
        function docClick(evt) {
            // $scope.$parent.col.grid.options.rowHeight = 40;
            if ($(evt.target).closest('.ui-select-container').size() === 0) {
                $scope.$emit(uiGridEditConstants.events.END_CELL_EDIT);

                $document.off('click', docClick);

            }
        }
    };
}
