mergeInto(LibraryManager.library, {

  FullScreenToggle: function () {
    if(document.fullscreenElement!==null){
      this.unityInstance.SetFullscreen(0);
    }
    else{
      this.unityInstance.SetFullscreen(1);
    }
  },

});