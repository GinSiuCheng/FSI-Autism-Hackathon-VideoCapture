Testfile = "https://scenario2storage.blob.core.windows.net/asset-7a3e5529-b2ec-4bc2-816e-bfe7e7942cad/Movie on 4-23-18 at 12.36 PM_540x360_AACAudio_1000.mp4?sv=2015-07-08&sr=c&si=5fc7a7c0-209e-4408-bf21-f3401e7b0f0b&sig=7OAt%2F5DX6iTYGJ1hLSoWgkutBY%2FiQu0z16sd3Ih75YA%3D&st=2018-04-23T18%3A28%3A52Z&se=2118-04-23T18%3A28%3A52Z"


function takemethere(timeIndex) {
    playerInstance.currentTime(timeIndex);
}


function PopulateIndex() {
    // var objl = document.getElementById('listIndex');
    markerObj.sort(function (obj1, obj2) {
        return obj1.time - obj2.time;
    });
    if (markerObj.length !== 0) {
        var temp = JSON.stringify(markerObj);
        console.log(temp);
        // for (var i = 0; i < markerObj.length; i++) {
        //     var jobj = markerObj[i];
        //     // var strl = '<li class="list-group-item"><a href="#" onclick="takemethere(' + jobj['time'] + ')">' + jobj['text'] + '</a></li>';
        //     addIndex(jobj);
        // }
        addIndex(temp);
    }

}

function addIndex(jobj) {
    let val = JSON.parse(jobj);
    console.log(val);
    let template = [];
    val.forEach((d,i)=>{
        template.push(
            '<li class="list-group-item"><a href="#" id=' + d['time'] +
        ' onclick="takemethere(' + d['time'] + ')">' +
        convertSecsToTimeFormat(d['time']) + " - " + d['text'] + '</a></li>'
        );
    })
    var strl = '<li class="list-group-item"><a href="#" id=' + val[0]['time'] +
        ' onclick="takemethere(' + val[0]['time'] + ')">' +
        convertSecsToTimeFormat(val[0]['time']) + " - " + val[0]['text'] + '</a></li>';
    $('#listIndex').append(template);
}

function populatePatients(patients) {
    for (var i = 0; i < patients.length; i++) {
        var jobj = patients[i];
        var strl = '<a class="dropdown-item" href="#" onclick="patientSelected(\'' + jobj['id'] + '\',\'' + jobj['name'] + '\')">' + jobj['name'] + '</a>';
        $('#dropdownOptions').append(strl);
    }
}

function patientSelected(pid, pname) {
    patient = {
        id: pid,
        name: pname
    };
    $('#dropdownMenuButton').text(pname);
    $('#patientName').val(pname);
}

function postData() {
    // get patient id from global
    // var secs = parseInt($('#TimeStamp').val());
    var secs = convertTimeFormatToSecs($('#TimeStamp').val());
    var startTimeUTC = moment.utc(video.StartTime);
    var markerTime = startTimeUTC.add(secs, 's');
    console.log(markerTime.format("YYYYMMDDHHmmss"));
    // var TimeStamp = $('#TimeStamp').val();

    var pdata = {
        PatientID: patient.id,
        TimeStamp: markerTime.format("YYYYMMDDHHmmss"),
        Skill: $('#skill').val(),
        Target: $('#target').val(),
        Result: $('#result').val(),
        Comments: $('#comments').val()
    };
    $.ajax({
        type: "POST",
        url: "https://fsiautismny2.azurewebsites.net/api/Data",
        data: JSON.stringify(pdata),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (p) {
            console.log(p);
            addMarkpoint(secs);
            markerObj.push({
                'time': secs,
                'text': pdata.Skill + " " + pdata.Comments
            });
            // markerAry.push(secs);
            // markerAry.sort();
            cleanUPIndex();
            PopulateIndex();
        },
        failure: function (errMsg) {
            alert(errMsg);
        }
    });
}

function initializeVideo(videoData) {
    video = videoData;
    markerAry = [];
    markerObj = [];
    for (var event in video.Events) {
        var eobg = video.Events[event];
        markerAry.push(eobg.OffsetSeconds);
        markerObj.push({
            'time': eobg.OffsetSeconds,
            'text': eobg.Skill + " " + eobg.Comments
        });
    }
    // markerAry.sort();
    loadAMP(markerAry, video.URL);
    PopulateIndex();

    $('#videoPlayback').show();
    $('#postDataForm').show();
    $('#postheader').show();
}

function cleanUPIndex() {
    $('#listIndex').empty();
}

function GetVideos() {
    cleanUPIndex();
    if (typeof patient == 'undefined') {
        alert("Select patient");
        return;
    }
    var dates = $('#datr').val().split("-");
    var startDate = moment(new Date(dates[0].trim())).format("YYYYMMDD") + "000000";
    var endDate = moment(new Date(dates[1].trim())).format("YYYYMMDD") + "000000";
    // http://fsiautismny2.azurewebsites.net/api/VideosWithData?patientId=abc123&startTime=20180423000000&endTime=20180424000000
    $.ajax({
        type: "GET",
        url: "https://fsiautismny2.azurewebsites.net/api/VideosWithData",
        data: {
            "patientId": patient.id,
            "startTime": startDate,
            "endTime": endDate
        },
        // contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
            console.log(result);
            initializeVideo(result);
        },
        failure: function (errMsg) {
            alert(errMsg);
        }
    });
}

function getMarkpoint() {
    playerInstance.pause();
    var pos = playerInstance.currentTime();
    $('#TimeStamp').val(convertSecsToTimeFormat(Math.round(pos)));
}

function addMarkpoint(secs) {

    addMarker(null, secs);
}

// function _ampTimeUpdateHandler(strTime) {
//     //$('#footime').text(strTime);
//     console.log(playerInstance.currentTime());
//
// }

function loadAMP(aryTimes, videoSrc) {

    var myOptions = {
        autoplay: true,
        controls: true,
        width: "640",
        height: "400",
        poster: "",
        plugins: {
            //foobarbaz: {},
            timelineMarker: {
                markertime: aryTimes //["20","25", "30"]

            }
        }
    };
    playerInstance = amp("myplayer", myOptions);
    playerInstance.src([{
        src: videoSrc
    }]);
    // playerInstance.addEventListener(amp.eventName.timeupdate, _ampTimeUpdateHandler);

}

$(document).ready(function functionName() {
    $('#videoPlayback').hide();
    $('#postDataForm').hide();
    $('#postheader').hide();
    // $('#selectPatient').hide();
    $('input[name="daterange"]').daterangepicker();

    populatePatients([{
        id: "Ptn001",
        name: "Ptn001"
    }, {
        id: "Ptn002",
        name: "Ptn002"
    }, {
        id: "Ptn003",
        name: "Ptn003"
    }, {
        id: "4",
        name: "Lambert"
    }]);


    // get the marker data
    // var markerObj = [{
    //     time: "20",
    //     text: "Behaviour 1"
    // }, {
    //     time: "25",
    //     text: "Behaviour 2"
    // }, {
    //     time: "30",
    //     text: "Behaviour 3"
    // }];
    // PopulateIndex(markerObj);
    // let ary = new Array();
    // for (marker of markerObj) {
    //     ary.push(marker.time);
    // }
    // loadAMP(ary);
});
