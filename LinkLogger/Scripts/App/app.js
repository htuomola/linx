function AppDataModel() {
    var self = this;
    var mockData = ["alpha", "beta"];

    //self.links = ko.observableArray(mockData);
    self.getLinks = function() {
        return $.ajax("/api/links");
        //return mockData;
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
    //var mockData = ["alpha", "beta"];

    self.links = ko.observableArray();

    self.init = function() {
        dataModel.getLinks()
            .done(function(data) {
                if (typeof (data) === "object") {
                    for (var i = 0; i < data.length; i++) {
                        self.links.push(new LinkViewModel(app, data[i]));
                        //self.externalLoginProviders.push(new ExternalLoginProviderViewModel(app, data[i]));
                    }
                } else {
                    //self.errors.push("An unknown error occurred.");
                }
            })
            .fail(function() {

            });
    };
}

var app = new AppViewModel(new AppDataModel());