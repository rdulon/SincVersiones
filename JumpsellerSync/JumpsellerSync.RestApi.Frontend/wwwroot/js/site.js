const alerts = $('#ui-alerts');

function displaySuccessAlert(html) {
   _createAlert('success', html);
}

function displayWarningAlert(html) {
   _createAlert('warning', html);
}

function displayErrorAlert(html) {
   _createAlert('danger', html);
}

function _createAlert(className, html) {
   const firstChild = alerts.children() && alerts.children()[0];

   const alert = $('<div>')
      .addClass(`alert alert-${className} alert-dismissible fade show`)
      .attr('role', 'alert')
      .append(html)
      .append(
         $('<button>')
            .attr('type', 'button')
            .attr('arial-label', 'Close')
            .attr('data-dismiss', 'alert')
            .addClass('close')
            .append($('<span>').attr('aria-hidden', true).html('&times;')));

   if (firstChild == null) {
      alerts.append(alert);
   } else {
      alert.insertBefore($(firstChild));
   }
}