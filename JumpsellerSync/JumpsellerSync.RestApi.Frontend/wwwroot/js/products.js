async function loadUnsyncedProducts(page) {
  if (page == null) {
    return;
  }

  const providerId = providersSelect.val();
  const brandId = brandsSelect.val();

  const url = new URL("product/unsyncedProducts", window.location.origin);
  url.searchParams.append("providerId", providerId);
  url.searchParams.append("brandId", brandId);
  url.searchParams.append("page", page);
  url.searchParams.append("limit", _searchSession.itemsPerPage);
  url.searchParams.append(
    "skuOrName",
    $('#unsynced-search input[name="sku-or-name"]').val()
  );
  url.searchParams.append("withStock", _searchSession.withStock);

  try {
    unsyncedSpinner.fadeIn();
    const productsHtml = await $.get(url.toString());
    $("#unsynced-products")
      .hide()
      .html(productsHtml);
    const total = +$("#unsynced-total-pages").data("total-pages") || 1;
    $("#unsynced-products-pagination").bootpag({
      total: total,
      page: page,
      maxVisible: 5
    });
    $("#unsynced-products").fadeIn();
  } catch (e) {
    displayErrorAlert(
      "Ha ocurrido un error al intentar obtener productos no sincronizados."
    );
  } finally {
    unsyncedSpinner.fadeOut();
  }
}

async function loadSyncedProducts(page) {
  if (page == null) {
    return;
  }

  const url = new URL("product/syncedProducts", window.location.origin);
  url.searchParams.append("page", page.toString());
  url.searchParams.append("limit", _searchSession.itemsPerPage);
  url.searchParams.append(
    "skuOrName",
    $('#synced-search input[name="sku-or-name"]').val()
  );
  url.searchParams.append(
    "brandIds",
    $('#synced-search input[name="brands"]').val()
  );
  url.searchParams.append(
    "provider",
      $('#synced-search select[name="provider"]').val()
  );

  try {
    syncedSpinner.fadeIn();
    const prodHtml = await $.get(url.toString());

    $("#synced-products")
      .hide()
      .html(prodHtml);
    const total = +$("#synced-total-pages").data("total-pages") || 1;
    $("#synced-products-pagination").bootpag({
      total: total,
      page: page,
      maxVisible: 5
    });
    $("#synced-products").fadeIn();
  } catch (e) {
    displayErrorAlert("Ocurrió un error al cargar los productos.");
  } finally {
    syncedSpinner.fadeOut();
  }
}

async function loadBrands() {
  const providerId = providersSelect.val();
  const url = new URL("provider/brands", window.location.origin);
  url.searchParams.append("providerId", providerId);

  try {
    unsyncedSpinner.fadeIn();
    const brandsOptions = await $.get(url.toString());
    if (brandsOptions !== "") {
      brandsSelect.html(brandsOptions);
    }
  } catch (e) {
  } finally {
    unsyncedSpinner.fadeOut();
  }
}

async function syncProduct() {
  const modal = $("#syncProductModal");
  const modalBody = modal.find(".modal-body");
  const providerProductId = modalBody
    .find('input[name="providerProductId"]')
    .val();
  const name = modalBody.find('input[name="productName"]').val();
  const margin = parseInt(modalBody.find('input[name="margin"]').val());
  const providerId = providersSelect.val().toString();
  const sku = modalBody.find('input[name="productSku"]').val();

  const data = JSON.stringify({
    providerProductId,
    providerId,
    name,
    margin,
    sku
  });

  const url = new URL("product/syncProduct", window.location.origin);
  try {
    unsyncedSpinner.fadeIn();
    modal.modal("hide");
    await $.post({
      url: url.toString(),
      data,
      contentType: "application/json; charset=utf-8"
    });
    displaySuccessAlert(
      `El producto <strong>${name}</strong> ha sido sincronizado correctamente.`
    );
    loadUnsyncedProducts(getSyncActivePage());
  } catch (e) {
    displayErrorAlert(
      `El producto <strong>${name}</strong> no ha sido sincronizado.`
    );
  } finally {
    unsyncedSpinner.fadeOut();
  }
}

function getSyncActivePage() {
  return parseInt($("#synced-products-pagination li.active").attr("data-lp"));
}

function getUnsyncActivePage() {
  return parseInt($("#unsynced-products-pagination li.active").attr("data-lp"));
}

function initItemsPerPage() {
  _activateIPPPage();
  $("#items-per-page li").click(function() {
    if ($(this).data("ipp") === _searchSession.itemsPerPage) {
      return;
    }
    _searchSession.itemsPerPage = $(this).data("ipp");
    _activateIPPPage();
    loadSyncedProducts(1);
    loadUnsyncedProducts(1);
  });

  function _activateIPPPage() {
    $("#items-per-page li").removeClass("active");
    $(`#items-per-page li[data-ipp="${_searchSession.itemsPerPage}"]`).addClass(
      "active"
    );
  }
}

function initPagination() {
  const _bootpag = $.fn.bootpag;
  $.fn.bootpag = function(config) {
    const result = _bootpag.bind($(this))(config);
    $("ul.pagination.bootpag li>a").addClass("page-link");
    return result;
  };

  $("#synced-products-pagination")
    .bootpag({
      total: 0,
      page: _searchSession.sPage,
      maxVisible: 5,
      leaps: true
    })
    .on("page", async function(ev, pageNum) {
      await loadSyncedProducts(pageNum);
    });

  $("#unsynced-products-pagination")
    .bootpag({
      total: 0,
      page: _searchSession.uPage,
      maxVisible: 5,
      leaps: true
    })
    .on("page", async function(ev, pageNum) {
      await loadUnsyncedProducts(pageNum);
    });
}

function initUnsyncTabSelects() {
  providersSelect.change(async function() {
    await loadBrands();
    await loadUnsyncedProducts(_searchSession.uPage);
    $("#unsynced-products-pagination").bootpag({ page: _searchSession.uPage });
  });

  brandsSelect.change(async function() {
    await loadUnsyncedProducts(1);
    $("#unsynced-products-pagination").bootpag({ page: 1 });
  });
}

function initSyncProductModal() {
  $("#sync-product").click(syncProduct);

  $("#syncProductModal").on("show.bs.modal", function(event) {
    const button = $(event.relatedTarget);
    const providerProductId = button.data("provider-product-id");
    const productDesc = button.data("provider-product-desc");
    const brand = button.data("provider-product-brand").toLowerCase();
    const sku = button.data("provider-product-sku");
    const name = `${brand[0].toUpperCase()}${brand.substring(1)} ${sku}`;

    var modal = $(this);
    modal
      .find('.modal-body input[name="providerProductId"]')
      .val(providerProductId);
    modal.find('.modal-body input[name="productDesc"]').val(productDesc);
    modal.find('.modal-body input[name="productSku"]').val(sku);
    modal.find('.modal-body input[name="productName"]').val(name);
  });
}

async function loadSearchSession() {
  let brands = null;
  try {
    const url = new URL("product/prefetchBrands", window.location.origin);
    brands = await $.get(url.toString());
  } catch (e) {
    displayErrorAlert("No se han podido cargar las marcas de los productos.");
  }

  let searchSession = JSON.parse(sessionStorage.getItem("searchSession"));
  if (searchSession == null) {
    searchSession = {
      itemsPerPage: 10,
      sPage: 1,
      sSkuOrName: null,
      sBrands: null,
      uPage: 1,
      uSkuOrName: null,
      activeTab: "synced-tab",
      providerId: null,
      providerBrandId: null,
      withStock: true
    };
  }
  Object.assign(searchSession, {
    prefetchedBrands: brands || searchSession.brands || []
  });
  window._searchSession = searchSession;
}

function saveSearchSession() {
  Object.assign(_searchSession, {
    sPage: getSyncActivePage(),
    sSkuOrName: $('#synced-search input[name="sku-or-name"]').val(),
    sBrands: $('#synced-search input[name="brands"]').val(),
    uPage: getUnsyncActivePage(),
    uSkuOrName: $('#unsynced-search input[name="sku-or-name"]').val(),
    activeTab: $("#product-tabs > li > a.active").attr("id"),
    providerId: $("#unsynced-providers").val(),
    providerBrandId: $("#unsynced-brands").val()
  });
  sessionStorage.setItem("searchSession", JSON.stringify(_searchSession));
}

function initFilterInputs() {
  const brands = new Bloodhound({
    datumTokenizer: Bloodhound.tokenizers.obj.whitespace("name"),
    queryTokenizer: Bloodhound.tokenizers.whitespace,
    local: _searchSession.prefetchedBrands
  });
  brands.initialize();

  $('#synced-search input[name="brands"]')
    .val(_searchSession.sBrands)
    .tagsinput({
      itemValue: "id",
      itemText: "name",
      typeaheadjs: {
        name: "brands",
        displayKey: "name",
        source: brands.ttAdapter()
      }
    });

  initTypeaheadInputs();
}

function initTypeaheadInputs() {
  const suggestionsLimit = 12;
  for (let isSyncProduct of [true, false]) {
    const url = new URL(
      "product/skuOrNameProductSuggestions",
      window.location.origin
    );
    url.searchParams.append("syncedProducts", isSyncProduct.toString());
    url.searchParams.append("suggestionsLimit", suggestionsLimit.toString());

    const source = new Bloodhound({
      datumTokenizer: Bloodhound.tokenizers.obj.whitespace("value"),
      queryTokenizer: Bloodhound.tokenizers.whitespace,
      remote: {
        url: url.toString() + "&skuOrName=%skuOrName",
        wildcard: "%skuOrName",
        replace: function(url, skuOrName) {
          if (!isSyncProduct) {
            skuOrName += "&withStock=" + _searchSession.withStock;
          }
          let finalUrl = url.replace("%skuOrName", skuOrName);
          if (!isSyncProduct) {
            const _url = new URL(finalUrl);
            _url.searchParams.append("providerId", providersSelect.val());
            _url.searchParams.append("brandId", brandsSelect.val());
            finalUrl = _url.toString();
          }
          return finalUrl;
        }
      }
    });
    source.initialize();
    const formId = isSyncProduct ? "synced-search" : "unsynced-search";
    const initialVal = isSyncProduct
      ? _searchSession.sSkuOrName
      : _searchSession.uSkuOrName;

    $(`#${formId} input[name="sku-or-name"]`)
      .val(initialVal)
      .typeahead(
        {
          highlight: true
        },
        {
          source: source.ttAdapter(),
          limit: suggestionsLimit,
          displayKey: "value",
          valueKey: "value"
        }
      );
    $(`#${formId} button.btn-search-products`).click(function() {
      if (isSyncProduct) {
        loadSyncedProducts(1);
      } else {
        loadUnsyncedProducts(1);
      }
    });
  }
}

$(async function() {
  window.providersSelect = $("#unsynced-providers");
  window.brandsSelect = $("#unsynced-brands");
  window.syncedSpinner = $("#synced-loading");
  window.unsyncedSpinner = $("#unsynced-loading");

  await loadSearchSession();
  $(window).on("unload", saveSearchSession);

  $("#withStock").attr("checked", _searchSession.withStock);
  $("#withStock").click(function() {
    _searchSession.withStock = $(this).is(":checked");
  });

  $(`#${_searchSession.activeTab}`).click();

  initItemsPerPage();
  initUnsyncTabSelects();
  initSyncProductModal();
  initPagination();
  initFilterInputs();

  loadSyncedProducts(_searchSession.sPage);

  if (_searchSession.providerId != null) {
    $("#unsynced-providers").val(_searchSession.providerId);
  }

  await loadBrands();
  if (_searchSession.providerBrandId != null) {
    $("#unsynced-brands").val(_searchSession.providerBrandId);
  }
  loadUnsyncedProducts(_searchSession.uPage);
});
