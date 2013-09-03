(function (badges, $, undefined) {
    "use strict";

    var options = {};

    badges.options = function (o) {
        $.extend(options, o);
    };

    badges.init = function () {
        $("#add-work").on("click", "a", function(e) {
            console.log(this.getAttribute("data-work"));
        });
    };

}(window.Badges = window.Badges || {}, jQuery));