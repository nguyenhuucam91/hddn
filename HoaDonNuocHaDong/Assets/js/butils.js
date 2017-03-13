function BUtils() {

}
/* PUBLIC STATIC METHODS */
// return <select> selected text
BUtils.slGetSelectedText = function ($select) {
    return $select.find(":selected").text();
}
// clear <select> options
BUtils.slRemoveAllOptions = function ($select) {
    $select.html("");
}
// check if n is numeric
BUtils.isNumeric = function (n) {
    return !isNaN(parseFloat(n)) && isFinite(n);
}