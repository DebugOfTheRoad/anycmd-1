/// <reference path="../../../jquery-1.8.3.intellisense.js" />
/// <reference path="../../../miniui/miniui.js" />
/// <reference path="../../../jquery-bbq/jquery.ba-bbq.js" />
(function (window, $) {
    mini.namespace("AC.Account.Contractors");
    var self = AC.Account.Contractors;
    self.prifix = "AC_Account_Contractors_";
    self.loadData = loadData;
    self.search = search;
    self.enable = enable;
    self.disable = disable;
    self.gridReload = function () {
        grid.reload();
    };
    mini.namespace("AC.Account.Contractors.Edit");
    var edit = AC.Account.Contractors.Edit;
    edit.prifix = "AC_Account_Contractors_Edit_"
    edit.SaveData = SaveData;

    var tabConfigs = {
        infoTab: {
            url: bootPATH + "../AC/Account/Details",
            params: [{ "pName": 'id', "pValue": "Id" }],
            namespace: "Account.Details"
        },
        roleTab: {
            url: bootPATH + "../AC/Account/Roles",
            params: [{ "pName": 'accountID', "pValue": "Id" }],
            namespace: "Account.Roles"
        },
        groupTab: {
            url: bootPATH + "../AC/Account/Groups",
            params: [{ "pName": 'accountID', "pValue": "Id" }],
            namespace: "Account.Groups"
        },
        organizationTab: {
            url: bootPATH + "../AC/Account/Organizations",
            params: [{ "pName": 'accountID', "pValue": "Id" }],
            namespace: "Account.Organizations"
        },
        operationLogTab: {
            url: bootPATH + "../AC/OperationLog/Index",
            params: [{ "pName": 'targetID', "pValue": "Id" }],
            namespace: "AC.OperationLog.Index"
        }
    };
    self.filters = {
        Name: {
            type: 'string',
            comparison: 'like'
        },
        Code: {
            type: 'string',
            comparison: 'like'
        },
        Mobile: {
            type: 'string',
            comparison: 'like'
        },
        Email: {
            type: 'string',
            comparison: 'like'
        },
        QQ: {
            type: 'string',
            comparison: 'like'
        },
        Telephone: {
            type: 'string',
            comparison: 'like'
        },
        IsEnabled: {
            type: 'numeric',
            comparison: 'eq'
        },
        OrganizationName: {
            type: 'string',
            comparison: 'like'
        }
    };

    mini.parse();

    var win = mini.get(edit.prifix + "win1");
    var form;
    if (win) {
        form = new mini.Form(edit.prifix + "form1");
    }
    var passwordEditWin = mini.get(edit.prifix + "passwordEditWin");
    var passwordEditForm = new mini.Form(edit.prifix + "passwordEditForm");
    var btnChangePassword = mini.get(self.prifix + "btnChangePassword");
    $().ready(function () {
        var btnOk = "btnOk";
        var btnCancel = "btnCancel";
        if (edit.prifix) {
            btnOk = edit.prifix + btnOk;
            btnCancel = edit.prifix + btnCancel;
        }
        $("#" + passwordEditWin.id + " ." + btnOk).click(changePassword);
        $("#" + passwordEditWin.id + " ." + btnCancel).click(function () {
            passwordEditWin.hide()
        });
    });
    btnChangePassword.on("click", function (e) {
        var record = grid.getSelected();
        if (record) {
            passwordEditForm.setData(record);
            passwordEditWin.show();
        }
    });
    function changePassword() {
        var data = $("#" + edit.prifix + "passwordEditForm").serialize();
        var url = bootPATH + "../AC/Account/ChangePassword";
        passwordEditForm.validate();
        if (passwordEditForm.isValid() == false) return;

        $.post(url, data, function (result) {
            helper.response(result, function () {
                passwordEditWin.hide();
                grid.reload();
            });
        }, "json");
    }

    var chkbIncludedescendants = mini.get(self.prifix + "chkbIncludedescendants");
    chkbIncludedescendants.on("checkedchanged", function () {
        search();
    });
    var btnEdit = mini.get(self.prifix + "btnEdit");
    var btnDelete = mini.get(self.prifix + "btnRemove");
    var editPermission = btnEdit && btnEdit.enabled;
    var deletePermission = btnDelete && btnDelete.enabled;

    var tabs1 = mini.get(self.prifix + "tabs1");
    var grid = mini.get(self.prifix + "datagrid1");
    grid.on("drawcell", ondrawcell);
    var helperDrawcell = helper.ondrawcell(self, "AC.Account.Contractors");
    grid.on("load", helper.onGridLoad);
    loadData();

    helper.index.allInOne(
        edit,
        grid,
        bootPATH + "../AC/Account/Edit",
        bootPATH + "../AC/Account/Edit",
        bootPATH + "../AC/Account/Delete",
        self);

    helper.index.tabInOne(grid, tabs1, tabConfigs, self);

    function enable() {
        var records = grid.getSelecteds();
        if (records.length > 0) {
            var id;
            var ids = [];
            for (var i = 0, l = records.length; i < l; i++) {
                var r = records[i];
                ids.push(r.Id);
            }
            id = ids.join(',');
            $.post(bootPATH + "../AC/Account/EnableAccount", { id: id }, function (result) {
                helper.response(result, function () {
                    grid.reload();
                });
            }, "json");
        }
        else {
            mini.alert("请先选中记录");
        }
    }

    function disable() {
        var records = grid.getSelecteds();
        if (records.length > 0) {
            var id;
            var ids = [];
            for (var i = 0, l = records.length; i < l; i++) {
                var r = records[i];
                ids.push(r.Id);
            }
            id = ids.join(',');
            $.post(bootPATH + "../AC/Account/DisableAccount", { id: id }, function (result) {
                helper.response(result, function () {
                    grid.reload();
                });
            }, "json");
        }
        else {
            mini.alert("请先选中记录");
        }
    }

    function loadData() {
        search();
    }

    function search() {
        var data = {};
        var fragment = $.deparam.fragment();
        if (fragment.organizationCode) {
            data.organizationCode = fragment.organizationCode;
        }
        if (chkbIncludedescendants.getValue() == "1") {
            data.includedescendants = true;
        }
        else {
            data.includedescendants = false;
        }
        var filterArray = [];
        for (var k in self.filters) {
            var filter = self.filters[k];
            if (filter.value) {
                filterArray.push({ field: k, type: filter.type, comparison: filter.comparison, value: filter.value });
            }
        }
        data.filters = JSON.stringify(filterArray);
        if (!grid.sortField) {
            grid.sortBy("CreateOn", "desc");
        }
        grid.load(data, function () {
            var record = grid.getSelected();
            if (!record) {
                tabs1.hide();
            }
        });
    }
    function SaveData() {
        var fragment = $.deparam.fragment();
        if (fragment.organizationCode) {
            $("#" + edit.prifix + "form1 input[name='OrganizationCode']").val(fragment.organizationCode);
        }
        var data = $("#" + edit.prifix + "form1").serialize();
        var id = $("#" + edit.prifix + "form1 input[name='Id']").val();
        var url = bootPATH + "../AC/Account/Create";
        if (id) {
            url = bootPATH + "../AC/Account/Update";
        }
        form.validate();
        if (form.isValid() == false) return;

        $.post(url, data, function (result) {
            helper.response(result, function () {
                edit.CloseWindow("save");
                self.gridReload();
            });
        }, "json");
    };
    function ondrawcell(e) {
        var field = e.field;
        var value = e.value;
        var columnName = e.column.name;
        if (field) {

        }
        helperDrawcell(e);
    };

    helper.edit.allInOne(
        self,
        win,
        bootPATH + "../AC/Account/Create",
        bootPATH + "../AC/Account/Update",
        bootPATH + "../AC/Account/Get",
        form, edit);
})(window, jQuery);