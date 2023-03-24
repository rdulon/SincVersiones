const manageSpinner = $("#manage-spinner");

async function loadJumpsellerProducts() {
  manageSpinner.fadeIn();
  try {
    const url = new URL(
      "/manage/loadJumpsellerProducts",
      window.location.origin
    );
    const html = await $.post(url.toString(), {});
    $("#manage-content").html(html);
  } catch (e) {
    console.error(e);
  } finally {
    manageSpinner.fadeOut();
  }
}

$(function() {
  $("#load-jumpseller-products").click(loadJumpsellerProducts);
  $("#purge-jumpseller-products").click(function() {
    alert("Not implemented yet.");
  });
});
