/// <reference path="../../../jquery-1.8.3.intellisense.js" />
/// <reference path="../../../miniui/miniui.js" />
(function (window) {
    mini.namespace("AC.Property.Index");
    var self = AC.Property.Index;
    self.prifix = "AC_Property_Index_";
    self.sortUrl = bootPATH + "../AC/Property/UpdateSortCode";
    self.add = add;
    self.search = search;
    self.loadData = loadData;
    self.gridReload = function () {
        grid.reload();
    };
    mini.namespace("Property.Edit");
    var edit = Property.Edit;
    edit.SetData = SetData;
    edit.prifix = "AC_Property_Index_Edit_";
    var faceInitialized = false;

    var tabConfigs = {
        infoTab: {
            url: bootPATH + "../AC/Property/Details",
            entityTypeCode: 'Property',
            controller: 'Property',
            params: [{ "pName": 'id', "pValue": "Id" }],
            namespace: "Property.Details"
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
        IsViewField: {
            type: 'boolean',
            comparison: 'eq'
        },
        IsDetailsShow: {
            type: 'boolean',
            comparison: 'eq'
        },
        DicName: {
            type: 'string',
            comparison: 'eq'
        },
        ClrPropertyType: {
            type: 'string',
            comparison: 'like'
        },
        ClrPropertyName: {
            type: 'string',
            comparison: 'like'
        },
        InputType: {
            type: 'string',
            comparison: 'eq'
        }
    };

    mini.parse();

    var btnCreateCommonProperties = mini.get(self.prifix + "btnCreateCommonProperties");
    if (btnCreateCommonProperties) {
        btnCreateCommonProperties.on("click", function () {
            mini.confirm("确定插入通用属性吗？", "确定？", function (action) {
                if (action == "ok") {
                    $.post(bootPATH + "../AC/Property/CreateCommonProperties", { entityTypeID: getParams().entityTypeID }, function (result) {
                        helper.response(result);
                        grid.reload();
                    }, "json");
                }
            });
        });
    }
    var win = mini.get(edit.prifix + "win1");
    var form;
    if (win) {
        form = new mini.Form(edit.prifix + "form1");
    }

    var tabs1 = mini.get(self.prifix + "tabs1");
    var grid = mini.get(self.prifix + "dgField");
    grid.on("drawcell", ondrawcell);
    var helperDrawcell = helper.ondrawcell(self, "AC.Property.Index");
    grid.on("load", helper.onGridLoad);

    helper.index.allInOne(
        edit,
        grid,
        bootPATH + "../AC/Property/Edit",
        bootPATH + "../AC/Property/Edit",
        bootPATH + "../AC/Property/Delete",
        self);
    helper.index.tabInOne(grid, tabs1, tabConfigs, self);

    function add() {
        var data = { action: "new", DicID: '' };
        if (win) {
            win.setTitle("添加");
            win.setIconCls("icon-add");
            win.show();
            data.EntityTypeID = getParams().entityTypeID;
            edit.SetData(data);
        }
    }

    function search() {
        var data = getParams();
        var filterArray = [];
        for (var k in self.filters) {
            var filter = self.filters[k];
            if (filter.value) {
                filterArray.push({ field: k, type: filter.type, comparison: filter.comparison, value: filter.value });
            }
        }
        data.filters = JSON.stringify(filterArray);
        if (!grid.sortField) {
            grid.sortBy("SortCode", "asc");
        }
        grid.load(data);
    }

    function loadData() {
        search();
    }

    function getParams() {
        data = {};
        if (self.params && self.params.entityTypeID) {
            data.entityTypeID = self.params.entityTypeID;
        }
        else {
            data.entityTypeID = $.deparam.fragment().entityTypeID || $.deparam.querystring().entityTypeID
        }
        return data;
    }

    function ondrawcell(e) {
        var field = e.field;
        var value = e.value;
        var record = e.record;
        if (field) {
            switch (field) {
                case "IsEnabled":
                case "IsDetailsShow":
                case "Nullable":
                case "DbIsNullable":
                case "IsConfigValid":
                case "IsViewField":
                    if (value == "正常" || value == "1" || value == "是" || value == "true" || value == true) {
                        e.cellHtml = "<span class='icon-enabled width16px'></span>";
                    } else if (value == "禁用" || value == "0" || value == "否" || value == "false" || value == false) {
                        e.cellHtml = "<span class='icon-disabled width16px'></span>";
                    } break;
                case "Icon":
                    if (value) {
                        e.cellHtml = "<img src='" + bootPATH + "../Content/icons/16x16/" + value + "'></img>";
                    }
                    break;
                case "DicName":
                    if (value) {
                        var url = bootPATH + "../AC/Dic/Details?isTooltip=true&id=" + record.DicID
                        e.cellHtml = "<a href='" + url + "' onclick='helper.cellTooltip(this);return false;' rel='" + url + "'>" + value + "</a>";
                    }
                    break;
            }
        }
        helperDrawcell(e);
    }

    function SetData(data) {
        //跨页面传递的数据对象，克隆后才可以安全使用
        data = mini.clone(data);
        if (data.action == "edit") {
            $.ajax({
                url: bootPATH + "../AC/Property/Get",
                data: { id: data.id },
                cache: false,
                success: function (result) {
                    helper.response(result, function () {
                        form.setData(result);
                        form.validate();
                        if (result.Icon) {
                            $("#msg").html("<img style='margin-top:3px;' src='" + bootPATH + "../Content/icons/16x16/" + result.Icon + "' alt='图标' />");
                        } else {
                            $("#msg").html("");
                        }
                    });
                }
            });
        }
        else if (data.action == "new") {
            form.setData(data);
        }
        if (!faceInitialized) {
            $.getJSON(bootPATH + "../Home/GetIcons", null, function (result) {
                $("#message_face").jqfaceedit({ txtAreaObj: $("#msg"), containerObj: $(mini.get('faceWindow').getBodyEl()), emotions: result, top: 25, left: -27, width: 658, height: 420 });
            });
            faceInitialized = true;
        }
    }

    helper.edit.allInOne(
        self,
        win,
        bootPATH + "../AC/Property/Create",
        bootPATH + "../AC/Property/Update",
        bootPATH + "../AC/Property/Get",
        form, edit);
})(window);
