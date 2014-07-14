/// <reference path="../../../jquery-1.8.3.intellisense.js" />
/// <reference path="../../../miniui/miniui.js" />
/// <reference path="../../../jquery-bbq/jquery.ba-bbq.js" />
(function (window) {
    mini.namespace("Account.Organizations");
    var self = Account.Organizations;
    self.prifix = "AC_Account_Organizations_";
    self.loadData = loadData;

    mini.parse();

    function loadData() {
        
    }

    function getParams() {
        var data = { };
        if (self.params && self.params.accountID) {
            data.accountID = self.params.accountID;
        }
        else {
            data.accountID = $.deparam.fragment().accountID || $.deparam.querystring().accountID;
        }
        return data;
    }
})(window);