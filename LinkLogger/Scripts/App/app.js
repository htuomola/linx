function AppDataModel() {
    var self = this;
    
    self.getLinks = function() {
        return $.ajax("/api/links");
    };
}

function LinkViewModel(app, model) {
    var self = this;

    self.url = model.Url;
    self.postedAt = model.PostedAt;
    self.channel = model.Channel;
    self.user = model.User;
    self.id = model.Id;
}

function AppViewModel(dataModel) {
    var self = this;
    
    self.links = ko.observableArray();

    self.init = function() {
        dataModel.getLinks()
            .done(function(data) {
                if (typeof (data) === "object") {
                    for (var i = 0; i < data.length; i++) {
                        self.links.push(new LinkViewModel(app, data[i]));
                    }
                } else {
                    //self.errors.push("An unknown error occurred.");
                    // TODO: notify error
                }
            })
            .fail(function() {
                // TODO: notify error
            });
    };

    self.linkAdded = function(link) {
        var vm = new LinkViewModel(app, link);
        self.links.unshift(vm);
    };
}

var app = new AppViewModel(new AppDataModel());

$(function () {
    // Reference the auto-generated proxy for the hub.
    var linkHub = $.connection.linkHub;
    // Create a function that the hub can call back for notifying of new links.
    linkHub.client.addNewLink = function(link) {
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
