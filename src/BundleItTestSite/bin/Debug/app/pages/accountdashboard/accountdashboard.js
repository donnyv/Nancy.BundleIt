$(document).ready(function () {

    // methods
    function CreateProp(prop) {

        var address = null;
        if (prop.FullAddress) {
            var state = _.find(fcr.States, function (s) { return s.Id == prop.FullAddress.StateId; }).FullName;
            address = {
                Address: prop.FullAddress.Address,
                City: prop.FullAddress.City,
                State: state,
                StateId: prop.FullAddress.StateId,
                Zip: prop.FullAddress.Zip
            };
        }

        return {
            Id: prop.Id,
            Name: prop.Name,
            FullAddress: address
        };
    }

    var _Properties = null;
    function ProcessProperties() {
        
        // store copy
        if (_Properties == null)
            _Properties = JSON.parse(JSON.stringify(fcr.Properties)); //$.extend(true, {}, fcr.Properties);
        else
            fcr.Properties = JSON.parse(JSON.stringify(_Properties));  //$.extend(true, {}, _Properties);

        // sort 
        if (fcr.Properties.length > 0) {
            fcr.Properties = _.sortBy(fcr.Properties, function (p) { return p.Name });
        }

        // get how many rows to create
        var rows = 0;
        if(fcr.Properties.length <= 3)
            rows = 1;

        var d = fcr.Properties.length / 3;
        if((fcr.Properties.length / 3).toString().indexOf(".") == -1)
            rows = d;
        else
            rows = parseInt(d.toString()) + 1;

        // build rows
        var propRows = [];
        var proplist;
            
        // if only 1 row
        if(rows == 1){
            proplist = [];

            for (var j = 0, l = fcr.Properties.length; j < l; j++) {
                proplist.push(CreateProp(fcr.Properties[j]));
            }

            propRows.push({ properties: proplist });
        } else {
            // more then 1 row
            // keep grabbing in batch of 3s
            while (fcr.Properties.length >= 3) {
                proplist = [];

                for (var j = 0; j < 3; j++) {
                    proplist.push(CreateProp(fcr.Properties.shift()));
                }

                propRows.push({ properties: proplist });
            }

            // if any left, grab the remaining
            if(fcr.Properties.length > 0){
                proplist = [];

                for (var j = 0, l = fcr.Properties.length; j < l; j++) {
                    proplist.push(CreateProp(fcr.Properties[j]));
                }

                propRows.push({ properties: proplist });
            }
        }

        return propRows;
    }

    function AddProperty(prop) {
        var propRows = propertylist.data.proprows;
        if (propRows[propRows.length - 1].properties.length == 3) {
            propRows.push({ properties: [CreateProp(prop)] });
        }
        else {
            propRows[propRows.length - 1].properties.push(CreateProp(prop));
        }
    }

    function ShowPropertyForm(isEdituser, formData) {

        // templates
        var propertyForm = new Ractive({
            el: "PropertyForm",
            template: "#PropertyFormTemplate",
            data: {
                isedituser: isEdituser,
                formData: formData,
                States: fcr.States
            }
        });

        // set defaults
        propertyForm.set("FullAddress.StateId", fcr.States[0].Id);

        // events
        // form validation events
        var jqPropertyForm = $("#PropertyForm form");
        jqPropertyForm.parsley().subscribe("parsley:form:validate", function (formInstance) {

            propertyForm.set("is_form_invalid", !formInstance.isValid());

            if (formInstance.isValid()) {

                // create new property
                if (!propertyForm.get("isedituser")) {

                    propertyForm.set("is_sending_ajax_req", true);

                    var newprop = propertyForm.data.formData;
                    accountdashboardBL.AddProperty(newprop, function (result) {

                        propertyForm.set("is_sending_ajax_req", false);

                        if (result.IsError) {
                            propertyForm.set("show_alert_msg", true);
                            propertyForm.set("errormsg", result.StatusMsg);
                            return;
                        }

                        // add property card
                        newprop.Id = result.Data;
                        _Properties.push(newprop);
                        propertylist.data.proprows = ProcessProperties();
                        propertylist.update();

                        // close
                        $("#PropertyForm").modal("hide")

                    });

                }
                // edit property
                else {

                    propertyForm.set("is_sending_ajax_req", true);

                    var prop = propertyForm.data.formData;
                    accountdashboardBL.UpdateProperty(prop, function (result) {

                        propertyForm.set("is_sending_ajax_req", false);

                        if (result.IsError) {
                            propertyForm.set("show_alert_msg", true);
                            propertyForm.set("errormsg", result.StatusMsg);
                            return;
                        }

                        // update property cards
                        _.find(_Properties, function (p,i) {
                            if (p.Id == prop.Id)
                                _Properties[i] = prop;
                        });
                        //existingProperty = prop;
                        propertylist.data.proprows = ProcessProperties();
                        propertylist.update();

                        // close
                        $("#PropertyForm").modal("hide")

                    });

                }

            }

        });

        var Validate = function () {
            jqPropertyForm.parsley().validate();
        }

        propertyForm.on("add", Validate);
        propertyForm.on("save", Validate);

        $("#PropertyForm").modal("show");
    }

    // templates
    var propertylist = new Ractive({
        el: "Properties",
        template: "#PropertyCardsTemplate",
        data: {
            proprows: ProcessProperties()
        }
    });

    // events
    propertylist.on("edit-property", function (e) {
        ShowPropertyForm(true, e.context);
    });

    $("#add_property").on("click", function () {
        ShowPropertyForm(false, null);
    });

});