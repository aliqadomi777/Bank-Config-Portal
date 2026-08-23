$(function () {
    $.validator.addMethod("lessthan", function (value, element, params) {
        if (!value) return true; 

        var namePrefix = element.name.substr(0, element.name.lastIndexOf(".") + 1);
        var otherElementId = "#" + (namePrefix + params.other).replace(/[\.\[\]]/g, "\\$&");
        var otherElement = $(otherElementId);

        if (otherElement.length === 0) return true;

        var otherValue = otherElement.val();
        if (!otherValue) return true; 

        var currentNum = parseFloat(value);
        var otherNum = parseFloat(otherValue);

        if (isNaN(currentNum) || isNaN(otherNum)) return true;

        // 40 < 41 is true (validation passes, error disappears!)
        return currentNum < otherNum;
    });

    $.validator.unobtrusive.adapters.add("lessthan", ["other"], function (options) {
        options.rules["lessthan"] = {
            other: options.params.other
        };
        options.messages["lessthan"] = options.message;
    });

    // Whenever ANY input changes, we check if it is a 'maximum' target for a 'lessthan' field
    $(document).on("change keyup", "input", function () {
        var alteredElement = this;
        var alteredName = alteredElement.name;

        if (!alteredName) return;

        // Look for any 'minimum' field on the page that points to this field as its 'other' property
        $("input[data-val-lessthan]").each(function () {
            var minElement = this;
            var namePrefix = minElement.name.substr(0, minElement.name.lastIndexOf(".") + 1);
            var targetMaxName = namePrefix + $(minElement).attr("data-val-lessthan-other");

            // If the field that just changed IS the maximum field for this minimum field...
            if (alteredName === targetMaxName) {
                var form = $(minElement).closest("form");
                if (form.length > 0) {
                    var validator = form.validate();
                    if (validator) {
                        // Force the minimum field to re-evaluate right now!
                        validator.element(minElement);
                    }
                }
            }
        });
    });
}(jQuery));
