document.addEventListener('DOMContentLoaded', function () {
    var fileInput = document.getElementById('file-upload');
    var dropArea = document.getElementById('drop-area');
    var fileList = document.getElementById('file-list');

    dropArea.onclick = function () {
        fileInput.click();
    };

    fileInput.onchange = function () {
        handleFiles(this.files);
    };

    dropArea.ondragover = dropArea.ondragenter = function (evt) {
        evt.preventDefault();
    };

    dropArea.ondrop = function (evt) {
        evt.preventDefault();
        handleFiles(evt.dataTransfer.files);
    };

    function handleFiles(files) {
        for (let i = 0; i < files.length; i++) {
            const file = files[i];

            const fileElement = document.createElement('div');
            fileElement.className = 'file-entry';
            fileElement.innerHTML = `
                <span class="file-name">${file.name}</span>
                <span class="file-size">${(file.size / 1024 / 1024).toFixed(2)} MB</span>
                <span class="delete-file" onclick="removeFileEntry(event)">&times;</span>
            `;
            fileList.appendChild(fileElement);
        }
    }

    window.removeFileEntry = function (event) {
        event.target.closest('.file-entry').remove();
    };
});
