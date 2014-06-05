(function (badges, $, undefined) {
    "use strict";

    var options = {}, geocoder, map, marker;

    badges.options = function (o) {
        $.extend(options, o);
    };

    badges.init = function () {
        initializeTypeahead();
        initializeImagePreview();
        $(".datepicker").datepicker({ format: 'm/d/yyyy', autoclose: true });
        initializeMap();
    };

    function initializeTypeahead() {
        $("#Experience_Name").typeahead({
            name: 'title',
            prefetch: options.TitlesUrl,
            limit: 10
        });

        $("#Experience_Organization").typeahead({
            name: 'orgs',
            prefetch: options.OrganizationsUrl,
            limit: 10
        });
    }
    
    function initializeImagePreview() {
        $("#cover-image").change(function () {
            var filesToUpload = this.files;
            if (filesToUpload.length !== 1) return;

            var file = filesToUpload[0];

            if (!file.type.match(/image.*/)) {
                alert("only images, please");
            }
            

            var img = document.getElementById("cover-preview");
            img.src = window.URL.createObjectURL(file);
            img.onload = function(e) {
                window.URL.revokeObjectURL(this.src);
            };
            //img.height = 500;
        });
    }

    function initializeMap() {
        var davisLatLng = new google.maps.LatLng(38.5449065, -121.7405167);
        geocoder = new google.maps.Geocoder();
        var mapOptions = {
            center: davisLatLng,
            zoom: 10,
            mapTypeId: google.maps.MapTypeId.ROADMAP
        };

        map = new google.maps.Map(document.getElementById("map-canvas"),
            mapOptions);
        marker = new google.maps.Marker({
            map: map,
            position: davisLatLng
        });

        $("#Experience_Location").blur(function () {
            var address = this.value;
            geocoder.geocode({ 'address': address }, function (results, status) {
                if (status == google.maps.GeocoderStatus.OK) {
                    if (marker) marker.setMap(null); //clear existing marker
                    map.setCenter(results[0].geometry.location);
                    marker = new google.maps.Marker({
                        map: map,
                        position: results[0].geometry.location
                    });
                    if (results[0].geometry.viewport)
                        map.fitBounds(results[0].geometry.viewport);
                } else {
                    alert('Geocode was not successful for the following reason: ' + status);
                }
            });
        });
    }
}(window.Badges = window.Badges || {}, jQuery));