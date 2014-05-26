///<reference path="../typings/jquery/jquery.d.ts" />
///<reference path="../typings/signalr/signalr.d.ts" />
///<reference path="../typings/MySignalRHubs.d.ts" />
module LinkApp {

    export class AppDataModel {
        getLinks() { return $.ajax("/api/links/getlinks"); }

        getMoreLinks(oldestLoadedId: number) { return $.ajax("/api/links/getLinksOlderThan/" + oldestLoadedId); }
    }

    export class LinkViewModel {
        url: string;
        urlText: string;
        postedAt: string;
        channel: string;
        user: string;
        id : number;
        title: string;

        constructor(app: AppViewModel, model) {
            this.url = model.Url;
            this.urlText = model.Url.substring(0, 20) + '...';
            this.postedAt = moment.utc(model.PostedAt).fromNow();
            this.channel = model.Channel;
            this.user = model.User;
            this.id = model.Id;
            this.title = model.Title;
        }
    }

    export class AppViewModel {

        constructor(dataModel: AppDataModel) {
            this.dataModel = dataModel; 
        }

        dataModel: AppDataModel;
        links = ko.observableArray<LinkViewModel>();

        loadingLinks = ko.observable(false);

        loadingMoreLinks = ko.observable(false);
        hasMoreItems = ko.observable(true);
        
        init() {
            this.loadingLinks(true);
            var self : AppViewModel = this;
            this.dataModel.getLinks()
                .done(function (data) {
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
                })
                .fail(function () {
                    // TODO: notify error
                });
            this.loadingLinks(false);
        }

        loadMoreItems() {
            if (this.loadingLinks()) return;
            this.loadingMoreLinks(true);
            var self = this;
            var lastLoadedId = this.links()[this.links().length - 1].id;
            
            this.dataModel.getMoreLinks(lastLoadedId)
                .done(function (data) {
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
                })
                .fail(function () {
                    // TODO: notify error
                });

            this.loadingMoreLinks(false);
        }

        linkAdded(link) {
            var vm = new LinkViewModel(this, link);
            this.links.unshift(vm);
        }
    }

}

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
