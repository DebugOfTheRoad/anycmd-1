/// <reference path="../../../jquery-1.8.3.intellisense.js" />
/// <reference path="../../../miniui/miniui.js" />
/// <reference path="../../../helper.js" />
/// <reference path="../../../jquery-bbq/jquery.ba-bbq.js" />
(function (window) {
    mini.namespace("Organization.Accounts");
    var self = Organization.Accounts;
    self.prifix = "AC_Organization_Accounts_";
    self.loadData = loadData;
    self.search = loadData;

    window.onSelectOk = onSelectOk;

    mini.parse();

    var btnAddTo = mini.get(self.prifix + "btnAddTo");
    if (btnAddTo) {
        btnAddTo.on("click", function () {
            var win = mini.get("Select_win1");
            win.show();
        });
    }
    var btnRemoveFrom = mini.get(self.prifix + "btnRemoveFrom");
    if (btnRemoveFrom) {
        btnRemoveFrom.on("click", function () {
            var records = grid.getSelecteds();
            if (records.length > 0) {
                mini.confirm("确定移除选中记录？", "确定？", function (action) {
                    if (action == "ok") {
                        remove(records);
                    }
                });
            } else {
                mini.alert("请先选中记录");
            }
        });
    }
    var chkbIncludedescendants = mini.get(self.prifix + "chkbIncludedescendants");
    chkbIncludedescendants.on("checkedchanged", function () {
        loadData();
    });
    var key = mini.get(self.prifix + "key");
    key.on("enter", loadData);
    var grid = mini.get(self.prifix + "datagrid1");
    var showRowDetail = document.getElementById(self.prifix + "showRowDetail");
    var dgUserAccount = mini.get(self.prifix + "dgUserAccount");
    dgUserAccount.on("drawcell", ondrawcell);
    grid.on("drawcell", ondrawcell);
    grid.on("load", helper.onGridLoad);
    grid.on("showRowDetail", onShowRowDetail);
    loadData();

    function onSelectOk(accountDic) {
        var data = getParams();
        data.accountIDs = accountDic.accountIDs;
        $.post(bootPATH + "../AC/Organization/AddAccountOrganizations",
            data,
            function (result) {
                helper.response(result, function () {
                    grid.reload();
                });
            }, "json");
    }

    function remove(records) {
        var id;
        if (typeof records == typeof []) {
            var ids = [];
            for (var i = 0, l = records.length; i < l; i++) {
                var r = records[i];
                ids.push(r.Id);
            }
            id = ids.join(',');
            directRemove(id);
        }
        else {
            mini.confirm("确定删除选中记录？", "确定？", function (action) {
                if (action == "ok") {
                    if (typeof records == "string") {
                        id = records;
                    }
                    else if (records && records.Id) {
                        id = records.Id;
                    }
                    directRemove(id);
                }
            });
        }
    }

    function directRemove(id) {
        if (id) {
            grid.loading("操作中，请稍后......");
            $.post(bootPATH + "../AC/Organization/RemoveAccountOrganizations", { id: id }, function (result) {
                helper.response(result, function () {
                    grid.reload();
                }, function () {
                    grid.unmask();
                });
            }, "json");
        }
    }

    function loadData() {
        var data = getParams();
        if (data.organizationCode != grid.organizationCode) {
            grid.organizationCode = data.organizationCode;
            key.setValue("");
        }
        data.key = key.getValue();
        if (chkbIncludedescendants.getValue() == "1") {
            data.includedescendants = true;
        }
        else {
            data.includedescendants = false;
        }
        if (!grid.sortField) {
            grid.sortBy("CreateOn", "asc");
        }
        grid.load(data);
    }

    function getParams() {
        data = { };
        if (self.params && self.params.organizationCode) {
            data.organizationCode = self.params.organizationCode;
            data.organizationID = self.params.organizationID;
        }
        else {
            var fragment = $.deparam.fragment();
            var querystring = $.deparam.querystring();
            data.organizationCode = fragment.organizationCode || querystring.organizationCode;
            data.organizationID = fragment.organizationID || querystring.organizationID;
        }
        return data;
    }

    function onShowRowDetail(e) {
        var row = e.record;
        var td = grid.getRowDetailCellEl(row);
        td.appendChild(showRowDetail);
        showRowDetail.style.display = "";
        dgUserAccount.load({ contractorID: row.ContractorID });
        if (!dgUserAccount.sortField) {
            dgUserAccount.sortBy("CreateOn", "desc");
        }
    }

    function ondrawcell(e) {
        var field = e.field;
        var value = e.value;
        var columnName = e.column.name;
        var deletePermission = btnRemoveFrom && btnRemoveFrom.enabled;

        if (field) {
            switch (field) {
                case "IsContractorEnabled":
                case "IsEnabled":
                    if (value == "正常" || value == "1" || value == true) {
                        e.cellHtml = "<span class='icon-enabled width16px'></span>";
                    } else if (value == "禁用" || value == "0" || value == false) {
                        e.cellHtml = "<span class='icon-disabled width16px'></span>";
                    } break;
            }
        }
        if (columnName && columnName == "action") {
            var record = e.record;
            if (deletePermission) {
                e.cellHtml = '<a title="移除" href="javascript:Organization.Accounts.remove(\'' + record.Id + '\')"><img alt="删除" border="0" src="' + bootPATH + '../Scripts/miniui/themes/icons/remove.gif" /></a>';
            }
        }
    };
    helper.index.allInOne(
        null,
        grid,
        bootPATH + "../AC/Organization/AddAccountOrganizationPrivilege",
        bootPATH + "../AC/Organization/UpdateAccountOrganizationPrivilege",
        bootPATH + "../AC/Organization/RemoveAccountOrganizationPrivileges",
        self);
})(window);