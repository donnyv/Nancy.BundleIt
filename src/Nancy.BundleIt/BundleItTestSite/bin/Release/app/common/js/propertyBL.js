(function () {

    var propertyBL = {
        DeleteProperty: function (PropertyId, callback) {

            var errorMsg = "Missing a parameter!";
            if (PropertyId == null) { throw errorMsg; return; }
            if (callback == null) { throw errorMsg; return; }

            $.ajax({
                type: "DELETE",
                url: "/property/delete/" + PropertyId,
                processData: false,
                cache: false
            }).done(function (result) {
                callback(result);
            });

        }
    };

    if (!fcr.propertyBL) {
        fcr.propertyBL = propertyBL;
    }

})();