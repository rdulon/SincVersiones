async function syncProvider() {
  const providerId = $(this).data("provider-id");
  const providerName = $(this).data("provider-name");
  try {
    const url = new URL(`provider/sync/${providerId}`, window.location.origin);
    await $.post(url.toString(), {});
    displaySuccessAlert(
      `El proveedor <strong>${providerName}</strong> ha iniciado la sincronización.`
    );
  } catch (e) {
    displayErrorAlert(
      `Ha ocurrido un error al intentar sincronizar el proveedor <strong>${providerName}</strong>.`
    );
  }
}

function addProviderHour() {
  deleteHoursError();

  let n = 0;
  $('input[id^="Hours_"]').each(function(_, el) {
    const ni = $(el)
      .attr("id")
      .match(/\d+/)[0];
    n = Math.max(n, +ni);
  });
  $("#provider-hours").append(
    $("<div>")
      .addClass("col-sm-6 col-md-4 hour-container d-flex align-items-center")
      .append(
        $("<input>")
          .addClass("form-control")
          .attr("id", `Hours_${n + 1}`)
          .attr("name", "Hours")
      )
      .append(
        $("<i>")
          .addClass("fa fa-trash text-danger delete")
          .attr("aria-label", "hidden")
          .click(removeProviderHour)
      )
  );
}

function deleteHoursError() {
  $("#provider-hours > span.text-danger").remove();
}

function removeProviderHour() {
  deleteHoursError();
  $(this)
    .parent()
    .remove();
}

function changeProviderType() {
  $(".sync-type").each(function(_, el) {
    $(el).toggleClass("hide-sync-type");
  });
}

async function pingProvider() {
  let url = $('input[name="Url"]').val();
  if (url != "") {
    try {
      url = new URL("/api/ping", url).toString();
      try {
        const resp = await $.get(url);
        if (resp === "Pong") {
          $(this)
            .removeClass("btn-outline-secondary")
            .addClass("btn-success");
        }
      } catch (e) {
        $(this)
          .removeClass("btn-outline-secondary")
          .addClass("btn-danger");
      }
    } catch (e) {}
  }
}

$(function() {
  $(".sync-provider").click(syncProvider);
  $("#add-provider-hour").click(addProviderHour);
  $(".hour-container i").click(removeProviderHour);
  $('.sync-type-check input[type="radio"]').change(changeProviderType);
  $("#ping-provider").click(pingProvider);
});
