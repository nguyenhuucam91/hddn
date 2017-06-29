$.validator.methods.date = function (value, element) {
    return this.optional(element) || Globalize.parseDate(value, "MM/dd/yyyy") !== null;
}