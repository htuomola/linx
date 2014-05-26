///<reference path="../typings/jquery/jquery.d.ts" />
///<reference path="../typings/signalr/signalr.d.ts" />
///<reference path="../typings/MySignalRHubs.d.ts" />
var LinkApp;
(function (LinkApp) {
    var AppDataModel = (function () {
        function AppDataModel() {
        }
        AppDataModel.prototype.getLinks = function () {
            return $.ajax("/api/links/");
        };

        AppDataModel.prototype.getMoreLinks = function (oldestLoadedId) {
            return $.ajax("/api/links/before/" + oldestLoadedId);
        };
        return AppDataModel;
    })();
    LinkApp.AppDataModel = AppDataModel;

    var LinkViewModel = (function () {
        function LinkViewModel(app, model) {
            this.url = model.Url;
            this.urlText = model.Url.substring(0, 20) + '...';
            this.postedAt = moment.utc(model.PostedAt).fromNow();
            this.channel = model.Channel;
            this.user = model.User;
            this.id = model.Id;
            this.title = model.Title;
        }
        return LinkViewModel;
    })();
    LinkApp.LinkViewModel = LinkViewModel;

    var AppViewModel = (function () {
        function AppViewModel(dataModel) {
            this.links = ko.observableArray();
            this.loadingLinks = ko.observable(false);
            this.loadingMoreLinks = ko.observable(false);
            this.hasMoreItems = ko.observable(true);
            this.dataModel = dataModel;
        }
        AppViewModel.prototype.init = function () {
            this.loadingLinks(true);
            var self = this;
            this.dataModel.getLinks().done(function (data) {
                if (typeof (data) === "object") {
                    for (var i = 0; i < data.length; i++) {
                        self.links.push(new LinkViewModel(self, data[i]));
                    }
                    var initialPageSize = 20;
                    self.hasMoreItems(data.length == initialPageSize);
                } else {
                    //self.errors.push("An unknown error occurred.");
                    // TODO: notify error
                }
            }).fail(function () {
                // TODO: notify error
            });
            this.loadingLinks(false);
        };

        AppViewModel.prototype.loadMoreItems = function () {
            if (this.loadingLinks())
                return;
            this.loadingMoreLinks(true);
            var self = this;
            var lastLoadedId = this.links()[this.links().length - 1].id;

            this.dataModel.getMoreLinks(lastLoadedId).done(function (data) {
                if (typeof (data) === "object") {
                    for (var i = 0; i < data.length; i++) {
                        self.links.push(new LinkViewModel(self, data[i]));
                    }
                    var loadMoreItemsPageSize = 10;
                    self.hasMoreItems(data.length == loadMoreItemsPageSize);
                } else {
                    //self.errors.push("An unknown error occurred.");
                    // TODO: notify error
                }
            }).fail(function () {
                // TODO: notify error
            });

            this.loadingMoreLinks(false);
        };

        AppViewModel.prototype.linkAdded = function (link) {
            var vm = new LinkViewModel(this, link);
            this.links.unshift(vm);
        };
        return AppViewModel;
    })();
    LinkApp.AppViewModel = AppViewModel;
})(LinkApp || (LinkApp = {}));

var linkApp = new LinkApp.AppViewModel(new LinkApp.AppDataModel());

$(function () {
    // Reference the auto-generated proxy for the hub.
    var linkHub = $.connection.linkHub;

    // Create a function that the hub can call back for notifying of new links.
    linkHub.client.addNewLink = function (link) {
        linkApp.linkAdded(link);
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
    if (($(window).scrollTop() + $(window).height()) >= $(document).height() * 0.9) {
        if (linkApp.hasMoreItems())
            linkApp.loadMoreItems();
    }
});
//# sourceMappingURL=app.js.map
