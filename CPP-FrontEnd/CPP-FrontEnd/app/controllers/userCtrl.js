angular.module('cpp.controllers').
    //User Controller
    controller('UserCtrl', ['$state', '$http', '$uibModal', 'User', '$rootScope', '$scope', 'Page', 'ProjectTitle', 'TrendStatus', '$location', 'AllEmployee', '$timeout',
        function ($state, $http, $uibModal, User, $rootScope, $scope, Page, ProjectTitle, TrendStatus, $location, AllEmployee, $timeout) {
            Page.setTitle('Users');
            ProjectTitle.setTitle('');
            TrendStatus.setStatus('');
            var empArray = [];
            $http.get(serviceBasePath + 'request/role').then(function (response) {
                $scope.RoleCollection = response.data.result;

                $scope.gridOptions.columnDefs[6].editDropdownOptionsArray = response.data.result;
                console.log($scope.RoleCollection);
            })
            var newOrEdit = "";
            var url = serviceBasePath + "response/user";
            $scope.$on('ngGridEventEndCellEdit', function (data) {
                console.log(data.targetScope.row.entity.status);
                //data.targetScope.row.entity.status = 'Modified';
                console.log($scope.userCollection);
            });

            //Get all employees - filter by organization later
            AllEmployee.get({}, function (employees) {
                $scope.employeeCollection = employees.result;
                console.log(employees.result);

                angular.forEach($scope.employeeCollection, function (employee) {
                    employee.EmployeeName = employee.Name; //here
                });


                angular.forEach(employees.result, function (item) {
                    //$scope.positionArray.push({ ID: item.Id, value: item.PositionDescription });
                    //$rootScope.positionArray.push({ ID: item.Id, value: item.PositionDescription });
                    empArray.push({ ID: item.ID, value: item.Name });
                }
                );

                //$scope.gridOptions.columnDefs[7].editDropdownOptionsArray = $scope.employeeCollection;

                console.log($scope.employeeCollection);


                console.log(AllEmployee);
                console.log(User);

                //Get all Users
                User.get({}, function (Users) {
                    $scope.checkList = [];
                    $scope.userCollection = Users.result;
                    $scope.orgUserCollection = angular.copy(Users.result);
                    console.log(Users);
                    angular.forEach($scope.userCollection, function (user, key) {
                        user.status = 'Unchanged';
                    });
                    $scope.orgUserCollection = angular.copy(Users.result);

                    //Declare true false dropdown list
                    $scope.trueFalseDropDown = [{ PasswordChangeRequiredName: "True" }, { PasswordChangeRequiredName: "False" }];
                    $scope.gridOptions.columnDefs[8].editDropdownOptionsArray = $scope.trueFalseDropDown;

                    addIndex($scope.userCollection);
                    angular.forEach($scope.userCollection, function (item, index) {
                        $scope.checkList[index + 1] = false;
                        item.checkbox = false;

                        //Find password change name
                        if (item.PasswordChangeRequired) {
                            item.PasswordChangeRequiredName = "True";
                        } else {
                            item.PasswordChangeRequiredName = "False";
                        }

                        //Find employee
                        for (var x = 0; x < $scope.employeeCollection.length; x++) {
                            console.log($scope.employeeCollection[x].ID, item.EmployeeID);
                            if ($scope.employeeCollection[x].ID == item.EmployeeID) {
                                item.EmployeeName = $scope.employeeCollection[x].Name;
                                //$scope.userCollection.EmployeeID = item.EmployeeID;
                                //$scope.userCollection.EmployeeName = item.EmployeeName;
                            }
                        }
                    });
                    $scope.gridOptions.data = $scope.userCollection;
                    console.log($scope.userCollection);
                });
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
            $scope.cellPasswordEditableTemplate = '<input type = "password" ng-class="\'colt\' + col.index" ng-input="COL_FIELD" ng-model="COL_FIELD" ng-blur="updateEntity(row)" />';
            $scope.cellSelectEditableTemplate = '<select ng-class="\'colt\' + col.index" ng-input="COL_FIELD" ng-model="COL_FIELD" ng-click = "test(COL_FIELD)" ng-options="role.Role for role in RoleCollection" />';
            $scope.mySelections = [];
            $scope.test = function (test) {
                console.log(test);
            }
            $scope.cellCheckEditTableTemplate = '<input class="c-approval-matrix-check"  type="checkbox" ng-input="COL-FIELD" ng-model="COL_FIELD"/>';
            $scope.addRow = function () {
                var x = Math.max.apply(Math, $scope.userCollection.map(function (o) {

                    return o.displayId;
                }))

                if (x < 0) {
                    console.log(x);
                    x = 0;
                }

                $scope.userCollection.splice(x, 0, {
                    displayId: ++x,
                    UserID: '',
                    FirstName: '',
                    MiddleName: '',
                    LastName: '',
                    Email: '',
                    Role: 'Click to Select',
                    EmployeeID: '',
                    PasswordChangeRequired: '',
                    EmployeeName: '',
                    Password: '',
                    checkbox: false,
                    new: true
                });

                console.log($scope.userCollections);
                $scope.gridApi.core.clearAllFilters();//Nivedita-T on 17/11/2021
                $timeout(function () {
                    
                    $scope.gridApi.core.scrollTo($scope.gridOptions.data[$scope.gridOptions.data.length - 1], $scope.gridOptions.columnDefs[0]);
                }, 1);
            }

            $scope.gridOptions = {
                enableColumnMenus :false,
                enableCellEditOnFocus: true,
                enableFiltering: true,
                /*data: 'userCollection',
                enableRowSelection: false,
                enableCellSelection: true,
                selectedItems: $scope.mySelections,
                enableCellEditOnFocus: true,
                multiSelect: false,*/
                rowHeight: 40,
                /*afterSelectionChange: function (rowItem, event) {
                    console.log($scope.mySelections);
                    $scope.selectedIDs = [];
                    console.log(rowItem);
                    angular.forEach($scope.mySelections, function (item) {
                        $scope.selectedIDs.push(item.id)
                    });

                },*/
                columnDefs: [{
                    field: 'displayId',
                    name: 'ID',
                    enableCellEdit: false,
                    //cellClass: 'c-col-id',
                    width: 50,
                    cellClass: 'c-col-Num' //Manasi

                    /*cellTemplate: '<div ng-class="c-col-id" style="margin-top:15%;" ng-click="clicked(row,col)">{{row.getProperty(col.field)}}</div>'*/

                }, {
                    field: 'UserID',
                    name: 'User Id*'
                    /*enableCellEditOnFocus: true,
                    editableCellTemplate: $scope.cellInputEditableTemplate*/

                }, {
                    field: 'FirstName',
                    name: 'First Name*'
                    /*enableCellEditOnFocus: true,
                    editableCellTemplate: $scope.cellInputEditableTemplate*/


                },
                    {
                        field: 'MiddleName',
                        name: 'Middle Name'
                       /* enableCellEditOnFocus: true,
                        editableCellTemplate: $scope.cellInputEditableTemplate*/


                    },
                    {
                        field: 'LastName',
                        name: 'Last Name*'
                        /*enableCellEditOnFocus: true,
                        editableCellTemplate: $scope.cellInputEditableTemplate*/


                    },
                    {
                        field: 'Email',
                        name: 'Email*',
                        /*enableCellEditOnFocus: true,
                        editableCellTemplate: $scope.cellInputEditableTemplate,*/
                        width: 200


                    },
                    {
                        field: 'Role',
                        name: 'Role*',
                        editableCellTemplate: 'ui-grid/dropdownEditor',
                        editDropdownValueLabel: 'Role',
                        editDropdownIdLabel: 'Role',
                        editDropDownChange: 'test',
                       /* enableCellEditOnFocus: true,
                        editableCellTemplate: $scope.cellSelectEditableTemplate,*/
                        cellFilter: 'mapRole',
                        width: 200
                    }
                    ,
                    {
                        field: 'EmployeeName',
                        name: 'Employee*',
                        editableCellTemplate: 'ui-grid/dropdownEditor',
                        editDropdownIdLabel: 'ID',
                        editDropdownValueLabel: 'value',
                        editDropdownOptionsArray: empArray,
                        //editDropDownChange: 'test',
                        //cellFilter: 'mapRole',
                        cellFilter: 'customFilter:this',
                        cellClass: 'c-col-Num',  
                        width: 200
                    },
                    {
                        field: 'PasswordChangeRequiredName',
                        name: 'Pwd Change*',
                        editableCellTemplate: 'ui-grid/dropdownEditor',
                        editDropdownValueLabel: 'PasswordChangeRequiredName',
                        editDropdownIdLabel: 'PasswordChangeRequiredName',
                        editDropDownChange: 'test',
                        cellFilter: 'mapRole',
                        width: 120
                    }
                    ,
                    {
                        field: 'LoginPassword',
                        name: 'Password',
                        type: 'password',
                        validators: {
                            required: false
                        },
                        /*enableCellEditOnFocus: true,
                        editableCellTemplate: true,*/
                        width: 200
                    },
                    {
                        field: 'checkBox',
                        name: '',
                        enableCellEdit: false,
                        enableFiltering: false,
                        width: 35,
                        cellTemplate: '<input type="checkbox" ng-model="checkList[row.entity.displayId]" class = "c-col-check" ng-click="grid.appScope.check(row,col)" style="text-align: center;vertical-align: middle;">'

                    }
                ]
            }
            $scope.gridOptions.onRegisterApi = function (gridApi) {
                $scope.gridApi = gridApi;

                $scope.gridApi.edit.on.beginCellEdit($scope, function (rowEntity, colDef) {
                    $('div.ui-grid-cell form').find('input').css('height', '40px');
                    $('div.ui-grid-cell form').find('select').css('height', '40px');
                    $('div.ui-grid-cell form').css('margin', '0px');
                    $('div.ui-grid-cell form').find('input').putCursorAtEnd();
                    $('div.ui-grid-cell form').find('select').putCursorAtEnd();
                    $('div.ui-grid-cell form').find('select').focus();
                    //    .on("focus", function () { // could be on any event
                    //    //$('div.ui-grid-cell form').find('input').putCursorAtEnd();
                    //    //console.log($('div.ui-grid-cell form').find('input').putCursorAtEnd());
                    //});
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
                    row.config.enableRowSelection = true;
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
                var isReload = false;
                //
                var isReload = false;
                var isChanged = true;
                var isFilled = true;
                var listToSave = [];
                console.log($scope.userCollection, $scope.orgUserCollection);
                angular.forEach($scope.userCollection, function (user, key) {
                    console.log(user);
                    //Find employee id for a user
                    for (var x = 0; x < $scope.employeeCollection.length; x++) {
                        if ($scope.employeeCollection[x].ID == user.EmployeeName) {
                            user.EmployeeID = $scope.employeeCollection[x].ID;
                            user.EmployeeName = $scope.employeeCollection[x].Name;
                            break;
                        }
                    }
                        //for (var x = 0; x < $scope.employeeCollection.length; x++) {
                        //    if ($scope.employeeCollection[x].Name == user.EmployeeName) {
                        //        user.EmployeeID = $scope.employeeCollection[x].ID;
                        //    }
                        //}
                   
                        
                    
                    
                    
                    
                    console.log(user.PasswordChangeRequired);
                    //Find password change required for a user
                    if (user.PasswordChangeRequiredName == "True") {
                        user.PasswordChangeRequired = true;
                    } else {
                        user.PasswordChangeRequired = false;
                    }
                    console.log(user.PasswordChangeRequired);

                    
                    //Detech change
                    isChanged = true;
                    angular.forEach($scope.orgUserCollection, function (orgUser) {
                        console.log(user.PasswordChangeRequired === orgUser.PasswordChangeRequired);
                        if (user.Id === orgUser.Id &&
                            user.UserID === orgUser.UserID &&
                            user.FirstName === orgUser.FirstName &&
                            user.MiddleName === orgUser.MiddleName &&
                            user.LastName === orgUser.LastName &&
                            user.Role === orgUser.Role &&
                            user.EmployeeID === orgUser.EmployeeID &&
                            user.LoginPassword === orgUser.LoginPassword &&
                            user.Email === orgUser.Email &&
                            user.PasswordChangeRequired === orgUser.PasswordChangeRequired) {
                            isChanged = false;
                        }
                    });

                    if (isChanged && user.Id == undefined && (user.UserID == "" || user.UserID == null
                                                              || user.LoginPassword == "" || user.LoginPassword == null
                                                              || user.FirstName == "" || user.FirstName == null
                                                              || user.LastName == "" || user.LastName == null
                                                              || user.Email == "" || user.Email == null
                                                              || user.Role == "" || user.Role == null
                                                              || user.EmployeeID == "" || user.EmployeeID == null
                                                              || user.PasswordChangeRequired == undefined || user.PasswordChangeRequired == null)) {
                        dhtmlx.alert({
                            text: "UserID, First Name, Last Name, Email, Role, Employee, and Password cannot be empty (Row " + user.displayId + ")",
                            width: "300px"
                        });
                        isFilled = false;
                        return;
                    }

                    if (isFilled == false) {
                        return;
                    }
                    //--------------------------Swapnil 01-09-2020 email validation--------------------------------------------------------------------------
                    const regex = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;

                    if (isChanged && !regex.test(user.Email)) {
                        dhtmlx.alert({
                            text: "Enter valid email address (Row " + user.displayId + ")",
                            width: "300px"
                        });
                        isFilled = false;
                        return;
                    }
                    //------------------------------------------------------------------------------------------------------
                    //-----------------------------Nivedita 09-11-2021 password validation-------------------------------------------------------------------------



                    const regexPassword = /^(?=.*[A-Z])(?=.*\d)(?=.*[$@$!%*#?&])[A-Za-z\d$@$!%*#?&]{8,20}$/;
                    if (isChanged && !regexPassword.test(user.LoginPassword) && user.LoginPassword != "") {
                        dhtmlx.alert({
                            text: "Enter valid password.<br/> Password should be at least 8 characters long, should contain atleast one number,one capital character and one special character. (Row " + user.displayId + ")",
                            width: "300px"
                        });
                        isFilled = false;
                        return;
                    }
//------------------------------------------------------------------------------------------------------------------------
                    if (user.new === true) {
                        var dataObj = {
                            Operation: '1',
                            UserID: user.UserID,
                            FirstName: user.FirstName,
                            MiddleName: user.MiddleName,
                            LastName: user.LastName,
                            Email: user.Email,
                            Role: user.Role,
                            EmployeeID: user.EmployeeID,
                            LoginPassword: user.LoginPassword,
                            PasswordChangeRequired: user.PasswordChangeRequired
                        }
                        listToSave.push(dataObj);
                    }
                    else {

                        if (isChanged) {
                            isChanged = true;
                            if (typeof user.Role == 'string') {
                                temp = user.Role;
                            } else {
                                temp = user.Role;
                            }
                            console.log(user);

                            var dataObj = {
                                Operation: '2',
                                Id: user.Id,
                                UserID: user.UserID,
                                FirstName: user.FirstName,
                                MiddleName: user.MiddleName,
                                LastName: user.LastName,
                                Email: user.Email,
                                EmployeeID: user.EmployeeID,
                                PasswordChangeRequired: user.PasswordChangeRequired,
                                Role: temp

                            }

                            //
                            if (!(user.LoginPassword === "" || user.LoginPassword == null))
                                dataObj.LoginPassword = user.LoginPassword;
                            listToSave.push(dataObj);


                        } else {
                            //delete

                        }
                    }
                });

                angular.forEach($scope.listToDelete, function (item) {
                    listToSave.push(item);
                });

                if (isFilled == false) {
                    return;
                } else {
                    console.log(listToSave)
                    $http({
                        url: url,
                        //url: 'http://localhost:29986/api/response/phasecode',
                        method: "POST",
                        data: JSON.stringify(listToSave),
                        headers: {'Content-Type': 'application/json'}
                    }).then(function success(response) {
                        response.data.result.replace(/[\r]/g, '\n');

                        if (response.data.result) {
                            dhtmlx.alert(response.data.result);
                        } else {
                            dhtmlx.alert('No changes to be saved.');
                        }

                        $state.reload();

                        //Luan here - 
                        User.get({}, function (userData) {
                            $scope.checkList = [];
                            $scope.userCollection = userData.result;
                            $scope.orgUserCollection = angular.copy(userData.result);
                            addIndex($scope.userCollection);
                            angular.forEach($scope.userCollection, function (item, index) {
                                item.checkbox = false;
                                $scope.checkList[index + 1] = false;

                                //Find employee name for a user
                                for (var x = 0; x < $scope.employeeCollection.length; x++) {
                                    if ($scope.employeeCollection[x].ID == item.EmployeeID) {
                                        item.EmployeeName = $scope.employeeCollection[x].Name;
                                        //$scope.userCollection.EmployeeID = item.EmployeeID;
                                        //$scope.userCollection.EmployeeName = item.EmployeeName;
                                    }
                                }

                                //Find required password change
                                if(item.PasswordChangeRequired) {
                                    item.PasswordChangeRequiredName = "True";
                                } else {
                                    item.PasswordChangeRequiredName = "False";
                                }
                            });
                            $scope.gridOptions.data = $scope.userCollection;

                            console.log($scope.userCollection);
                        });

                    },function error(response){
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
                angular.forEach($scope.userCollection, function (item) {
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
                                UserID: item.UserID,
                                Id: item.Id,
                                FirstName: item.FirstName,
                                MiddleName: item.MiddleName,
                                LastName: item.LastName,
                                Email: item.Email,
                                Role: item.Role,
                                EmployeeID: item.EmployeeID,
                                PasswordChangeRequired: item.PasswordChangeRequired,
                                displayId : item.displayId

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
                if(newList.length != 0){
                    for(var i = 0; i < newList.length; i++) {
                        var ind = -1;
                        angular.forEach($scope.userCollection, function (item, index) {
                            if (item.displayId == newList[i].displayId) {
                                item.checkbox = false;

                                ind = index;
                            }
                        });
                        if(ind != -1){
                            $scope.checkList.splice(newList[i].displayId,1);
                            $scope.userCollection.splice(ind,1);
                        }
                    }

                }
                if(listToSave.length != 0) {

                    for(var i = 0; i < listToSave.length; i++) {
                        var ind = -1;
                        angular.forEach($scope.userCollection, function (item, index) {
                            if (item.displayId == listToSave[i].displayId) {
                                item.checkbox = false;

                                ind = index;
                            }
                        });
                        if(ind != -1){
                            $scope.checkList.splice(listToSave[i].displayId,1);
                            $scope.userCollection.splice(ind,1);
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
            var originalCollection = $scope.orgUserCollection;
            var currentCollection = $scope.userCollection;

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
            scope.params = {confirm: $scope.confirm};
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
;