(function ($) {
    "use strict";

    var currencyFormatter = new Intl.NumberFormat("fr-FR", { maximumFractionDigits: 0 });

    function initBootstrapComponents(scope) {
        var root = scope || document;

        root.querySelectorAll("[data-bs-toggle='tooltip']").forEach(function (element) {
            bootstrap.Tooltip.getOrCreateInstance(element);
        });

        root.querySelectorAll("[data-bs-toggle='dropdown']").forEach(function (element) {
            bootstrap.Dropdown.getOrCreateInstance(element, {
                boundary: document.body,
                popperConfig: function (config) {
                    return Object.assign({}, config, { strategy: "fixed" });
                }
            });
        });
    }

    function getRawCellValue(settings, dataIndex, columnIndex) {
        var cell = settings.aoData[dataIndex] && settings.aoData[dataIndex].anCells[columnIndex];
        if (!cell) {
            return "";
        }

        return cell.getAttribute("data-filter") || cell.textContent.trim();
    }

    if (window.DataTable) {
        DataTable.ext.search.push(function (settings, data, dataIndex) {
            var table = settings.nTable;
            if (!table.id) {
                return true;
            }

            var controls = document.querySelectorAll("[data-table='#" + table.id + "'][data-filter-type]");
            return Array.prototype.every.call(controls, function (control) {
                if (!control.value) {
                    return true;
                }

                var columnIndex = parseInt(control.dataset.column, 10);
                var rawValue = getRawCellValue(settings, dataIndex, columnIndex);
                var filterType = control.dataset.filterType;

                if (filterType === "date-min" || filterType === "date-max") {
                    var cellDate = Date.parse(rawValue);
                    var filterDate = Date.parse(control.value + "T00:00:00Z");
                    if (Number.isNaN(cellDate) || Number.isNaN(filterDate)) {
                        return true;
                    }
                    return filterType === "date-min" ? cellDate >= filterDate : cellDate <= filterDate;
                }

                var cellNumber = parseFloat(rawValue);
                var filterNumber = parseFloat(control.value);
                if (Number.isNaN(cellNumber) || Number.isNaN(filterNumber)) {
                    return true;
                }
                return filterType === "number-min" ? cellNumber >= filterNumber : cellNumber <= filterNumber;
            });
        });
    }

    function filterStorageKey(tableElement) {
        return "gestion-hoteliere:filters:" + window.location.pathname + ":" + tableElement.id;
    }

    function saveCustomFilters(tableElement) {
        var values = {};
        document.querySelectorAll("[data-table='#" + tableElement.id + "']").forEach(function (control) {
            if (control.id) {
                values[control.id] = control.value;
            }
        });
        window.localStorage.setItem(filterStorageKey(tableElement), JSON.stringify(values));
    }

    function restoreCustomFilters(tableElement) {
        try {
            var values = JSON.parse(window.localStorage.getItem(filterStorageKey(tableElement)) || "{}");
            Object.keys(values).forEach(function (id) {
                var control = document.getElementById(id);
                if (control) {
                    control.value = values[id];
                }
            });
        } catch (_) {
            window.localStorage.removeItem(filterStorageKey(tableElement));
        }
    }

    function applyTableFilters(api, tableElement) {
        var search = document.querySelector("[data-table='#" + tableElement.id + "'][data-dt-search]");
        api.search(search ? search.value : "");

        document.querySelectorAll("[data-table='#" + tableElement.id + "'][data-column-filter]").forEach(function (control) {
            var column = api.column(parseInt(control.dataset.column, 10));
            var value = control.value;
            var expression = value ? "^" + DataTable.util.escapeRegex(value) + "$" : "";
            column.search(expression, true, false);
        });

        api.draw();
    }

    function initDataTables() {
        if (!window.DataTable) {
            return;
        }

        document.querySelectorAll("table.js-data-table").forEach(function (tableElement) {
            if (DataTable.isDataTable(tableElement)) {
                return;
            }

            restoreCustomFilters(tableElement);
            var order = [[0, "asc"]];
            try {
                order = JSON.parse(tableElement.dataset.order || "[[0,\"asc\"]]");
            } catch (_) {
                order = [[0, "asc"]];
            }

            var api = new DataTable(tableElement, {
                responsive: true,
                pageLength: parseInt(tableElement.dataset.pageLength || "10", 10),
                lengthMenu: [10, 25, 50, 100],
                stateSave: true,
                order: order,
                columnDefs: [{ targets: "no-sort", orderable: false, searchable: false }],
                layout: {
                    topStart: "pageLength",
                    topEnd: null,
                    bottomStart: "info",
                    bottomEnd: "paging"
                },
                language: {
                    emptyTable: "Aucune donnée disponible",
                    info: "_START_ à _END_ sur _TOTAL_ éléments",
                    infoEmpty: "0 élément",
                    infoFiltered: "(filtré sur _MAX_)",
                    lengthMenu: "Afficher _MENU_",
                    loadingRecords: "Chargement…",
                    processing: "Traitement…",
                    zeroRecords: "Aucun résultat ne correspond aux filtres",
                    paginate: { first: "Premier", last: "Dernier", next: "Suivant", previous: "Précédent" }
                }
            });

            applyTableFilters(api, tableElement);
            api.on("draw", function () { initBootstrapComponents(tableElement); });

            document.querySelectorAll("[data-table='#" + tableElement.id + "']").forEach(function (control) {
                var eventName = control.matches("input") ? "input" : "change";
                control.addEventListener(eventName, function () {
                    saveCustomFilters(tableElement);
                    applyTableFilters(api, tableElement);
                });
            });

            var reset = document.querySelector("[data-reset-table='#" + tableElement.id + "']");
            if (reset) {
                reset.addEventListener("click", function () {
                    document.querySelectorAll("[data-table='#" + tableElement.id + "']").forEach(function (control) {
                        control.value = "";
                    });
                    window.localStorage.removeItem(filterStorageKey(tableElement));
                    api.search("").columns().search("");
                    api.draw();
                });
            }
        });
    }

    function updateReservationEstimate(form) {
        var type = form.querySelector(".js-room-type-select");
        var arrival = form.querySelector(".js-arrival-date");
        var departure = form.querySelector(".js-departure-date");
        if (!type || !arrival || !departure) {
            return;
        }

        if (arrival.value) {
            var minimumDeparture = new Date(arrival.value + "T00:00:00Z");
            minimumDeparture.setUTCDate(minimumDeparture.getUTCDate() + 1);
            departure.min = minimumDeparture.toISOString().slice(0, 10);
        }

        var selected = type.options[type.selectedIndex];
        var price = selected ? parseFloat(selected.dataset.price || "0") : 0;
        var start = Date.parse(arrival.value + "T00:00:00Z");
        var end = Date.parse(departure.value + "T00:00:00Z");
        var nights = !Number.isNaN(start) && !Number.isNaN(end) && end > start
            ? Math.max(1, Math.round((end - start) / 86400000))
            : 0;

        form.querySelector(".js-estimate-nights").textContent = nights;
        form.querySelector(".js-estimate-price").textContent = currencyFormatter.format(price);
        form.querySelector(".js-estimate-total").textContent = currencyFormatter.format(nights * price);
    }

    function initDynamicForms() {
        document.querySelectorAll(".js-reservation-form").forEach(function (form) {
            form.querySelectorAll(".js-room-type-select, .js-arrival-date, .js-departure-date").forEach(function (control) {
                control.addEventListener("change", function () { updateReservationEstimate(form); });
            });
            var roomSelect = form.querySelector(".js-reservation-room");
            if (roomSelect) {
                roomSelect.addEventListener("change", function () {
                    var selectedRoom = roomSelect.options[roomSelect.selectedIndex];
                    var typeSelect = form.querySelector(".js-room-type-select");
                    if (selectedRoom && selectedRoom.dataset.typeId && typeSelect) {
                        typeSelect.value = selectedRoom.dataset.typeId;
                        typeSelect.dispatchEvent(new Event("change"));
                    }
                });
            }
            updateReservationEstimate(form);
        });

        document.querySelectorAll(".js-room-capacity-source").forEach(function (select) {
            select.addEventListener("change", function () {
                var selected = select.options[select.selectedIndex];
                var bedInput = select.closest("form").querySelector(".js-bed-count");
                if (selected && bedInput && selected.dataset.capacity) {
                    bedInput.value = selected.dataset.capacity;
                    if ($.fn.valid) {
                        $(bedInput).valid();
                    }
                }
            });
        });

        document.querySelectorAll(".js-invoice-form").forEach(function (form) {
            var update = function () {
                var amount = parseFloat(form.querySelector(".js-invoice-ht").value || "0");
                var tax = parseFloat(form.querySelector(".js-invoice-tax").value || "0");
                form.querySelector(".js-invoice-total").textContent = currencyFormatter.format(Math.max(0, amount + tax));
            };
            form.querySelectorAll(".js-invoice-ht, .js-invoice-tax").forEach(function (control) {
                control.addEventListener("input", update);
            });
            update();
        });
    }

    function upgradeLegacyForms() {
        document.querySelectorAll("main form").forEach(function (form, index) {
            if (form.closest(".modal") || form.querySelector(":scope > .form-surface")) {
                return;
            }

            var submit = form.querySelector("button[type='submit'], input[type='submit']");
            var card = form.closest(".row");
            if (!submit || !card) {
                return;
            }

            var formId = form.id || "legacyCrudForm" + index;
            form.id = formId;
            submit.setAttribute("form", formId);

            var trailing = card.nextElementSibling;
            var back = trailing ? trailing.querySelector("a[href]") : null;
            var actions = document.createElement("div");
            actions.className = "form-actions";
            if (back) {
                back.innerHTML = "<i class='bi bi-arrow-left' aria-hidden='true'></i> Retour";
                actions.appendChild(back);
                if (!trailing.children.length) {
                    trailing.remove();
                }
            } else {
                actions.appendChild(document.createElement("span"));
            }

            if (submit.tagName === "INPUT") {
                var button = document.createElement("button");
                button.type = "submit";
                button.className = submit.className;
                button.setAttribute("form", formId);
                button.innerHTML = "<i class='bi bi-check-lg' aria-hidden='true'></i> " + submit.value;
                submit.replaceWith(button);
                submit = button;
            }
            actions.appendChild(submit);
            card.insertAdjacentElement("afterend", actions);
        });
    }

    $(function () {
        upgradeLegacyForms();
        initBootstrapComponents(document);
        initDataTables();
        initDynamicForms();

        $(".app-toast").each(function () {
            new bootstrap.Toast(this, { delay: 4200 }).show();
        });

        $(document).on("click", ".js-confirm-action", function () {
            var $button = $(this);
            var modal = bootstrap.Modal.getOrCreateInstance(document.getElementById("appConfirmModal"));
            $("#appConfirmTitle").text($button.data("confirmTitle") || "Confirmer l'action");
            $(".js-confirm-message").text($button.data("confirmMessage") || "Cette action va être appliquée.");
            $(".js-confirm-form").attr("action", $button.data("postUrl"));
            $(".js-confirm-submit")
                .attr("class", "btn js-confirm-submit " + ($button.data("confirmClass") || "btn-danger"))
                .text($button.data("confirmButton") || "Confirmer");
            modal.show();
        });

        $(document).on("click", ".js-open-payment", function () {
            var modalElement = document.getElementById("paymentModal");
            var modal = bootstrap.Modal.getOrCreateInstance(modalElement);
            $(".js-payment-modal-content").html("<div class='modal-body text-muted'>Chargement…</div>");
            modal.show();
            $.get($(this).data("paymentUrl")).done(function (html) {
                $(".js-payment-modal-content").html(html);
                if ($.validator && $.validator.unobtrusive) {
                    $.validator.unobtrusive.parse($(".js-payment-modal-content"));
                }
            });
        });

        $(document).on("submit", ".js-payment-form", function (event) {
            event.preventDefault();
            var $form = $(this);
            $.ajax({ url: $form.attr("action"), method: "POST", data: $form.serialize() })
                .done(function (response) { if (response && response.success) { window.location.reload(); } })
                .fail(function (xhr) {
                    $(".js-payment-modal-content").html(xhr.responseText);
                    if ($.validator && $.validator.unobtrusive) {
                        $.validator.unobtrusive.parse($(".js-payment-modal-content"));
                    }
                });
        });

        $(".js-kpi-number").each(function () {
            var $counter = $(this);
            var target = parseFloat(String($counter.data("target")).replace(",", "."));
            var hasDecimal = target % 1 !== 0;
            $({ value: 0 }).animate({ value: target }, {
                duration: 850,
                step: function (now) { $counter.text(hasDecimal ? now.toFixed(1) : Math.round(now)); },
                complete: function () { $counter.text(hasDecimal ? target.toFixed(1) : Math.round(target)); }
            });
        });

        $(".js-dashboard-filter").on("input", function () {
            var query = $(this).val().toString().trim().toLowerCase();
            $(".dashboard-searchable").each(function () {
                var $item = $(this);
                var text = ($item.data("search") || $item.text()).toString().toLowerCase();
                $item.toggleClass("dashboard-hidden", query.length > 0 && text.indexOf(query) === -1);
            });
        });
    });
})(window.jQuery);
