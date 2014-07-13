/// <reference path="../../../jquery-1.8.3.intellisense.js" />
/// <reference path="../../../miniui/miniui.js" />
/// <reference path="../../../helper.js" />
/// <reference path="../../../jquery-bbq/jquery.ba-bbq.js" />
(function (window) {
    mini.namespace("Node.NodeStateCodes");
    var self = Node.NodeStateCodes;
    self.prifix = "EDI_Node_NodeStateCodes_";
    self.loadData = loadData;
    var nodeID = $.deparam.fragment().nodeID || $.deparam.querystring().nodeID || '';

    mini.parse();
    
    function loadData() {
        var data = getParams();
    }

    function getParams() {
        data = { key: key.getValue() };
        if (self.params && self.params.nodeID) {
            data.nodeID = self.params.nodeID;
        }
        else {
            var fragment = $.deparam.fragment();
            var querystring = $.deparam.querystring();
            data.nodeID = fragment.nodeID || querystring.nodeID;
        }
        return data;
    }
})(window);