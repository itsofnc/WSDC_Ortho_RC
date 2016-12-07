var DateEditStarted = false;

function DateFocus(Field) {
    DateEditStarted = true;
    DisplayField = Field.id.split("_", 2)[1];
    document.getElementById(DisplayField + "Validate").innerHTML = '<br /><input type="text" id="' + DisplayField + 'Show" readonly="readonly" style="border: 0px solid #000000;"/>';
    Field.value = Field.value.replace(/\//g, "");  // remove all slashes (have to escapet the slash in the mask \/
    DateChange(Field);
    if (DisplayField) {
        document.getElementById(DisplayField + "Show").style.visibility = "visible";
        document.getElementById(DisplayField + "Show").style.height = Field.style.height;
        document.getElementById(DisplayField + "Show").style.width = Field.style.width;
    }
    Field.select();
    Field.focus();
}
function DateBlur(Field) {
    DateEditStarted = false;
    DisplayField = Field.id.split("_", 2)[1];
    if (DisplayField) {
        Field.value = Field.value.replace(/\//g, "");
        if (Field.value.length == 6) { Field.value = Field.value.substr(0, 4) + "20" + Field.value.substr(4, 2); }

        DateChange(Field);   // in case the user pasted a Date number with "/"'s
        FieldValue = Field.value;
        if (FieldValue.replace(/ /g, "") == "") {
        } else if (FieldValue.length == 8) {

            // is this a valid date?
            dteDate = new Date(Date.parse(Field.value.substr(4, 4) + "/" + Field.value.substr(0, 2) + "/" + Field.value.substr(2, 2), "yyyy/dd/MM"));

            if (dteDate == "Invalid Date") {
                alert("Invalid Date Entered\n\nUse format: mmddyyyy")
                Field.value = ""
                Field.focus();
            } else {
                ValidDate = Field.value.substr(0, 2) * 1 == dteDate.getMonth() * 1 + 1;
                ValidDate = ValidDate && Field.value.substr(2, 2) * 1 == dteDate.getDate() * 1;
                ValidDate = ValidDate && Field.value.substr(4, 4) * 1 == dteDate.getFullYear() * 1;
                if (!ValidDate) {
                    alert("Invalid Date Entered\n\nUse format: mmddyyyy")
                    Field.value = ""
                    Field.focus();
                } else {
                    // date okay
                    try {
                    Field.value = document.getElementById(DisplayField + "Show").value
                    document.getElementById(DisplayField + "Show").style.visibility = "hidden";
                        //                                                                document.getElementById(DisplayField + "Show").style.height = "0px";
                        //                                                                document.getElementById(DisplayField + "Show").style.width = "0px";
                        //                                                                document.getElementById(DisplayField + "Validate").innerHTML = "";
                    }
                    catch(err) {
                        Field.value = Field.value.substr(0,2) + '/' + Field.value.substr(2,2) + '/' + Field.value.substr(4,4)
                    }
                        Field.value = Field.value.replace(/ /g, "");
                        Field.value = Field.value.replace(/\(/g, "");
                        Field.value = Field.value.replace(/\)/g, "");
                        TestValue = Field.value;
                    

                        if (TestValue.match(/^\d{1,2}(\-|\/|\.)\d{1,2}\1\d{4}$/)) {
                        } else {
                            alert("Invalid Format\n\nUse format: mmddyyyy")
                            Field.value = ""
                            Field.focus();
                        }
                   
                }
            }

        } else {
            alert("Please key 8 numeric digits\n\nUse format: mmddyyyy")
            Field.value = ""
            Field.focus();
        }
    }

}
function DateKey(Field, evt) {
    if (DateEditStarted == true) {
        if (Field.value.length >= 8) { Field.value = ""; }
        var theEvent = evt || window.event;
        if (Field.value.length < 8) {
            var key = theEvent.keyCode || theEvent.which;
            key = String.fromCharCode(key);
            var regex = /[0-9]|\./;
            if (!regex.test(key)) {
                theEvent.returnValue = false;
                if (theEvent.preventDefault) theEvent.preventDefault();
            }
        } else {
            theEvent.returnValue = false;
            if (theEvent.preventDefault) theEvent.preventDefault();
        }
    }
}
function DateChange(Field) {
    if (DateEditStarted == true) {
        DisplayField = Field.id.split("_", 2)[1];
        if (DisplayField) {
            FieldValue = (Field.value.replace(/\//g, "") + "        ").substr(0, 8);
            FieldValue = FieldValue.replace(/ /g, "#");
            document.getElementById(DisplayField + "Show").value = "(" + FieldValue.substr(0, 2) + "/" + FieldValue.substr(2, 2) + "/" + FieldValue.substr(4, 4) + ")";

        }
    }
}
