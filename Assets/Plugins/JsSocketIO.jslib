mergeInto(LibraryManager.library, {
    JsSocketIo: function(){
      this.SocketIo = {};
      this.SocketIo._events = [];
      this.gameObjectName = null;
      this.callBackName = null;
    },
    JsSocketIo_SetCallback : function(_gameObjectName, _callBackName) {
      gameObjectName = UTF8ToString(_gameObjectName);
      callBackName = UTF8ToString(_callBackName);
      this.SocketIo.gameObjectName = gameObjectName;
      this.SocketIo.callBackName = callBackName;
    },
    JsSocketIo_Connect : function() {
      this.SocketIo.socket = io();
      this.SocketIo._events.forEach(function(_event){
        this.SocketIo.socket.on(_event, function(msg) {
            var objParameter = {
              "_event" : _event,
              "_msg" : JSON.stringify([msg]),
            };
            unityInstance.SendMessage(SocketIo.gameObjectName, SocketIo.callBackName, JSON.stringify(objParameter));
           });
      });
    },
    JsSocketIo_On: function(_event){
      event = UTF8ToString(_event);
      if (this.SocketIo._events.indexOf(event) == -1) {
        this.SocketIo._events.push(event);
      }
    },
    JsSocketIo_Emit: function(_event, _json){
      var json = UTF8ToString(_json);
      var event = UTF8ToString(_event);
      var obj = JSON.parse(json);
      console.log(obj);
      window.SocketIo.socket.emit(event, obj);
    },
    EvalJScript: function(f) {
      eval(f);
    }
});
