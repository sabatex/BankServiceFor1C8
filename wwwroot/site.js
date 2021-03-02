window.Sabatex = {
    culture: {
        get: () => window.localStorage['BlazorCulture'],
        set: (value) => window.localStorage['BlazorCulture'] = value
    },
    getLocalValue: function (valueName) {
        if (valueName === undefined) return null; 
        return window.localStorage.getItem("currentCulture");
    },
    setLocalValue: function (valueName, value) {
        window.localStorage[valueName] = value;
    },
    getFileUrl:function (fileContent) {
        var data = new Blob([fileContent], { type: 'text/plain' });
        return window.URL.createObjectURL(data);
    },
    downloadFileResult:function (fileUrl,fileName) {
        let link = document.createElement("a");
        link.download = fileName;
        link.href = fileUrl;
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    },
    downloadFile:function (fileName, fileContent) {
        let link = document.createElement("a");
        link.download = fileName;
        var data = new Blob([fileContent], { type: 'text/plain' });
        link.href = window.URL.createObjectURL(data);
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    }
    
};

// Single Page Apps for GitHub Pages
// https://github.com/rafrex/spa-github-pages
// Copyright (c) 2016 Rafael Pedicini, licensed under the MIT License
// ----------------------------------------------------------------------
// This script checks to see if a redirect is present in the query string
// and converts it back into the correct url and adds it to the
// browser's history using window.history.replaceState(...),
// which won't cause the browser to attempt to load the new url.
// When the single page app is loaded further down in this file,
// the correct url will be waiting in the browser's history for
// the single page app to route accordingly.
(function (l) {
    if (l.search) {
        var q = {};
        l.search.slice(1).split('&').forEach(function (v) {
            var a = v.split('=');
            q[a[0]] = a.slice(1).join('=').replace(/~and~/g, '&');
        });
        if (q.p !== undefined) {
            window.history.replaceState(null, null,
                l.pathname.slice(0, -1) + (q.p || '') +
                (q.q ? ('?' + q.q) : '') +
                l.hash
            );
        }
    }
}(window.location))