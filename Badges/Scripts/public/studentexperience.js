(function (badges, $, undefined) {
    "use strict";

    var workFields = {
        "text": ["notes"],
        "photo": ["url", "file"],
        "link": ["url"],
        "video": ["url"],
        "audio": ["file"],
        "file": ["file"]
    };

    var options = {};

    badges.options = function (o) {
        $.extend(options, o);
    };

    badges.init = function () {
        configureWork();
        configureOutcomes();
    };

    function configureOutcomes() {
        $('#outcome-add').on('click', function() {
            // reset the notes when modal is shown
            $("#notes").val(null);
            $("#existingOutcomeId").val(null);

            $("#outcome-submit").val("Add Outcome");
        });

        $("#outcomes").on('click', '.outcome-edit', function(e) {
            var outcome = this.getAttribute("data-name");
            var notes = this.getAttribute("data-note");
            var outcomeid = this.getAttribute("data-id");

            //Find the popup, set the existing values
            $("#notes").val(notes);
            $("#existingOutcomeId").val(outcomeid);
            $("#outcomeid option").filter(function() {
                return $(this).text() == outcome; 
            }).prop('selected', true);
            
            $("#outcome-submit").val("Edit Outcome");
        });
    }

    function configureWork() {
        $("#add-work").on("click", "a", function(e) {
            var worktype = this.getAttribute("data-work");
            var fields = workFields[worktype];

            $(".work-optional", "#supporting-work-fields").hide();

            for (var i = 0; i < fields.length; i++) {
                $("#work-" + fields[i]).show();
            }

            $("#work-type").val(worktype);
            $("#work-helptext").html(this.getAttribute("data-helptext"));
        });

        $("#work-file").on('change', 'input:file', function(e) {
            var files = e.currentTarget.files;

            if (files.length > 0) {
                var fileSize = ((files[0].size / 1024) / 1024).toFixed(4); //file size in MB

                if (fileSize > 10.0) {
                    $("#upload-error").html("Sorry, the maximum upload size is 10.0MB").show();
                    this.value = null;
                } else {
                    $("#upload-error").empty().hide();
                }
            }
        });
    }

}(window.Badges = window.Badges || {}, jQuery));