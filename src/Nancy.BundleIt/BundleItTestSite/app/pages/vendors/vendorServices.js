(function () {

    var vendorServices = {
        AddVendor: function (NewVendor, callback) {

            var errorMsg = "Missing a parameter!";
            if (NewVendor == null) { throw errorMsg; }
            if (callback == null) { throw errorMsg; }

            $.ajax({
                type: "POST",
                url: "/vendors/add",
                dataType: "json",
                data: JSON.stringify(NewVendor),
                contentType: 'application/json; charset=utf-8',
                processData: false,
                cache: false
            }).done(function (result) {
                callback(result);
            }).fail(function (result) {
                callback({ IsError: true, Status: result.statusText });
            });

        },
        UpdateVendor: function (Vendor, callback) {

            var errorMsg = "Missing a parameter!";
            if (Vendor == null) { throw errorMsg; }
            if (callback == null) { throw errorMsg; }

            $.ajax({
                type: "PUT",
                url: "/vendors/update",
                dataType: "json",
                data: JSON.stringify(Vendor),
                contentType: 'application/json; charset=utf-8',
                processData: false,
                cache: false
            }).done(function (result) {
                callback(result);
            }).fail(function (result) {
                callback({ IsError: true, Status: result.statusText });
            });

        },
        DeleteVendor: function (VendorId, callback) {

            var errorMsg = "Missing a parameter!";
            if (VendorId == null) { throw errorMsg; return; }
            if (callback == null) { throw errorMsg; return; }

            $.ajax({
                type: "DELETE",
                url: "/vendors/delete/" + VendorId,
                processData: false,
                cache: false
            }).done(function (result) {
                callback(result);
            }).fail(function (result) {
                callback({ IsError: true, Status: result.statusText });
            });

        },
        GetVendor: function (vendor_id, callback){
            //$.getJSON("/vendors/" + vendor_id, function (data) {
            //    callback(data);
            //});
            callback();
        }
    };

    if (!window.vendorServices) {
        window.vendorServices = vendorServices;
    }

})();