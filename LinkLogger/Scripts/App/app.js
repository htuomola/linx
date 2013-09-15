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
    var mockData = ["alpha", "beta"];

    self.links = ko.observableArray(mockData);
}

var app = new AppViewModel(new AppDataModel());