(function(badges, $, undefined) {
    "use strict";

    var options = {}, currentCriteriaContainer, workIsotope;

    badges.options = function(o) {
        $.extend(options, o);
    };

    badges.init = function () {
        bindModalEvents();
        createModels();
        ko.applyBindings(badges.Associate, document.getElementById("badge-criteria"));
        ko.applyBindings(badges.Support, document.getElementById('associate-work'));
        getCriteria();
        getExperiences(options.MyExperiencesUrl, {});
    };
    
    function bindModalEvents() {
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
    
    function getCriteria() {
        $.getJSON(options.CriteriaUrl, {}, function (response) {
            $.each(response, function(i, v) {
                badges.Associate.addCriterion(v);
            });
            
            workIsotope = $('.associated-work').isotope({
                // options
                itemSelector: '.work-item',
                layoutMode: 'masonry'
            });
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

            self.associateExperience = function(exp) {
                var fulfillment = new badges.Fulfillment({ Details: exp.name, WorkId: exp.id, ExperienceId: exp.id, WorkType: 'experience' });
                badges.Associate.associateWithCurrentCriterion(fulfillment);
            };
            
            self.associateWork = function (work) {
                var fulfillment = new badges.Fulfillment({ Details: work.Description, WorkId: work.Id, ExperienceId: work.experienceId, WorkType: 'work', SupportType: work.Type });
                badges.Associate.associateWithCurrentCriterion(fulfillment);
            };
        };

        badges.Support = new function() {
            var self = this;
            self.criteriaDetails = ko.observable();
            self.criteriaIndex = ko.observable();
            self.experiences = ko.observableArray([]);

            self.addExperience = function(experience) {
                self.experiences.push(new badges.Experience(experience));
            };
        };

        badges.Fulfillment = function (fulfillment) {
            var self = this;
            self.comment = fulfillment.Comment;
            self.details = fulfillment.Details;
            self.experienceid = fulfillment.ExperienceId;
            self.workid = fulfillment.WorkId;
            self.worktype = fulfillment.WorkType;
            self.type = fulfillment.SupportType;
        };

        badges.Criterion = function (criterion) {
            var self = this;
            self.id = criterion.Criteria.Id;
            self.details = criterion.Criteria.Details;
            
            self.fulfillments = ko.observableArray([]);

            self.addFulfillment = function(fulfillment) {
                self.fulfillments.push(new badges.Fulfillment(fulfillment));
            };

            self.removeFulfillment = function(fulfillment) {
                self.fulfillments.remove(fulfillment);
                recomputeAssociationsIsotope();
            };

            $.each(criterion.Fulfillments, function(i, v) {
                self.addFulfillment(v);
            });
        };

        badges.Associate = new function() {
            var self = this;
            self.selectedCriterion = ko.observable();
            self.criteria = ko.observableArray([]);

            self.addCriterion = function(criterion) {
                self.criteria.push(new badges.Criterion(criterion));
            };

            self.associateWork = function(index, criterion) {
                self.selectedCriterion(criterion);
                badges.Support.criteriaDetails(criterion.details);
                badges.Support.criteriaIndex(index + 1);
            };

            self.associateWithCurrentCriterion = function(fulfillment) {
                var currentCriterion = self.selectedCriterion();
                currentCriterion.fulfillments.push(fulfillment);

                recomputeAssociationsIsotope();
            };
        };
    }
    
    function recomputeAssociationsIsotope() {
        workIsotope.isotope('destroy'); //TODO: (maybe redo layout on modal close?)
        workIsotope = $('.associated-work').isotope({
            // options
            itemSelector: '.work-item',
            layoutMode: 'masonry'
        });
    }
}(window.Badges = window.Badges || {}, jQuery));