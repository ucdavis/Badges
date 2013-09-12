(function(badges, $, undefined) {
    "use strict";

    var options = {};

    badges.options = function(o) {
        $.extend(options, o);
    };

    badges.init = function() {
        createModels();
        ko.applyBindings(badges.Support);
        getMyWork();
    };
    
    function getMyWork() {
        $.getJSON(options.MyWorkUrl, '', function (response) {
            console.log(response);
            $.each(response, function(i, v) {
                badges.Support.addExperience(v);
            });
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