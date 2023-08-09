namespace Luthetus.Website.RazorLib.Store.ReplCase.Facts.BlazorWasmAppCase;

public partial class BlazorWasmAppFacts
{
    public static readonly string APP_JS_ABSOLUTE_FILE_PATH = @"/BlazorWasmApp/wwwroot/js/app.js";

    public static readonly string APP_JS_CONTENTS = @"window.luthetusCommon = {
    localStorageSetItem: function (key, value) {
        localStorage.setItem(key, value);
    },
    localStorageGetItem: function (key, value) {
        return localStorage.getItem(key);
    }
}

Blazor.registerCustomEventType('keydownwithpreventscroll', {
    browserEventName: 'keydown',
    createEventArgs: e => {

        let preventDefaultOnTheseKeys = [
            ""ContextMenu"",
            ""ArrowLeft"",
            ""ArrowDown"",
            ""ArrowUp"",
            ""ArrowRight"",
            ""Home"",
            ""End"",
            ""Space"",
            ""Enter"",
            ""PageUp"",
            ""PageDown""
        ];

        let preventDefaultOnTheseCodes = [
            ""Space"",
            ""Enter"",
        ];

        if (preventDefaultOnTheseKeys.indexOf(e.key) !== -1 ||
            preventDefaultOnTheseCodes.indexOf(e.code) !== -1) {
            e.preventDefault();
        }

        return {
            Type: e.type,
            MetaKey: e.metaKey,
            AltKey: e.altKey,
            ShiftKey: e.shiftKey,
            CtrlKey: e.ctrlKey,
            Repeat: e.repeat,
            Location: e.location,
            Code: e.code,
            Key: e.key
        };
    }
});";
}