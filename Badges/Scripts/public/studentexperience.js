(function (badges, $, undefined) {
    "use strict";

    var workFields = {
        "photo": ["url", "file"],
        "video": ["url"],
        "text": ["notes"]
    };

    var options = {};

    badges.options = function (o) {
        $.extend(options, o);
    };

    badges.init = function () {
        $("#add-work").on("click", "a", function(e) {
            var worktype = this.getAttribute("data-work");
            var fields = workFields[worktype];

            $(".work-optional", "#supporting-work-fields").hide();

            for (var i = 0; i < fields.length; i++) {
                $("#work-" + fields[i]).show();
            }

            $("#work-type").val(worktype);
        });
    };

}(window.Badges = window.Badges || {}, jQuery));