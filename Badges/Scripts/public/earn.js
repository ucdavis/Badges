(function(badges, $, undefined) {
    "use strict";

    var options = {}, currentCriteriaContainer;

    badges.options = function(o) {
        $.extend(options, o);
    };

    badges.init = function () {
        bindModalEvents();
        createModels();
        ko.applyBindings(badges.Support);
        getMyWork('', function () { });
        bindAssociationEvents();
    };

    function bindModalEvents() {
        $("#badge-criteria").on("click", ".associate-modal", function () {
            currentCriteriaContainer = $(this).parents(".associated-work-container");
            
            $("#current-criterion").html(this.getAttribute("data-criterion"));
        });
    }

    function getMyWork(filter, fnUpdate) {
        $.getJSON(options.MyWorkUrl, filter, function (response) {
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
    
    function bindAssociationEvents() {
        $("#experience-accordion").on('click', '.association', function(e) {
            e.preventDefault();

            var type = this.getAttribute("data-type");
            var text = this.getAttribute("data-text");
            var id = this.id;

            var associatedWork = currentCriteriaContainer.find(".associated-work");
            var name = 'criterion[' + associatedWork.attr("data-index") + '].' + type; //Either .work or .experience

            //Add this work to the proper container, then close the modal
            var workItem = $("<li>", { text: text })
                .append($("<input>", { name: name, value: id, type: 'hidden' }));
            associatedWork.append(workItem);

            $("#associate-work").modal('hide');
        });
    }
}(window.Badges = window.Badges || {}, jQuery));