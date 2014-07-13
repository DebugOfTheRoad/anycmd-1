/// <reference path="../../../jquery-1.8.3.intellisense.js" />
/// <reference path="../../../miniui/miniui.js" />
/// <reference path="../../../jquery-bbq/jquery.ba-bbq.js" />
(function (window, $) {
    mini.namespace("Account.ContractorAccounts");
    mini.namespace("Account.ContractorAccounts");
    var self = Account.ContractorAccounts;
    self.prifix = "AC_Account_ContractorAccounts_"
    self.loadData = loadData;
    self.enable = enable;
    self.disable = disable;
    self.gridReload = function () {
        grid.reload();
    };

    mini.namespace("Account.ContractorAccounts.Edit");
    var edit = Account.ContractorAccounts.Edit;
    edit.prifix = "AC_Account_ContractorAccounts_Edit_";
    edit.SaveData = SaveData;
    edit.SetData = SetData;

    mini.parse();

    var win = mini.get(edit.prifix + "win1");
    var form;
    if (win) {
        form = new mini.Form(edit.prifix + "form1");
    }

    var grid = mini.get(self.prifix + "datagrid1");
    grid.on("drawcell", helper.ondrawcell(self, "Account.ContractorAccounts"));
    grid.on("load", helper.onGridLoad);

    helper.index.allInOne(
        edit,
        grid,
        bootPATH + "../AC/Account/Edit",
        bootPATH + "../AC/Account/Edit",
        bootPATH + "../AC/Account/Delete",
        self);

    function loadData() {
        var data = getParams();
        if (!grid.url) {
            grid.url = bootPATH + "../AC/Account/GetAccountsByContractorID";
        }
        if (!grid.sortField) {
            grid.sortBy("CreateOn", "desc");
        }
        grid.load(data);
    }

    function getParams() {
        if (self.params && self.params.contractorID) {
            return self.params;
        }
        else {
            return { contractorID: $.deparam.fragment().contractorID || $.deparam.querystring().contractorID };
        }
    }

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

    function SetData(data) {
        //跨页面传递的数据对象，克隆后才可以安全使用
        data = mini.clone(data);
        if (data.action == "edit") {
            $.ajax({
                url: bootPATH + "../AC/Account/Get",
                data: { id: data.id },
                cache: false,
                success: function (result) {
                    helper.response(result, function () {
                        form.setData(result);
                        form.validate();
                    });
                }
            });
            var fields = form.getFields();
            for (var i = 0, l = fields.length; i < l; i++) {
                var c = fields[i];
                if (c.name == "Password" || c.name == "LoginName") {
                    if (c.setReadOnly) c.setReadOnly(true);     //只读
                    if (c.setIsValid) c.setIsValid(true);      //去除错误提示
                    if (c.addCls) c.addCls("asLabel");          //增加asLabel外观
                }
            }
        }
        else if (data.action == "new") {
            var fields = form.getFields();
            for (var i = 0, l = fields.length; i < l; i++) {
                var c = fields[i];
                if (c.name == "Password" || c.name == "LoginName") {
                    if (c.setReadOnly) c.setReadOnly(false);     //只读
                    if (c.setIsValid) c.setIsValid(true);      //去除错误提示
                    if (c.addCls) c.removeCls("asLabel");          //增加asLabel外观
                }
            }
            form.setData(data);
        }
    }

    function SaveData() {
        var contractorID = getParams().contractorID;
        if (contractorID) {
            $("#" + edit.prifix + "form1 input[name='contractorID']").val(contractorID);
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
    helper.edit.allInOne(
        self,
        win,
        bootPATH + "../AC/Account/Create",
        bootPATH + "../AC/Account/Update",
        bootPATH + "../AC/Account/Get",
        form, edit);
})(window, jQuery);