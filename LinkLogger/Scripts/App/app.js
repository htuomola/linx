function AppDataModel() {
    var self = this;
    var mockData = ["alpha", "beta"];

    //self.links = ko.observableArray(mockData);
    self.getLinks = function() {
        return mockData;
    };
}

function AppViewModel(dataModel) {
    var self = this;
    //var mockData = ["alpha", "beta"];

    self.links = ko.observableArray();

    self.init = function() {
        self.links(dataModel.getLinks());
    };
}

var app = new AppViewModel(new AppDataModel());