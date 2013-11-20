// Get signalr.d.ts.ts from https://github.com/borisyankov/DefinitelyTyped (or delete the reference)
/// <reference path="signalr/signalr.d.ts" />
/// <reference path="jquery/jquery.d.ts" />

////////////////////
// available hubs //
////////////////////
//#region available hubs

interface SignalR {

    /**
      * The hub implemented by LinkLogger.Hubs.LinkHub
      */
    linkHub : LinkHub;
}
//#endregion available hubs

///////////////////////
// Service Contracts //
///////////////////////
//#region service contracts

//#region LinkHub hub

interface LinkHub {
    
    /**
      * This property lets you send messages to the LinkHub hub.
      */
    server : LinkHubServer;

    /**
      * The functions on this property should be replaced if you want to receive messages from the LinkHub hub.
      */
    client : any;
}

interface LinkHubServer {

    /** 
      * Sends a "send" message to the LinkHub hub.
      * Contract Documentation: ---
      * @param message {string} 
      * @return {JQueryPromise of void}
      */
    send(message : string) : JQueryPromise<void>;
}

//#endregion LinkHub hub

//#endregion service contracts



////////////////////
// Data Contracts //
////////////////////
//#region data contracts

//#endregion data contracts

