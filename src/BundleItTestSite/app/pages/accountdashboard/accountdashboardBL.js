(function () {

    var accountdashboardBL = {
        AddProperty: function (NewProp, callback) {

            var errorMsg = "Missing a parameter!";
            if (NewProp == null) { throw errorMsg; }
            if (callback == null) { throw errorMsg; }

            $.ajax({
                type: "POST",
                url: "account-dashboard/add-property",
                dataType: "json",
                data: JSON.stringify(NewProp),
                contentType: 'application/json; charset=utf-8',
                processData: false,
                cache: false
            }).done(function (result) {
                callback(result);
            }).fail(function (result) {
                callback({ IsError: true, Status: result.statusText });
            });

        },
        UpdateProperty: function (Prop, callback) {

            var errorMsg = "Missing a parameter!";
            if (Prop == null) { throw errorMsg; }
            if (callback == null) { throw errorMsg; }

            $.ajax({
                type: "PUT",
                url: "account-dashboard/update-property",
                dataType: "json",
                data: JSON.stringify(Prop),
                contentType: 'application/json; charset=utf-8',
                processData: false,
                cache: false
            }).done(function (result) {
                callback(result);
            }).fail(function (result) {
                callback({ IsError: true, Status: result.statusText });
            });

        }
    };

    if (!window.accountdashboardBL) {
        window.accountdashboardBL = accountdashboardBL;
    }

})();