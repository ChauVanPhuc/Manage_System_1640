// Auto size for textarea
function resizeTextarea() {
    $('textarea').each(function () {
        this.setAttribute('style',  'px;overflow-y:hidden;');
    }).on('input', function () {
        if (this.value != '') {
            this.style.height = 'auto';
            
        }
        else {
            this.style.height = '20px';
        }
    });
}
$(document).ready(function () {
    resizeTextarea();
});