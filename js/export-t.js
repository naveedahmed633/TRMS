(function (window, document) {
    "use strict";

    var PDF_BUTTON_IDS = [
        "generate-monthly-time-sheet",
        "generate-monthly-punched-photo",
        "generate-captured-photo-sheet",
        "generate-loan-report",
        "generate-payroll-report"
    ];

    function hasJsPdf() {
        return !!((window.jspdf && window.jspdf.jsPDF) || window.jsPDF);
    }

    function getJsPdfCtor() {
        return (window.jspdf && window.jspdf.jsPDF) || window.jsPDF;
    }

    function closestVisibleTable() {
        var selectors = [
            "#datatable-buttons",
            "#datatable",
            ".dataTable",
            "table.table",
            "table"
        ];

        for (var i = 0; i < selectors.length; i++) {
            var nodes = document.querySelectorAll(selectors[i]);
            for (var j = 0; j < nodes.length; j++) {
                var el = nodes[j];
                if (el && el.offsetParent !== null && el.querySelectorAll("tr").length > 1) {
                    return el;
                }
            }
        }

        return null;
    }

    function buildFileName() {
        var title = (document.title || "report").replace(/[^\w\- ]+/g, "").trim().replace(/\s+/g, "-");
        if (!title) {
            title = "report";
        }
        return title + "-" + new Date().toISOString().slice(0, 10) + ".pdf";
    }

    function isArabicLanguage() {
        var ddl = document.querySelector("#ddl_language");
        if (ddl && (ddl.value || "").toLowerCase() === "ar") {
            return true;
        }

        var hiddenLang = document.querySelector("input[name='lang'], input[name='GV_Langauge']");
        if (hiddenLang && (hiddenLang.value || "").toLowerCase() === "ar") {
            return true;
        }

        var htmlLang = (document.documentElement.getAttribute("lang") || "").toLowerCase();
        return htmlLang.indexOf("ar") === 0;
    }

    function getPdfLabels(isArabic) {
        if (isArabic) {
            return {
                governorate: "محافظة جنوب الباطنة",
                labelDate: "التاريخ",
                labelTime: "الوقت",
                labelName: "الاسم",
                labelEmployeeCode: "رمز الموظف",
                labelFilterDate: "تاريخ التقرير",
                labelPrintedBy: "تمت الطباعة بواسطة"
            };
        }

        return {
            governorate: "Al Batinah South Governorate",
            labelDate: "Date",
            labelTime: "Time",
            labelName: "Name",
            labelEmployeeCode: "Employee Code",
            labelFilterDate: "Date",
            labelPrintedBy: "Report Printed By"
        };
    }

    function pad2(num) {
        return (num < 10 ? "0" : "") + num;
    }

    function getDateTimeParts() {
        var now = new Date();
        return {
            date: pad2(now.getDate()) + "/" + pad2(now.getMonth() + 1) + "/" + now.getFullYear(),
            time: pad2(now.getHours()) + ":" + pad2(now.getMinutes()) + ":" + pad2(now.getSeconds())
        };
    }

    function pickFirst(root, selectors) {
        for (var i = 0; i < selectors.length; i++) {
            var node = root.querySelector(selectors[i]);
            if (node) return node;
        }
        return null;
    }

    function getCurrentUserName() {
        var fromPageTitle = document.querySelector(".page-title .text-success");
        if (fromPageTitle && (fromPageTitle.textContent || "").trim()) {
            return (fromPageTitle.textContent || "").trim();
        }

        var navUser = document.querySelector(".nav-user");
        if (navUser && (navUser.textContent || "").trim()) {
            return (navUser.textContent || "").trim();
        }

        var titleText = (document.querySelector(".page-title") && document.querySelector(".page-title").textContent || "").trim();
        if (titleText) {
            var m = titleText.match(/Name\s*:\s*([^\]\n\r]+)/i);
            if (m && m[1] && m[1].trim()) return m[1].trim();
        }

        return "N/A";
    }

    function getCurrentUserCode() {
        var loginIdNode = document.querySelector(".page-title .text-warning");
        if (loginIdNode) {
            var fromTitle = (loginIdNode.getAttribute("title") || "").trim();
            if (fromTitle) return fromTitle;
            var fromText = (loginIdNode.textContent || "").trim();
            if (fromText) return fromText;
        }
        var titleText = (document.querySelector(".page-title") && document.querySelector(".page-title").textContent || "").trim();
        if (titleText) {
            var m = titleText.match(/Login ID\s*:\s*([^\]&\n\r]+)/i);
            if (m && m[1] && m[1].trim()) return m[1].trim();
        }
        return "N/A";
    }

    function getChosenSelectedText(selectNode) {
        if (!selectNode || !selectNode.id) return "";
        var chosen = document.querySelector("#" + selectNode.id + "_chosen .chosen-single span");
        if (chosen && (chosen.textContent || "").trim()) {
            return (chosen.textContent || "").trim();
        }
        return "";
    }

    function getTableHeaderIndex(table, keywords) {
        if (!table) return -1;
        var ths = table.querySelectorAll("thead th");
        for (var i = 0; i < ths.length; i++) {
            var h = ((ths[i].textContent || "").trim()).toLowerCase();
            for (var k = 0; k < keywords.length; k++) {
                if (h.indexOf(keywords[k]) > -1) return i;
            }
        }
        return -1;
    }

    function getFirstBodyCellByIndex(table, idx) {
        if (!table || idx < 0) return "";
        var firstRow = table.querySelector("tbody tr");
        if (!firstRow) return "";
        var cells = firstRow.querySelectorAll("td");
        if (!cells || cells.length <= idx) return "";
        return (cells[idx].textContent || "").trim();
    }

    function getEmployeeName(contextRoot, table) {
        var root = contextRoot || document;
        var selectors = [
            "#EmployeeName",
            "#employee_name",
            "#monthly-time-sheet-employees option:checked",
            "#consolidated-employees option:checked",
            "#monthly-punched-photo-employees option:checked",
            "#monthly-geo-phencing-employees option:checked",
            "#monthly-captured-photo-employees option:checked",
            "#EmployeeId option:checked",
            "#employee_id option:checked",
            "select[name='employee_id'] option:checked",
            "select[name='employee_d'] option:checked"
        ];
        for (var i = 0; i < selectors.length; i++) {
            var node = root.querySelector(selectors[i]) || document.querySelector(selectors[i]);
            if (!node) continue;
            var value = (node.textContent || node.value || "").trim();
            if (value && value.toLowerCase() !== "all" && value.toLowerCase() !== "select employee") {
                if (value.indexOf(" - ") > -1) {
                    return value.split(" - ").slice(1).join(" - ").trim() || value;
                }
                return value;
            }
        }
        var selected = pickFirst(root, ["select[name='employee_id']", "select[name='employee_d']"]) || document.querySelector("select[name='employee_id'], select[name='employee_d']");
        if (selected && selected.selectedOptions && selected.selectedOptions.length > 0) {
            var selectedText = (selected.selectedOptions[0].textContent || "").trim();
            if (selectedText && selectedText.toLowerCase() !== "all") {
                if (selectedText.indexOf(" - ") > -1) {
                    return selectedText.split(" - ").slice(1).join(" - ").trim() || selectedText;
                }
                return selectedText;
            }
        }

        if (selected) {
            var chosenText = getChosenSelectedText(selected);
            if (chosenText && chosenText.toLowerCase() !== "all") {
                if (chosenText.indexOf(" - ") > -1) {
                    return chosenText.split(" - ").slice(1).join(" - ").trim() || chosenText;
                }
                return chosenText;
            }
        }

        var nameIdx = getTableHeaderIndex(table, ["name", "fname", "employee name", "الاسم"]);
        var nameFromTable = getFirstBodyCellByIndex(table, nameIdx);
        if (nameFromTable) return nameFromTable;

        return getCurrentUserName();
    }

    function getEmployeeCode(contextRoot, table) {
        var root = contextRoot || document;
        var codeInput = pickFirst(root, ["#EmployeeCode", "#employee_code"]) || document.querySelector("#EmployeeCode, #employee_code");
        if (codeInput && (codeInput.value || "").trim()) {
            return (codeInput.value || "").trim();
        }

        var selectedOptionSelectors = [
            "#monthly-time-sheet-employees option:checked",
            "#consolidated-employees option:checked",
            "#monthly-punched-photo-employees option:checked",
            "#monthly-geo-phencing-employees option:checked",
            "#monthly-captured-photo-employees option:checked",
            "#employee_id option:checked",
            "select[name='employee_id'] option:checked",
            "select[name='employee_d'] option:checked"
        ];

        for (var s = 0; s < selectedOptionSelectors.length; s++) {
            var opt = root.querySelector(selectedOptionSelectors[s]) || document.querySelector(selectedOptionSelectors[s]);
            if (!opt) continue;
            var text = (opt.textContent || "").trim();
            if (!text) continue;
            if (text.indexOf(" - ") > -1) {
                return text.split(" - ")[0].trim();
            }
        }

        var selectors = [
            "#monthly-time-sheet-employees",
            "#consolidated-employees",
            "#monthly-punched-photo-employees",
            "#monthly-geo-phencing-employees",
            "#monthly-captured-photo-employees",
            "#EmployeeId",
            "#employee_id",
            "select[name='employee_id']",
            "select[name='employee_d']"
        ];
        for (var i = 0; i < selectors.length; i++) {
            var node = root.querySelector(selectors[i]) || document.querySelector(selectors[i]);
            if (!node) continue;
            var value = (node.value || "").trim();
            if (value && value !== "-1" && value !== "0") {
                return value;
            }
        }

        var selected = pickFirst(root, ["select[name='employee_id']", "select[name='employee_d']"]) || document.querySelector("select[name='employee_id'], select[name='employee_d']");
        if (selected) {
            var chosenText = getChosenSelectedText(selected);
            if (chosenText) {
                if (chosenText.indexOf(" - ") > -1) return chosenText.split(" - ")[0].trim();
                return chosenText;
            }
        }

        var codeIdx = getTableHeaderIndex(table, ["empcode", "employee code", "code", "رمز"]);
        var codeFromTable = getFirstBodyCellByIndex(table, codeIdx);
        if (codeFromTable) return codeFromTable;

        return getCurrentUserCode();
    }

    function parseMonthValue(monthValue) {
        if (!monthValue) return null;
        var normalized = monthValue.trim();
        var parts = normalized.split("-");
        if (parts.length !== 2) return null;

        var year;
        var month;

        if (parts[0].length === 4) {
            year = parseInt(parts[0], 10);
            month = parseInt(parts[1], 10);
        } else if (parts[1].length === 4) {
            month = parseInt(parts[0], 10);
            year = parseInt(parts[1], 10);
        } else {
            return null;
        }

        if (!year || !month || month < 1 || month > 12) return null;
        var lastDay = new Date(year, month, 0).getDate();
        return {
            from: "01/" + pad2(month) + "/" + year,
            to: pad2(lastDay) + "/" + pad2(month) + "/" + year
        };
    }

    function getFilterDateRange(contextRoot) {
        var root = contextRoot || document;
        var fromInput = pickFirst(root, ["[name='from_date']", "[name='fromDate']", "#date_from", "#from-date", "#from_date"]) || document.querySelector("[name='from_date'], [name='fromDate'], #date_from, #from-date, #from_date");
        var toInput = pickFirst(root, ["[name='to_date']", "[name='toDate']", "#date_to", "#to-date", "#to_date"]) || document.querySelector("[name='to_date'], [name='toDate'], #date_to, #to-date, #to_date");
        var fromValue = fromInput && (fromInput.value || "").trim();
        var toValue = toInput && (toInput.value || "").trim();
        if (fromValue && toValue) {
            return fromValue + " to " + toValue;
        }

        var monthInput = pickFirst(root, ["[name='month']", "#monthly-time-sheet-month", "#month_year", "#pdf-month", "[name='month_year']", "[name='lmonth_year']"]) || document.querySelector("[name='month'], #monthly-time-sheet-month, #month_year, #pdf-month, [name='month_year'], [name='lmonth_year']");
        var monthValue = monthInput && (monthInput.value || "").trim();
        if (monthValue) {
            var parsed = parseMonthValue(monthValue);
            if (parsed) {
                return parsed.from + " to " + parsed.to;
            }
            return monthValue;
        }

        var anyDate = pickFirst(root, ["input[type='date']", "input.datepicker", "input[name*='date']", "input[name*='month']"]);
        if (anyDate && (anyDate.value || "").trim()) {
            return (anyDate.value || "").trim();
        }

        return "N/A";
    }

    function loadImageAsDataUrl(src, callback) {
        var img = new Image();
        img.crossOrigin = "anonymous";
        img.onload = function () {
            try {
                var canvas = document.createElement("canvas");
                canvas.width = img.naturalWidth || img.width;
                canvas.height = img.naturalHeight || img.height;
                var ctx = canvas.getContext("2d");
                ctx.drawImage(img, 0, 0);
                callback(canvas.toDataURL("image/png"));
            } catch (err) {
                callback(null);
            }
        };
        img.onerror = function () {
            callback(null);
        };
        img.src = src;
    }

    function getPrintedByName() {
        return getCurrentUserName();
    }

    function setPdfFont(doc, isArabic, isBold) {
        if (isArabic) {
            try {
                doc.setFont("Amiri-Regular", "normal");
                return;
            } catch (e) {
                doc.setFont("helvetica", isBold ? "bold" : "normal");
                return;
            }
        }
        doc.setFont("helvetica", isBold ? "bold" : "normal");
    }

    function drawPdfHeader(doc, reportTitle, headerInfo, labels, isArabic, done) {
        var pageWidth = doc.internal.pageSize.getWidth();
        var leftX = 14;
        var rightX = pageWidth - 14;
        var centerX = pageWidth / 2;
        var logoPath = window.location.origin + "/images/TRMS-LM.png";

        setPdfFont(doc, isArabic, false);
        doc.setFontSize(11);
        if (isArabic) {
            doc.text(labels.governorate, rightX, 12, { align: "right" });
        } else {
            doc.text(labels.governorate, leftX, 12);
        }

        doc.setFontSize(10);
        if (isArabic) {
            doc.text(labels.labelDate + " : " + headerInfo.date, leftX, 10);
            doc.text(labels.labelTime + " : " + headerInfo.time, leftX, 16);
        } else {
            doc.text(labels.labelDate + " : " + headerInfo.date, rightX, 10, { align: "right" });
            doc.text(labels.labelTime + " : " + headerInfo.time, rightX, 16, { align: "right" });
        }

        loadImageAsDataUrl(logoPath, function (logoData) {
            if (logoData) {
                doc.addImage(logoData, "PNG", centerX - 10, 4, 20, 20);
            }

            doc.setLineWidth(0.3);
            doc.line(leftX, 24, rightX, 24);

            setPdfFont(doc, isArabic, false);
            doc.setFontSize(10);
            if (isArabic) {
                doc.text(labels.labelName + ": " + headerInfo.employeeName, rightX, 31, { align: "right" });
                doc.text(labels.labelEmployeeCode + ": " + headerInfo.employeeCode, rightX, 37, { align: "right" });
                doc.text(labels.labelFilterDate + ": " + headerInfo.filterDateRange, rightX, 43, { align: "right" });
                doc.text(reportTitle, leftX, 34);
            } else {
                doc.text(labels.labelName + ": " + headerInfo.employeeName, leftX, 31);
                doc.text(labels.labelEmployeeCode + ": " + headerInfo.employeeCode, leftX, 37);
                doc.text(labels.labelFilterDate + ": " + headerInfo.filterDateRange, leftX, 43);
                doc.text(reportTitle, rightX, 34, { align: "right" });
            }

            done(50);
        });
    }

    function exportVisibleTableAsPdf(done, triggerElement) {
        if (!hasJsPdf()) {
            alert("PDF library not loaded.");
            done(false);
            return;
        }

        var table = closestVisibleTable();
        if (!table) {
            alert("No report data found to export.");
            done(false);
            return;
        }

        var jsPDF = getJsPdfCtor();
        var doc = new jsPDF("l", "mm", "a4");
        var isArabic = isArabicLanguage();
        var labels = getPdfLabels(isArabic);
        var title = (document.title || "Report").trim();
        var dateTime = getDateTimeParts();
        var contextRoot = (triggerElement && (triggerElement.form || triggerElement.closest("form"))) || document;
        var headerInfo = {
            date: dateTime.date,
            time: dateTime.time,
            employeeName: getEmployeeName(contextRoot, table),
            employeeCode: getEmployeeCode(contextRoot, table),
            filterDateRange: getFilterDateRange(contextRoot)
        };
        var printedBy = getPrintedByName();

        setPdfFont(doc, isArabic, false);

        if (typeof doc.autoTable !== "function" && window.jspdfAutoTable) {
            doc.autoTable = window.jspdfAutoTable.default || window.jspdfAutoTable;
        }

        if (typeof doc.autoTable !== "function") {
            alert("PDF table plugin not loaded.");
            done(false);
            return;
        }

        drawPdfHeader(doc, title, headerInfo, labels, isArabic, function (startY) {
            doc.autoTable({
                html: table,
                startY: startY,
                theme: "grid",
                styles: {
                    fontSize: 8,
                    cellPadding: 1.5,
                    overflow: "linebreak",
                    halign: isArabic ? "right" : "left",
                    font: isArabic ? "Amiri-Regular" : "helvetica"
                },
                headStyles: {
                    fillColor: [52, 58, 64],
                    textColor: 255,
                    halign: isArabic ? "right" : "left",
                    font: isArabic ? "Amiri-Regular" : "helvetica"
                }
            });

            var y = (doc.lastAutoTable && doc.lastAutoTable.finalY ? doc.lastAutoTable.finalY : startY) + 10;
            if (y > 200) {
                doc.addPage();
                y = 20;
            }
            setPdfFont(doc, isArabic, true);
            if (isArabic) {
                doc.text(labels.labelPrintedBy + ": " + printedBy, doc.internal.pageSize.getWidth() - 14, y, { align: "right" });
            } else {
                doc.text(labels.labelPrintedBy + ": " + printedBy, 14, y);
            }

            doc.save(buildFileName());
            done(true);
        });
    }

    function shouldHandlePdfButton(el) {
        if (!el || !el.id) {
            return false;
        }

        if (el.id === "generate-payroll") {
            return false;
        }

        if (PDF_BUTTON_IDS.indexOf(el.id) >= 0) {
            return true;
        }

        if (el.id.indexOf("generate-") === 0) {
            var text = (el.textContent || el.value || "").toLowerCase();
            if (text.indexOf("pdf") >= 0 || text.indexOf("statement") >= 0 || text.indexOf("download") >= 0) {
                return true;
            }
        }

        return false;
    }

    document.addEventListener("click", function (e) {
        var button = e.target.closest("button, input[type='submit']");
        if (!button || !shouldHandlePdfButton(button)) {
            return;
        }

        e.preventDefault();
        e.stopPropagation();
        exportVisibleTableAsPdf(function () { }, button);
    }, true);

    window.ExportT = {
        exportVisibleTableAsPdf: exportVisibleTableAsPdf
    };
})(window, document);
