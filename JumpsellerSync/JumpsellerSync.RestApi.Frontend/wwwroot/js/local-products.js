async function loadLocalProducts(page) {
  if (page == null) {
    return;
  }

   const url = new URL("localProduct/products", window.location.origin);
  url.searchParams.append("page", page.toString());
  url.searchParams.append("limit", window._local_products_ipp);

  try {
    localProductsSpinner.fadeIn();
    const prodHtml = await $.get(url.toString());
    const localProds = $("#local-products")
      .hide()
      .html(prodHtml);
    const total = +$("#total-pages").data("total-pages") || 1;
    $("#local-products-pagination").bootpag({
      total: total,
      page: page,
      maxVisible: 5
    });
    _setInputPriceValues("#local-products-table");
    localProds.fadeIn();
  } catch (e) {
    displayErrorAlert("Ocurrió un error al cargar los productos.");
  } finally {
    localProductsSpinner.fadeOut();
  }
}

function initPagination() {
  const _bootpag = $.fn.bootpag;
  $.fn.bootpag = function(config) {
    const result = _bootpag.bind($(this))(config);
    $("ul.pagination.bootpag li>a").addClass("page-link");
    return result;
  };

  $("#local-products-pagination")
    .bootpag({
      total: 0,
      page: 1,
      maxVisible: 5,
      leaps: true
    })
    .on("page", async function(ev, pageNum) {
      await loadLocalProducts(pageNum);
    });
}

async function addLocalProducts() {
  const products = [];
  $(
    '#synced-products div.add-product-cbox-container input[type="checkbox"]:checked'
  ).each(function() {
    const productId = $(this).data("product-id");
    products.push({
      productId: productId,
      price: +$(`input[name="Price_${productId}"]`).val(),
      stock: +$(`input[name="Stock_${productId}"]`).val()
    });
  });
  if (products.length) {
    const url = new URL("localProduct/create", window.location.origin);
    try {
      searchLocalProductsSpinner.fadeIn();
      await $.post({
        url: url.toString(),
        data: JSON.stringify({ products: products }),
        contentType: "application/json; charset=utf-8"
      });
      loadLocalProducts(getCurrentPage());
      searchSyncedProducts();
    } catch (e) {
      displayErrorAlert("Ocurrió un error mientras se creaban los productos.");
    } finally {
      $("#addLocalProductModal").modal("hide");
      searchLocalProductsSpinner.fadeOut();
    }
  }
}

async function searchSyncedProducts() {
  const url = new URL("localProduct/syncedProducts", window.location.origin);
  url.searchParams.append("limit", window._local_products_ipp);
  url.searchParams.append(
    "skuOrName",
    $('#local-products-search input[name="sku-or-name"]').val()
  );
  url.searchParams.append(
    "brandId",
    $('#local-products-search select[name="brand"]').val()
  );

  try {
    searchLocalProductsSpinner.fadeIn();
    const prodHtml = await $.get(url.toString());
    const prods = $("#synced-products")
      .hide()
      .html(prodHtml);
    _setInputPriceValues("#synced-products");
    prods.fadeIn();
  } catch (e) {
    displayErrorAlert("Ocurrió un error al cargar los productos.");
  } finally {
    searchLocalProductsSpinner.fadeOut();
  }
}

function _setInputPriceValues(selector) {
  $(`${selector} input[type="number"][data-price]`).each(function() {
    const $el = $(this);
    $el.val($el.data("price"));
  });
}

function initAddProductModal() {
  window.searchLocalProductsSpinner = $("#synced-products-loading");
  searchLocalProductsSpinner.hide();

  $("#add-local-product").click(addLocalProducts);
  $("#local-products-search button.btn-search-products").click(
    searchSyncedProducts
  );
}

function getCurrentPage() {
  return +$("#local-products-pagination li.active").attr("data-lp");
}

function initItemsPerPage() {
  window._local_products_ipp = $("#items-per-page li.active").data("ipp");
  _activateIPPPage();
  $("#items-per-page li").click(function() {
    if ($(this).data("ipp") === window._local_products_ipp) {
      return;
    }
    window._local_products_ipp = $(this).data("ipp");
    _activateIPPPage();
    loadLocalProducts(getCurrentPage());
  });

  function _activateIPPPage() {
    $("#items-per-page li").removeClass("active");
    $(`#items-per-page li[data-ipp="${window._local_products_ipp}"]`).addClass(
      "active"
    );
  }
}

async function updateLocalProducts() {
  const products = [];
  $("#local-products-table tr[data-local-product-id]").each(function() {
    const $row = $(this);
    products.push({
      id: $row.data("local-product-id"),
      price: +$row.find('input[name^="Price_"]').val(),
      stock: +$row.find('input[name^="Stock_"]').val()
    });
  });

  if (products.length) {
    const url = new URL("localProduct/update", window.location.origin);
    try {
      localProductsSpinner.fadeIn();
      await $.ajax({
        type: "PUT",
        url: url.toString(),
        data: JSON.stringify({ products: products }),
        contentType: "application/json; charset=utf-8"
      });
      displaySuccessAlert("Productos actualizados correctamente.");
    } catch (e) {
      displayErrorAlert(
        "Ha ocurrido un error mientras se actualizaban los productos."
      );
    } finally {
      localProductsSpinner.fadeOut();
    }
  }
}

$(async function() {
  window.localProductsSpinner = $("#local-products-loading");
  $("#update-local-products").click(updateLocalProducts);

  initPagination();
  initItemsPerPage();
  initAddProductModal();

  loadLocalProducts(1);
});
