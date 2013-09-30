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
        getExperiences(options.MyExperiencesUrl, {});
        bindAssociationEvents();
    };

    function bindModalEvents() {
        $("#badge-criteria").on("click", ".associate-modal", function () {
            currentCriteriaContainer = $(this).parents(".criterion");
            
            $("#current-criterion").html(this.getAttribute("data-criterion"));
        });

        var ttl = 10000; //cache results for 1 minute

        $("#searchbox").typeahead([
            {
                name: 'experiences',
                prefetch: { url: options.TypeaheadExperiencesUrl, ttl: ttl },
                header: '<h4 class="typeahead-worktype">Experiences</h3>'
            },
            {
                name: 'work',
                prefetch: { url: options.TypeaheadWorkUrl, ttl: ttl },
                header: '<h4 class="typeahead-worktype">Supporting Work</h3>'
            }
        ]).on('typeahead:selected typeahead:autocompleted', function(event, datum, dataset) {
            badges.Support.experiences.removeAll();
            getExperiences(dataset === 'experiences' ? options.MyExperiencesUrl : options.MyWorkUrl, { filter: datum.value });
        });
    }

    function getExperiences(url, filter) {
        $.getJSON(url, filter, function (response) {
            $.each(response, function (i, v) {
                badges.Support.addExperience(v);
            });
            $("#work-loading-indicator").hide();
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
            var name = 'criterion[' + associatedWork.attr("data-index") + '].';
            var idName = name + type;
            var commentName = name + 'comment';

            //Add this work to the proper container, then close the modal
            var workItem = $("<li>", { text: text })
                .append($("<input>", { name: idName, value: id, type: 'hidden' }))
                .append($("<input>", { name: commentName, type: 'text', placeholder: 'Comment' }));

            associatedWork.append(workItem);

            $("#associate-work").modal('hide');
        });
    }
}(window.Badges = window.Badges || {}, jQuery));