const palette_barchart = ['#4779FA', '#3834EF'];
const palette_piechart =['#F0C079', '#7197F9', '#E77A81'];

var abp = abp || {};
var l = abp.localization.getResource("LookOn");
var ShowBooleanLabel = function (inputBoolean){
    return inputBoolean ? '<span class="badge bg-success">' + l("Yes") + '</span>' : '<span class="badge bg-danger">' + l("No") + '</span>';
}
$('#navbarSidebar a').click(function (){
    var href = $(this).attr('href');
    if (href != '#')
    {
        abp.ui.block({
            elm: '#pageWrap',
            busy: true
        });
    }

});
$(window).ready(function(){

    abp.ui.unblock();

});

