(function(badges, $, undefined) {
    "use strict";

    var options = {}, currentCriteria;

    badges.options = function(o) {
        $.extend(options, o);
    };

    badges.init = function () {
        bindModalEvents();
        createModels();
        ko.applyBindings(badges.Support);
        getMyWork('', function () { });
    };

    function bindModalEvents() {
        $("#badge-criteria").on("click", ".associate-modal", function () {

            currentCriteria = $(this).parents(".associated-work-container");
            console.log(currentCriteria);
            
            $("#current-criterion").html(this.getAttribute("data-criterion"));
        });
    }

    function getMyWork(filter, fnUpdate) {
        $.getJSON(options.MyWorkUrl, filter, function (response) {
            console.log(response);
            $.each(response, function(i, v) {
                badges.Support.addExperience(v);
            });
            $("#work-loading-indicator").hide();
            fnUpdate.call();
        });
    }

    function createModels() {
        badges.Experience = function(experience) {
            var self = this;
            self.id = experience.Id;
            self.name = experience.Name;
            self.description = experience.Description;
            self.coverImageUrl = experience.CoverImageUrl;

            self.work = experience.Work;
        };

        badges.Support = new function() {
            var self = this;
            self.experiences = ko.observableArray([]);

            self.addExperience = function(experience) {
                self.experiences.push(new badges.Experience(experience));
            };
        };
    }
}(window.Badges = window.Badges || {}, jQuery));