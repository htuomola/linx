function AppDataModel() {
    var self = this;

    self.getLinks = function () {
        return $.ajax("/api/links/getlinks");
    };

    self.getMoreLinks = function (oldestLoadedId) {
        return $.ajax("/api/links/getLinksOlderThan/" + oldestLoadedId);
    };
}

function LinkViewModel(app, model) {
    var self = this;

    self.url = model.Url;
    self.urlText = model.Url.substring(0, 20) + '...';
    self.postedAt = moment.utc(model.PostedAt).fromNow();
    self.channel = model.Channel;
    self.user = model.User;
    self.id = model.Id;
    self.title = model.Title;
}

function AppViewModel(dataModel) {
    var self = this;

    self.links = ko.observableArray();

    self.loadingLinks = ko.observable(false);
    
    self.loadingMoreLinks = ko.observable(false);
    self.hasMoreItems = ko.observable(true);
    
    self.init = function () {
        self.loadingLinks(true);
        dataModel.getLinks()
            .done(function (data) {
                if (typeof (data) === "object") {
                    for (var i = 0; i < data.length; i++) {
                        self.links.push(new LinkViewModel(app, data[i]));
                    }
                    var initialPageSize = 20;
                    self.hasMoreItems(data.length == initialPageSize);
                } else {
                    //self.errors.push("An unknown error occurred.");
                    // TODO: notify error
                }
            })
            .fail(function () {
                // TODO: notify error
            });
        self.loadingLinks(false);
    };

    self.loadMoreItems = function() {
        if (self.loadingLinks()) return;
        self.loadingMoreLinks(true);

        var lastLoadedId = self.links()[self.links().length-1].id;
        //var lastLoadedId = 10;

        dataModel.getMoreLinks(lastLoadedId)
            .done(function(data) {
                if (typeof(data) === "object") {
                    for (var i = 0; i < data.length; i++) {
                        self.links.push(new LinkViewModel(app, data[i]));
                    }
                    var loadMoreItemsPageSize = 10;
                    self.hasMoreItems(data.length == loadMoreItemsPageSize);
                } else {
                    //self.errors.push("An unknown error occurred.");
                    // TODO: notify error
                }
            })
            .fail(function() {
                // TODO: notify error
            });

        self.loadingMoreLinks(false);
    };

    self.linkAdded = function (link) {
        var vm = new LinkViewModel(app, link);
        self.links.unshift(vm);
    };
}

var app = new AppViewModel(new AppDataModel());

$(function () {
    // Reference the auto-generated proxy for the hub.
    var linkHub = $.connection.linkHub;
    // Create a function that the hub can call back for notifying of new links.
    linkHub.client.addNewLink = function (link) {
        app.linkAdded(link);
    };

    // Start the connection.
    $.connection.hub.start().done(function () {
        // Initialize client->server event handlers here
        // i.e.
        // Call the Send method on the hub.
        // linkHub.server.send('foobar');
    });
});


$(window).scroll(function (e) {
    //if ($(window).data('ajaxready') == false) return;

    if (($(window).scrollTop() + $(window).height()) >= $(document).height() * 0.9) {
        //$('div#loadmoreajaxloader').show();
        //$(window).data('ajaxready', false);
        if(app.hasMoreItems())
            app.loadMoreItems();
        //$.ajax({
        //    cache: false,
        //    url: 'loadmore.php?lastid=' + $('.postitem:last').attr('id'),
        //    success: function (html) {
        //        if (html) {
        //            $('#postswrapper').append(html);
        //            $('div#loadmoreajaxloader').hide();
        //        } else {
        //            $('div#loadmoreajaxloader').html();
        //        }
        //        $(window).data('ajaxready', true);
        //    }
        //});
    }
});
