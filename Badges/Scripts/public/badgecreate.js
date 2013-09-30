(function(badges, $, undefined) {
    "use strict";

    var options = {};

    badges.options = function(o) {
        $.extend(options, o);
    };

    badges.init = function() {
        createModels();
        createBindingHandlers();
        ko.applyBindings(badges.Badge);
        bindCategoryHandlers();
    };

    function createModels() {
        badges.Criterion = function(description) {
            var self = this;
            self.description = ko.observable(description);

        };

        badges.Badge = new function() {
            var self = this;
            self.criteria = ko.observableArray([new badges.Criterion("")]);

            self.addCriterion = function() {
                if (self.criteria().length < 10) {
                    self.criteria.push(new badges.Criterion(""));
                }
            };

            //Valid if we have at least one criteria and 
            self.valid = ko.computed(function() {
                var validCriterion = $.grep(self.criteria(), function(value) {
                    return value.description().length > 0;
                });
                return validCriterion.length > 0;
            });
        };
    }

    function createBindingHandlers() {
        ko.bindingHandlers.executeOnEnter = {
            init: function(element, valueAccessor, allBindingsAccessor, viewModel) {
                var allBindings = allBindingsAccessor();
                $(element).keypress(function(event) {
                    var keyCode = (event.which ? event.which : event.keyCode);
                    if (keyCode === 13) {
                        allBindings.executeOnEnter.call(viewModel, viewModel, event);
                        return false;
                    }
                    return true;
                });
            }
        };
    }
    
    function bindCategoryHandlers() {
        $("#Category").on('change', function() {
            var selected = $("option:selected", this);
            document.getElementById("category-image").src = selected.attr("data-img");
        });
    }
}(window.Badges = window.Badges || {}, jQuery));