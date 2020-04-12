document.addEventListener('websocketCreate', function () {
    console.log("Websocket created!");
    showHideSettings(actionInfo.payload.settings);

    websocket.addEventListener('message', function (event) {
        console.log("Got message event!");

        // Received message from Stream Deck
        var jsonObj = JSON.parse(event.data);

        if (jsonObj.event === 'sendToPropertyInspector') {
            var payload = jsonObj.payload;
            //showHideSettings(payload);
            checkShowHide();
        }
        else if (jsonObj.event === 'didReceiveSettings') {
            var payload = jsonObj.payload;
            //showHideSettings(payload.settings);
            checkShowHide();
        }
    });
});

function showHideSettings(payload) {
    console.log("Show Hide Settings Called", payload);
}

function checkShowHide() {
    elem = document.getElementById("vJoyElementType");

    document.getElementById('resax').style.display = 'none';
    document.getElementById('resax_line').style.display = 'none';
    document.getElementById('setax').style.display = 'none';
    document.getElementById('setax_line').style.display = 'none';
    document.getElementById('vJoyButtonIdBlock').style.display = 'none';
    document.getElementById('setToCustomValueBlock').style.display = 'none';
    document.getElementById('setStepUpBlock').style.display = 'none';
    document.getElementById('setStepDownBlock').style.display = 'none';
    document.getElementById('resetToCustomValueBlock').style.display = 'none';
    document.getElementById('resetStepUpBlock').style.display = 'none';
    document.getElementById('resetStepDownBlock').style.display = 'none';
    document.getElementById('vJoyButtonIdBlock').style.display = 'none';


    if (elem.value == "btn") {
        document.getElementById('vJoyButtonIdBlock').style.display = 'flex';
    } else {

        // TODO: Double if queries depending on button action.. must be made much simpler

        // Only Push
        if (document.getElementById('triggerPush').checked) {
            document.getElementById('setax').style.display = 'flex';
            document.getElementById('setax_line').style.display = 'block';



            // Set to Step Up marked
            if (document.getElementById('setToStepUp').checked) {
                document.getElementById('setStepUpBlock').style.display = 'flex';
            }


            // Set to Step Down marked
            if (document.getElementById('setToStepDown').checked) {
                document.getElementById('setStepDownBlock').style.display = 'flex';
            }

            // Set to Custom marked
            if (document.getElementById('setToCustom').checked) {
                document.getElementById('setToCustomValueBlock').style.display = 'flex';
            }



        }


        // Only Release
        if (document.getElementById('triggerRelease').checked) {
            document.getElementById('resax').style.display = 'flex';
            document.getElementById('resax_line').style.display = 'block';


            // Reset to Step Up marked
            if (document.getElementById('resetToStepUp').checked) {
                document.getElementById('resetStepUpBlock').style.display = 'flex';
            }

            // Reset to Step Down marked
            if (document.getElementById('resetToStepDown').checked) {
                document.getElementById('resetStepDownBlock').style.display = 'flex';
            }

            // Reset to Custom marked
            if (document.getElementById('resetToCustom').checked) {
                document.getElementById('resetToCustomValueBlock').style.display = 'flex';
            }
        }


        // Push and release 
        if (document.getElementById('triggerPushAndRelease').checked) {
            document.getElementById('setax').style.display = 'flex';
            document.getElementById('setax_line').style.display = 'block';
            document.getElementById('resax').style.display = 'flex';
            document.getElementById('resax_line').style.display = 'block';




            // Set to Step Up marked
            if (document.getElementById('setToStepUp').checked) {
                document.getElementById('setStepUpBlock').style.display = 'flex';
            }


            // Set to Step Down marked
            if (document.getElementById('setToStepDown').checked) {
                document.getElementById('setStepDownBlock').style.display = 'flex';
            }

            // Set to Custom marked
            if (document.getElementById('setToCustom').checked) {
                document.getElementById('setToCustomValueBlock').style.display = 'flex';
            }

            // Reset to Step Up marked
            if (document.getElementById('resetToStepUp').checked) {
                document.getElementById('resetStepUpBlock').style.display = 'flex';
            }

            // Reset to Step Down marked
            if (document.getElementById('resetToStepDown').checked) {
                document.getElementById('resetStepDownBlock').style.display = 'flex';
            }

            // Reset to Custom marked
            if (document.getElementById('resetToCustom').checked) {
                document.getElementById('resetToCustomValueBlock').style.display = 'flex';
            }

        }
    }
}
checkShowHide();