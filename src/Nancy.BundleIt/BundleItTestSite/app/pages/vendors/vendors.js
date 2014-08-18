$(document).ready(function () {

    // vendor page
    var vendor_page = new Ractive({
        el: "VendorsPage",
        template: "#VendorsPageTemplate",
        data: null
    });

    // event
    vendor_page.on("add-vendor", function (e) {
        fcr.VendorInputForm.show(null, null, {
            onSaveEvent: function (vendor, result) {
                vendor_list.data.Vendors.push(vendor);
                vendor_list.data.SortVendors(true);
            }
        }, null);
    });

    vendor_page.on("add-vendor-contact", function (e) {

    });



    // vendor list
    fcr.Vendors = _.sortBy(fcr.Vendors, function (v) { return v.Name.toLowerCase(); });
    var vendor_list = new Ractive({
        el: "Vendors",
        template: "#VendorsListTemplate",
        data: {
            GetState: toolbox.GetState,
            Vendors: fcr.Vendors,
            SortVendors: function (UpdateView) {
                this.Vendors = _.sortBy(this.Vendors, function (v) { return v.Name.toLowerCase(); });

                if (UpdateView)
                    vendor_list.update();
            }
        }
    });

    // event
    vendor_list.on("edit-vendor", function (e) {
        fcr.VendorInputForm.show(null, e.context, {
            onSaveEvent: function (vendor) {
                vendor_list.data.SortVendors(true);
            }
        }, null);
    });

    vendor_list.on("delete-vendor", function (e) {

        fcr.deletConfirmForm.show({
            Title: "Are you sure you want to delete vendor <b>\"" + e.context.Name + "\"</b> ?",
            ShowCanNotBeUndoneWarning: true,
            onDeleteClickEvent: function (callback) {

                vendorServices.DeleteVendor(e.context.Id, function (result) {

                    callback();

                    if (result.IsError) {
                        toastr.options = {
                            closeButton: true,
                            timeOut: 0,
                            extendedTimeOut: 0,
                            positionClass: "toast-bottom-full-width"
                        };
                        toastr.error(result.Status);
                        return;
                    }

                    fcr.Vendors.splice(e.index.i, 1);

                });

            }
        });

    });

});