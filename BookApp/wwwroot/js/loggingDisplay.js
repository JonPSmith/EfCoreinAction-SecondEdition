/**********************************************************************
 * LoggingDisplay handles the acquiring and display of the logs
 * 
 * First created: 2016/09/22
 * 
 * Under the MIT License (MIT)
 *
 * Written by Jon Smith : GitHub JonPSmith, www.thereformedprogrammer.net
 **********************************************************************/

var LoggingDisplay = (function($) {
    'use strict';

    var logApiUrl;
    var logs = null;
    var traceIdentLocal;

    var $showLogsLink = $('#show-logs');
    var $showBgLogsLink = $('#show-bg-logs');
    var $logModal = $('#log-modal');
    var $logModalBody = $logModal.find('.modal-body');
    var $logDisplaySelect = $logModal.find('#displaySelect');

    function getDisplayType() {
        return $logDisplaySelect.find('input[name=displaySelect]:checked').val();
    }

    function updateLogsCountDisplay() {
        var allCount = logs.requestLogs.length;
        $('.modal-title span').text(allCount);
        $('#all-select span').text(allCount);
        var sqlCount = 0;
        for (var i = 0; i < logs.requestLogs.length; i++) {
            if (logs.requestLogs[i].isDb) {
                sqlCount++;
            }
        }
        $('#sql-select span').text(sqlCount);
    }

    function showModal() {
        updateLogsCountDisplay();
        $logModal.modal();
    }

    function setContextualColors(eventType) {
        switch (eventType) {
            case 'Information':
                return 'text-info';
            case 'Warning':
                return 'text-warning';
            case 'Error':
                return 'text-danger';
            case 'Critical':
                return 'text-danger bold';
            default:
                return '';
        }
    }

    function fillModalBody(displayType) {
        displayType = displayType || getDisplayType();
        var body = '<div id="log-accordion">';
        for (var i = 0; i < logs.requestLogs.length; i++) {
            if (displayType !== 'sql' || logs.requestLogs[i].isDb)
                body +=
'<div class="card">'+
    '<div class="card-header text-overflow-dots">'+
        '<a class="card-link" data-toggle="collapse" href="#collapse'+i+'">'+
              '<span class="' + setContextualColors(logs.requestLogs[i].logLevel) + '">' + logs.requestLogs[i].logLevel + ':&nbsp;</span>' +
                   logs.requestLogs[i].eventString + 
                '</a>' +
    '</div>'+
    '<div id="collapse' + i +'" class="collapse" data-parent="#log-accordion">'+
        '<div class="card-body white-space-pre">'+
            logs.requestLogs[i].eventString+
        '</div>'+
    '</div>'+
'</div>';
        }
        body += '</div>';
        $logModalBody.html(body);
    }

    function getLogs(traceIdentifier) {
        $.ajax({
            url: logApiUrl,
            data: { traceIdentifier: traceIdentifier }
        })
        .done(function (data) {
            logs = data;
            fillModalBody();
            showModal();
        })
        .fail(function () {
            alert("error");
        });
    }

    function startModal() {
        getLogs(traceIdentLocal);
    }

    function setupTrace(traceIdentifier, numLogs) {
        //now set up the log menu link
        traceIdentLocal = traceIdentifier;
        $showLogsLink.find('.badge').text(numLogs + '+');
    }

    //public parts
    return {
        initialise: function(exLogApiUrl, traceIdentifier, numLogs) {
            logApiUrl = exLogApiUrl;

            setupTrace(traceIdentifier, numLogs);

            //setup the main events
            $showLogsLink.unbind('click')
                .bind('click',
                    function() {
                        startModal();
                    });
            $showLogsLink.removeClass('d-none');

            $showBgLogsLink.unbind('click')
                .bind('click',
                    function () {
                        getLogs('non-http-logs');
                    });

            $('#all-select').on('click',
                function() {
                    fillModalBody('all');
                });
            $('#sql-select').on('click',
                function () {
                    fillModalBody('sql');
                });
        },

        newTrace: function(traceIdentifier, numLogs) {
            setupTrace(traceIdentifier, numLogs);
        }
    };
}(window.jQuery));