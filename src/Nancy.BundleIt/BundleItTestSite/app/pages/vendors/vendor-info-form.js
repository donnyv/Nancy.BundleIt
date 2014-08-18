(function () {

    var VendorInfoForm = {
        show: function (vendor_id, options, callback) {

            // template
            var vendorlist = new Ractive({
                el: "VendorInfoFormModal",
                template: "#VendorInfoFormTemplate",
                data: null
            });

            // events
            vendorlist.on("edit", function (e) {
                options.onEditEvent();
            })

            vendorlist.on("close", function (e) {
                options.onCloseEvent();
            });

            // initialize
            function LoadVendorData(vendorid) {
                vendorServices.GetVendor(vendorid, function (result) {
                    vendorlist.data = result.Data;
                    $("#VendorInfoFormModal").modal("show");
                });
            }
            LoadVendorData(vendor_id)
            
        }
    };


    if (!fcr.VendorInfoForm) {
        fcr.VendorInfoForm = VendorInfoForm;
    }
    
})();