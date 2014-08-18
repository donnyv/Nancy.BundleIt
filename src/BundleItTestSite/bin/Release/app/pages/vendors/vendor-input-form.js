(function () {

    var VendorInputForm = {
        show: function (vendor_id, vendor, options, callback) {

            var jqVendorForm = null;
            var defaultOptions = {
                onAddEvent: false,
                onSaveEvent: false,
                onCloseEvent: false
            }

            // set options
            options = $.extend(true, defaultOptions, options);

            // globals
            var VendorInputFormModal = $("#VendorInputFormModal");
            var mode = vendor_id == null && vendor == null ? 0 : 1;

            // template
            var vendor_input_form = new Ractive({
                el: "VendorInputFormModal",
                template: "#VendorInputFormTemplate",
                data: {
                    FormData: {
                        Address: {
                            StateId: fcr.States[0].Id
                        }
                    },
                    mode: mode,
                    states: fcr.States
                },
                complete: function () {

                    // initialize
                    jqVendorForm = $("#VendorInputFormModal form").parsley();
                    jqVendorName = $("#vifName");

                    jqVendorName.popover({
                        trigger: "manual"
                    });

                    // form validation events
                    jqVendorForm.subscribe("parsley:field:error", function (fieldInstance) {

                        if (fieldInstance.$element[0].id == "vifName" && $.trim(jqVendorName.val()) != "") {

                            jqVendorName.popover('destroy');

                            jqVendorName.popover({
                                trigger: "manual",
                                content: "This vendor already exists."
                            });

                            jqVendorName.popover("show");
                        }

                    });

                    jqVendorForm.subscribe("parsley:field:success", function (fieldInstance) {

                        if (fieldInstance.$element[0].id == "vifName")
                            jqVendorName.popover("hide");

                    });

                }
            });


            // events
            vendor_input_form.on("add", function (e) {

                jqVendorForm.asyncValidate()
                    .done(function () {
                        vendor_input_form.set("sending_request", true);
                        vendorServices.AddVendor(e.context.FormData, function (result) {

                            vendor_input_form.set("sending_request", false);

                            if (result.IsError) {
                                vendor_input_form.set("error_msg", result.Status);
                            }

                            if (options.onSaveEvent) {
                                e.context.FormData.Id = result.Data;
                                options.onSaveEvent(e.context.FormData);
                            }

                            VendorInputFormModal.modal("hide");

                        });

                    }).fail(function (result) {
                        vendor_input_form.set("error_msg", result.statusText);
                    });

            })

            vendor_input_form.on("is-number", function (e) {
                toolbox.IsNumeric(e.original, false);
            });

            vendor_input_form.on("clear-form-alert", function (e) {
                vendor_input_form.set("error_msg", null);
            })

            vendor_input_form.on("save", function (e) {
                jqVendorForm.validate();
            });

            vendor_input_form.on("close", function (e) {

                if (options.onCloseEvent)
                    options.onCloseEvent();

            });

            

            // methods
            function GetVendorData(vendorid) {

                vendorServices.GetVendor(vendorid, function (result) {
                    vendor_input_form.set({
                        FormData: result
                    });

                    $("#VendorInputFormModal").modal("show");
                });

            }

            function LoadVendorData(vendor){
                vendor_input_form.set({
                    FormData: vendor
                });

                $("#VendorInputFormModal").modal("show");
            }

            // get vendor from server if provided an id
            if (vendor_id)
                GetVendorData(vendor_id);

            // load local vendor
            if (vendor)
                LoadVendorData(vendor);
            
            if (vendor_id == null && vendor == null)
                VendorInputFormModal.modal("show");
            
        }
    };


    if (!fcr.VendorInputForm) {
        fcr.VendorInputForm = VendorInputForm;
    }
    
})();