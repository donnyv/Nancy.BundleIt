(function () {

    var toolbox = {
        NumberFormats:{
            money: "0,0.00"
        },
        IsNumeric: function (e,includeDecimal) {
            var specialKeys = new Array();
            specialKeys.push(8); //Backspace

            if (includeDecimal)
                specialKeys.push(46); // Decimal

            var keyCode = e.which ? e.which : e.keyCode
            var ret = ((keyCode >= 48 && keyCode <= 57) || specialKeys.indexOf(keyCode) != -1);

            // check if another decimal exists
            if (includeDecimal && ret && keyCode == 46) {
                var r = e.currentTarget.value.indexOf(".") != -1 ? false : true;

                if(!r)
                    e.preventDefault();
            }

            if (!ret)
                e.preventDefault();
        },
        GetState: function (StateId) {
            return _.find(fcr.States, function (s) { return s.Id == StateId; }).FullName;
        }
    };

    if (!window.toolbox) {
        window.toolbox = toolbox;
    }

})();