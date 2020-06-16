window.blazorCulture = {
    get: () => window.localStorage['BlazorCulture'],
    set: (value) => window.localStorage['BlazorCulture'] = value
};

function getFileUrl(fileContent) {
    var data = new Blob([fileContent], { type: 'text/plain' });
    return window.URL.createObjectURL(data);
}

// URL pointing to the Blob with the file contents
var objUrl = null;
// create the blob with file content, and attach the URL to the downloadlink;
// NB: link must have the download attribute
// this method can go to your library
function exportFile(fileContent, fileName, downloadLinkId) {
    // revoke the old object URL to avoid memory leaks.
    if (objUrl !== null) {
        window.URL.revokeObjectURL(objUrl);
    }
    // create the object that contains the file data and that can be referred to with a URL
    var data = new Blob([fileContent], { type: 'text/plain' });
    objUrl = window.URL.createObjectURL(data);
    // attach the object to the download link (styled as button)
    var downloadLinkButton = document.getElementById(downloadLinkId);
    downloadLinkButton.download = fileName;
    downloadLinkButton.href = objUrl;
};

Blazor.start({
    loadBootResource: function (type, name, defaultUri, integrity) {
        // For framework resources, use the precompressed .br files for faster downloads
        // This is needed only because GitHub pages doesn't natively support Brotli (or even gzip for .dll files)
        if (type !== 'dotnetjs' && location.hostname !== 'localhost') {
            return (async function () {
                const response = await fetch(defaultUri + '.br', { cache: 'no-cache' });
                if (!response.ok) {
                    throw new Error(response.statusText);
                }
                const originalResponseBuffer = await response.arrayBuffer();
                const originalResponseArray = new Int8Array(originalResponseBuffer);
                const decompressedResponseArray = BrotliDecode(originalResponseArray);
                const contentType = type === 'dotnetwasm' ? 'application/wasm' : 'application/octet-stream';
                return new Response(decompressedResponseArray, { headers: { 'content-type': contentType } });
            })();
        }
    }
});

